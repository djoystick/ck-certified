﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using CK.Context;
using CK.Core;
using CK.Keyboard.Model;
using CK.Plugin;
using CK.Plugin.Config;
using CK.Storage;
using CK.Windows.App;
using CK.Windows.Config;
using ContextEditor.Resources;
using ContextEditor.ViewModels;

namespace ContextEditor
{
    [Plugin( KeyboardEditor.PluginIdString,
        PublicName = PluginPublicName,
        Version = KeyboardEditor.PluginIdVersion )]
    public class KeyboardEditor : IPlugin, IKeyboardEditorRoot
    {
        const string PluginIdString = "{66AD1D1C-BF19-405D-93D3-30CA39B9E52F}";
        Guid PluginGuid = new Guid( PluginIdString );
        const string PluginIdVersion = "1.0.0";
        const string PluginPublicName = "Keyboard editor";
        public static readonly INamedVersionedUniqueId PluginId = new SimpleNamedVersionedUniqueId( PluginIdString, PluginIdVersion, PluginPublicName );

        [ConfigurationAccessor( "{36C4764A-111C-45e4-83D6-E38FC1DF5979}" )]
        public IPluginConfigAccessor SkinConfiguration { get; set; }

        [DynamicService( Requires = RunningRequirement.MustExistAndRun )]
        public IService<IKeyboardContext> KeyboardContext { get; set; }

        public IPluginConfigAccessor Config { get; set; }
        
        [RequiredService( Required = true )]
        public IContext Context { get; set; }

        KeyboardEditorBootstrapper bootstrap;
        WindowManager _windowManager;
        AppViewModel _appViewModel;
        Window _mainWindow;
        bool _stopping;

        public bool Setup( IPluginSetupInfo info )
        {
            bootstrap = new KeyboardEditorBootstrapper();
            return bootstrap != null;
        }

        public void Start()
        {
            _windowManager = new WindowManager();
            _appViewModel = new AppViewModel( this );
            var dic = new Dictionary<string, object>();
            dic.Add("Topmost", false);
            _windowManager.ShowWindow( _appViewModel, null, dic );

            _mainWindow = _appViewModel.GetView( null ) as Window;
            _mainWindow.Closing += OnWindowClosing;
            _mainWindow.Topmost = false;
        }

        public void Stop()
        {
            _stopping = true;

            if( _mainWindow != null )
                _mainWindow.Close();
        }
            
        public void Teardown()
        {
            _stopping = false;
            _appViewModel = null;
            _windowManager = null;
            bootstrap = null;
        }

        void OnWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            //If we are already stopping. We do nothing.
            if( !_stopping )
            {   //If we are not already stopping, and we have a backup, apply it back.
                if( KeyboardBackup != null )
                {
                    ModalViewModel mvm = new ModalViewModel( R.WizardExitPopInTitle, R.WizardExitPopInDesc );
                    mvm.Buttons.Add( new ModalButton( mvm, R.Yes, ModalResult.Yes ) );
                    mvm.Buttons.Add( new ModalButton( mvm, R.No, ModalResult.No ) );
                    CustomMsgBox msgBox = new CustomMsgBox( ref mvm );

                    msgBox.ShowDialog();

                    if( mvm.ModalResult != ModalResult.Yes )
                    {
                        e.Cancel = true;
                        return;
                    }

                    //If the user really wants to quit the wizard, cancel all modifications
                    CancelModifications();
                }

                System.Action stop = () =>
                {
                    Context.ConfigManager.UserConfiguration.PluginsStatus.SetStatus( PluginGuid, ConfigPluginStatus.Disabled );
                    Context.PluginRunner.Apply();
                };
                e.Cancel = true;
                Dispatcher.CurrentDispatcher.BeginInvoke( stop, null );
            }

            if( _mainWindow != null )
                _mainWindow.Closing -= OnWindowClosing;
        }
     

        /// <summary>
        /// This property holds the version of the keyboard that is being edited, before any modification.
        /// if the Filepath contained in this object is null while we are editing a keyboard, it means that this keyboard is a new one.
        /// </summary>
        public KeyboardBackup KeyboardBackup { get; set; }

        ISharedDictionary _sharedDictionary;
        private ISharedDictionary SharedDictionary { get { return _sharedDictionary ?? ( _sharedDictionary = Context.ServiceContainer.GetService<ISharedDictionary>() ); } }

