﻿using System;
using System.Collections;
using System.Collections.Generic;
using CK.Context;
using CK.Core;
using CK.Keyboard.Model;
using CK.Plugin;
using CommonServices;

namespace CK.StandardPlugins.CommandManager
{
    [Plugin("{7CCD8104-FB8A-4495-8855-3E7B56EE5100}",
        Categories=new string[]{"Advanced"},
        Version="1.0.1",
        PublicName="CommandManager")]
    public class CommandManager : IPlugin, ICommandManagerService
    {
        Queue<DictionaryEntry> _runningCommands;

        public event EventHandler<CommandSendingEventArgs>  CommandSending;
        public event EventHandler<CommandSentEventArgs>  CommandSent;

        [DynamicService(Requires=RunningRequirement.MustExistAndRun)]
        public IKeyboardContext KeyboardContext { get; set; }

        [RequiredService]
        public IContext Context { get; set; }

        public bool Setup( IPluginSetupInfo info )
        {
            return true;
        }

        public void Start()
        {            
            KeyboardContext.CurrentKeyboardChanged += new EventHandler<CurrentKeyboardChangedEventArgs>( CurrentKeyboardChanged );
            if(KeyboardContext.CurrentKeyboard != null)
                RegistersOnKeyboard( KeyboardContext.CurrentKeyboard );
        }

        public void Stop()
        {
            KeyboardContext.CurrentKeyboardChanged -= new EventHandler<CurrentKeyboardChangedEventArgs>( CurrentKeyboardChanged );
            if( KeyboardContext.CurrentKeyboard != null )
                UnRegistersOnKeyboard( KeyboardContext.CurrentKeyboard );
        }

        public void Teardown() { }

        public bool IsRunningCommands
        {
            get { throw new NotImplementedException(); }
        }

        public void SendCommand( object sender, string command )
        {
            if( _runningCommands == null ) _runningCommands = new Queue<DictionaryEntry>();
            bool isRunning = _runningCommands.Count > 0;

            _runningCommands.Enqueue( new DictionaryEntry( sender, command.Trim() ) );

            if( !isRunning )
            {
                while( _runningCommands.Count > 0 )
                {
                    DictionaryEntry e = _runningCommands.Dequeue();
                    DoSendCommand( e.Key, (string)e.Value );
                }
            }
        }

        public void SendCommands( object sender, IReadOnlyList<string> commands )
        {
            if( _runningCommands == null ) _runningCommands = new Queue<DictionaryEntry>();
            bool isRunning = _runningCommands.Count > 0;

            foreach( string cmd in commands )
                _runningCommands.Enqueue( new DictionaryEntry( sender, cmd.Trim() ) );

            if( !isRunning )
            {
                while( _runningCommands.Count > 0 )
                {
                    DictionaryEntry e = _runningCommands.Dequeue();
                    DoSendCommand( e.Key, (string)e.Value );
                }
            }
        }

        /// <summary>
        /// Called when the current keyboard changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentKeyboardChanged( object sender, CurrentKeyboardChangedEventArgs e )
        {
            if( e.Previous != null )
                UnRegistersOnKeyboard( e.Previous );
            if( e.Current != null )
                RegistersOnKeyboard( e.Current );
        }

        /// <summary>
        /// Registers on the Keyboard given in parameters.
        /// </summary>
        /// <param name="keyboard"><see cref="VKeyboard"/> to register on.</param>
        void RegistersOnKeyboard( IKeyboard keyboard )
        {
            keyboard.KeyDown += new EventHandler<KeyInteractionEventArgs>( SendKeyCommand );
            keyboard.KeyPressed += new EventHandler<KeyPressedEventArgs>( SendKeyCommand );
            keyboard.KeyUp += new EventHandler<KeyInteractionEventArgs>( SendKeyCommand );
        }

        /// <summary>
        /// Unregisters on the Keyboard given in parameters.
        /// </summary>
        /// <param name="keyboard"><see cref="VKeyboard"/> to unregister on.</param>
        void UnRegistersOnKeyboard( IKeyboard keyboard )
        {
            keyboard.KeyDown -= new EventHandler<KeyInteractionEventArgs>( SendKeyCommand );
            keyboard.KeyPressed -= new EventHandler<KeyPressedEventArgs>( SendKeyCommand );
            keyboard.KeyUp -= new EventHandler<KeyInteractionEventArgs>( SendKeyCommand );
        }

        /// <summary>
        /// Redirects the command to the method which will process it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SendKeyCommand( object sender, KeyInteractionEventArgs e )
        {
            SendCommands( e.Key, e.Commands );
        }

        // Private method which realy Send commands, we don't allow client to use it directly.
        void DoSendCommand( object sender, string cmd )
        {
            CommandSendingEventArgs e = new CommandSendingEventArgs( cmd );
            if( CommandSending != null ) CommandSending( sender, e );
            if( CommandSent != null ) CommandSent( sender, new CommandSentEventArgs( e ) );
        }
    }
}