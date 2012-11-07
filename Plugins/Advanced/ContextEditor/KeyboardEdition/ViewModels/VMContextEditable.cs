#region LGPL License
/*----------------------------------------------------------------------------
* This file (Plugins\Accessibility\EditableSkin\ViewModels\VMContextEditable.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using CK.WPF.ViewModel;
using CK.Keyboard.Model;
using CK.Plugin.Config;
using CK.Core;
using CK.Context;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System;
using System.Globalization;

namespace ContextEditor.ViewModels
{
    public class VMContextEditable : VMContext<VMContextEditable, VMKeyboardEditable, VMZoneEditable, VMKeyEditable>
    {
        public VMContextEditable( IKeyboardEditorRoot root, IKeyboard keyboardToEdit )
            : base( root.Context, root.KeyboardContext.Service.Keyboards.Context )
        {
            _root = root;
            Model = root.KeyboardContext.Service;
            KeyboardVM = Obtain( keyboardToEdit );
        }

        IKeyboardEditorRoot _root;

        IPluginConfigAccessor _config;
        public IPluginConfigAccessor Config
        {
            get
            {
                if( _config == null ) _config = Context.GetService<IPluginConfigAccessor>( true );
                return _config;
            }
        }

        /// <summary>
        /// Gets the model linked to this ViewModel
        /// </summary>
        public IKeyboardContext Model { get; private set; }

        /// <summary>
        /// Gets the Skin plugin's configuration accessor
        /// </summary>
        public IPluginConfigAccessor SkinConfiguration { get { return _root.SkinConfiguration; } }

        VMContextElement<VMContextEditable, VMKeyboardEditable, VMZoneEditable, VMKeyEditable> _selectedElement;
        public VMContextElement<VMContextEditable, VMKeyboardEditable, VMZoneEditable, VMKeyEditable> SelectedElement
        {
            get { return _selectedElement; }
            set { _selectedElement = value; OnPropertyChanged( "SelectedElement" ); }
        }

        protected override VMKeyEditable CreateKey( IKey k )
        {
            return new VMKeyEditable( this, k );
        }

        protected override VMZoneEditable CreateZone( IZone z )
        {
            return new VMZoneEditable( this, z );
        }

        protected override VMKeyboardEditable CreateKeyboard( IKeyboard kb )
        {
            return new VMKeyboardEditable( this, kb );
        }

        protected override void OnCurrentKeyboardChanged( object sender, CurrentKeyboardChangedEventArgs e )
        {
            //Do nothing, we are not bound to the current keyboard of the keyboard context
        }

    }
}