        /// <summary>
        /// Backs up a keyboard.
        /// Returns the file path where the keyboard has been backed up.
        /// 
        /// Throws a CKException if the IKeyboard implementation is not IStructuredSerializable
        /// </summary>
        /// <param name="keyboardToBackup">The keyboard ot backup</param>
        /// <returns>the path to the file in which the keyboard has been saved</returns>
        public string BackupKeyboard( IKeyboard keyboardToBackup )
        {
            string backupFileName = GenerateBackupFileName();
            IStructuredSerializable serializableModel = keyboardToBackup as IStructuredSerializable;
            if( serializableModel != null )
            {
                using( FileStream str = new FileStream( backupFileName, FileMode.CreateNew ) )
                {
                    using( IStructuredWriter writer = SimpleStructuredWriter.CreateWriter( str, Context.ServiceContainer ) )
                    {
                        SharedDictionary.RegisterWriter( writer );
                        writer.Xml.WriteStartElement( "Keyboard" );
                        serializableModel.WriteContent( writer );
                        writer.Xml.WriteEndElement();
                    }
                }
                KeyboardBackup = new KeyboardBackup( keyboardToBackup, backupFileName );
            }
            else throw new CKException( "The IKeyboard implementation should be IStructuredSerializable" );

            return backupFileName;
        }

        //outputs a filepath of the form : [tempfolder]/CiviKey/CK-[GUID].txt
        private string GenerateBackupFileName()
        {
            string fileName = String.Format( "CK-{0}.txt", Guid.NewGuid().ToString() );
            string folderPath = Path.Combine( System.IO.Path.GetTempPath(), "CiviKey", "KeyboardEditorBackup" );
            if( !Directory.Exists( folderPath ) ) Directory.CreateDirectory( folderPath );
            string keyboardFilePath = Path.Combine( folderPath, fileName );
            return keyboardFilePath;
        }

        /// <summary>
        /// Cancels all modifications made to the keyboard being currently modified
        /// Throws a <see cref="NullReferenceException"/> if <see cref="KeyboardBackup"/> is null.
        /// </summary>
        public void CancelModifications()
        {
            if( KeyboardBackup == null || KeyboardBackup.BackedUpKeyboard == null ) throw new NullReferenceException( "Can't cancel modifications on a null KeyboardBackup" );

            IKeyboard keyboardToRevert = KeyboardBackup.BackedUpKeyboard as IKeyboard;

            if( !String.IsNullOrWhiteSpace( KeyboardBackup.BackUpFilePath ) )
            {
                using( FileStream str = new FileStream( KeyboardBackup.BackUpFilePath, FileMode.Open ) )
                {
                    using( IStructuredReader reader = SimpleStructuredReader.CreateReader( str, Context.ServiceContainer ) )
                    {
                        IKeyboard keyboard = KeyboardContext.Service.Keyboards.Create( Guid.NewGuid().ToString() );
                        IStructuredSerializable serializableKeyboard = keyboard as IStructuredSerializable;

                        if( serializableKeyboard == null ) throw new CKException( "The IKeyboard implementation should be IStructuredSerializable" );

                        keyboardToRevert.Destroy();

                        _sharedDictionary.RegisterReader( reader, CK.SharedDic.MergeMode.None );
                        //Erasing all properties of the keyboard. We re-apply the backedup ones.
                        serializableKeyboard.ReadContent( reader );
                    }
                }
            }
            else ///if KeyboardBackupFilePath is null of whitespace, then we were creating a new keyboard. Reverting the chances means destroying the keyboard.
            {
                KeyboardBackup.BackedUpKeyboard.Destroy();
            }

            
            //To work properly, there should be some kind of ReadContent with a configurable MergeMode, just like with the pluginsData
            //if( !String.IsNullOrWhiteSpace( KeyboardBackup.BackUpFilePath ) )
            //{
            //    IStructuredSerializable serializableKeyboard = KeyboardBackup.BackedUpKeyboard as IStructuredSerializable;
            //    if( serializableKeyboard == null ) throw new CKException( "The IKeyboard implementation should be IStructuredSerializable" );

            //    using( FileStream str = new FileStream( KeyboardBackup.BackUpFilePath, FileMode.Open ) )
            //    {
            //        using( IStructuredReader reader = SimpleStructuredReader.CreateReader( str, Context.ServiceContainer ) )
            //        {
            //            _sharedDictionary.RegisterReader( reader, CK.SharedDic.MergeMode.ReplaceExisting );
            //            //Erasing all properties of the keyboard. We re-apply the backedup ones.
            //            serializableKeyboard.ReadContent( reader );
            //        }
            //    }
            //}
            
            //After cancelling modifications, we have no backup left.
            EnsureBackupIsClean();
        }

        /// <summary>
        /// Deletes the backup file if it exists
        /// sets <see cref="KeyboardBackup"/> to null
        /// </summary>
        public void EnsureBackupIsClean()
        {
            if( KeyboardBackup != null && File.Exists( KeyboardBackup.BackUpFilePath ) )
            {
                File.Delete( KeyboardBackup.BackUpFilePath );
            }
            
            KeyboardBackup = null;
        }

        
    }
}

