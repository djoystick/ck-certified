﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CK.Keyboard.Model;
using CK.Windows;
using CK.Windows.Config;
using ContextEditor.Resources;

namespace ContextEditor.ViewModels
{
    public class EndingStepViewModel : WizardPage
    {
        public IList<WizardButtonViewModel> Buttons { get; set; }
        IKeyboardEditorRoot _root;

        public override bool CheckCanGoFurther()
        {
            return Buttons.Any( ( b ) => b.IsSelected );
        }

        public EndingStepViewModel( IKeyboardEditorRoot root, WizardManager wizardManager )
            : base( wizardManager, true )
        {
            Buttons = new List<WizardButtonViewModel>();
            HideNext = true;
            HideBack = true;

            Buttons.Add( new WizardButtonViewModel( R.Quit, R.EndingStepQuitDesc, "pack://application:,,,/ContextEditor;component/Resources/Images/exit.png", CloseWizard ) );
            Buttons.Add( new WizardButtonViewModel( R.StartOver, R.EndingStepStartOverDesc, "pack://application:,,,/ContextEditor;component/Resources/Images/restart.png", RestartWizard ) );

            _root = root;
            Title = R.EndingStepTitle;
            Description = R.EndingStepDesc;
        }

        SimpleCommand<WizardButtonViewModel> _command;
        public SimpleCommand<WizardButtonViewModel> ButtonCommand
        {
            get
            {
                if( _command == null ) _command = new SimpleCommand<WizardButtonViewModel>( ( k ) =>
                {
                    k.IsSelected = true;
                } );
                return _command;
            }
        }

        public void CloseWizard()
        {
            _root.EnsureBackupIsClean();
            WizardManager.Close();
        }

        public void RestartWizard()
        {
            _root.EnsureBackupIsClean();
            WizardManager.Restart();
        }
    }
}