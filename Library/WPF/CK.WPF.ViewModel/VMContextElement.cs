#region LGPL License
/*----------------------------------------------------------------------------
* This file (Library\WPF\CK.WPF.ViewModel\VMContextElement.cs) is part of CiviKey. 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CK.Keyboard.Model;
using CK.Plugin.Config;
namespace CK.WPF.ViewModel
{
    /// <summary>
    /// </summary>
    public abstract class VMContextElement<TC, TB, TZ, TK> : VMBase
        where TC : VMContext<TC, TB, TZ, TK>
        where TB : VMKeyboard<TC, TB, TZ, TK>
        where TZ : VMZone<TC, TB, TZ, TK>
        where TK : VMKey<TC, TB, TZ, TK>
    {
        TC _context;

        protected VMContextElement( TC context )
        {
            _context = context;
            if( _context.SkinConfiguration != null )
                _context.SkinConfiguration.ConfigChanged += new EventHandler<ConfigChangedEventArgs>( OnLayoutConfigChanged );
        }

        private IEnumerable<VMContextElement<TC, TB, TZ, TK>> GetParents()
        {
            VMContextElement<TC, TB, TZ, TK> elem = this;
            while( elem != null )
            {
                elem = elem.Parent;

                if( elem != null )
                    yield return elem;
            }
        }

        /// <summary>
        /// Gets the parents of the element.
        /// </summary>
        public IEnumerable<VMContextElement<TC, TB, TZ, TK>> Parents { get { return GetParents().Reverse(); } }

        /// <summary>
        /// Gets the parent of the object.
        /// </summary>
        public abstract VMContextElement<TC, TB, TZ, TK> Parent { get; }

        /// <summary>
        /// Gets the <see cref="VMContext"/> to which this element belongs.
        /// </summary>
        public TC Context { get { return _context; } }

        /// <summary>
        /// Internal method called by this <see cref="Context"/> only.
        /// </summary>
        internal void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }

        void OnLayoutConfigChanged( object sender, ConfigChangedEventArgs e )
        {
            if( e.MultiPluginId.Any( ( c ) => String.Compare( c.UniqueId.ToString(), "36C4764A-111C-45E4-83D6-E38FC1DF5979", true ) == 0 ) )
            {
                switch( e.Key )
                {
                    case "Background":
                    case "HoverBackground":
                    case "PressedBackground":
                    case "FontSize":
                    case "FontWeight":
                    case "FontSizes":
                    case "FontStyle":
                    case "TextDecorations":
                    case "FontColor":
                        OnPropertyChanged( "TextDecorationsAsBool" );
                        OnPropertyChanged( "FontWeightAsBool" );
                        OnPropertyChanged( "PressedBackground" );
                        OnPropertyChanged( "FontStyleAsBool" );
                        OnPropertyChanged( "HoverBackground" );
                        OnPropertyChanged( "LetterColor" );
                        OnPropertyChanged( "FontWeight" );
                        OnPropertyChanged( "Background" );
                        OnPropertyChanged( "FontSizes" );
                        OnPropertyChanged( "FontStyle" );
                        OnPropertyChanged( "FontSize" );
                        OnPropertyChanged( "FontSize" );
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the element to which (IKeyboard) element plugindatas should be attached
        /// </summary>
        public abstract IKeyboardElement LayoutElement { get; }

        #region Layout Edition elements

        VMCommand<string> _clearCmd;
        public VMCommand<string> ClearPropertyCmd { get { return _clearCmd == null ? _clearCmd = new VMCommand<string>( ClearProperty, CanClearProperty ) : _clearCmd; } }

        void ClearProperty( string propertyName )
        {
            string[] names = propertyName.Split( ',' );
            foreach( var pname in names ) _context.SkinConfiguration[LayoutElement].Remove( pname );
        }

        bool CanClearProperty( string propertyName )
        {
            string[] names = propertyName.Split( ',' );
            // We can clear property if the property owns directly a value.
            foreach( var pname in names ) if( _context.SkinConfiguration[LayoutElement][pname] != null ) return true;
            return false;
        }

        public Color Background
        {
            get { return LayoutElement.GetPropertyValue( _context.SkinConfiguration, "Background", Colors.White ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["Background"] = value;
            }
        }

        public Color HoverBackground
        {
            get { return LayoutElement.GetPropertyValue( _context.SkinConfiguration, "HoverBackground", Background ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["HoverBackground"] = value;
            }
        }

        public Color HighlightBackground
        {
            get { return LayoutElement.GetPropertyValue( _context.SkinConfiguration, "HighlightBackground", Background ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["HighlightBackground"] = value;
            }
        }

        public Color PressedBackground
        {
            get { return LayoutElement.GetPropertyValue( _context.SkinConfiguration, "PressedBackground", HoverBackground ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["PressedBackground"] = value;
            }
        }

        public Color LetterColor
        {
            get { return LayoutElement.GetPropertyValue( _context.SkinConfiguration, "LetterColor", Colors.Black ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["LetterColor"] = value;
            }
        }

        public FontStyle FontStyle
        {
            get { return LayoutElement.GetWrappedPropertyValue( _context.SkinConfiguration, "FontStyle", FontStyles.Normal ).Value; }
        }

        public FontWeight FontWeight
        {
            get { return LayoutElement.GetWrappedPropertyValue( _context.SkinConfiguration, "FontWeight", FontWeights.Normal ).Value; }
        }

        public TextDecorationCollection TextDecorations
        {
            get
            {
                return LayoutElement.GetWrappedPropertyValue<TextDecorationCollection>( _context.SkinConfiguration, "TextDecorations" ).Value;
            }
        }

        #region FontPoperties used for edition
        /// <summary>
        /// Gets whether the FontStyle of the Current LayoutKeyMode is <see cref="FontStyles.Italic"/>.
        /// Returns false otherwise.
        /// Used to bind the FontStyle to a boolean control ( for example, a <see cref="ToggleButton"/>)
        /// </summary>
        public bool FontStyleAsBool
        {
            get { return LayoutElement.GetWrappedPropertyValue( _context.Config, "FontStyle", FontStyles.Normal ).Value == FontStyles.Italic; }
            set
            {
                if( value ) _context.Config[LayoutElement]["FontStyle"] = FontStyles.Italic;
                else _context.Config[LayoutElement]["FontStyle"] = FontStyles.Normal;
            }
        }

        /// <summary>
        /// Gets whether the FontWeight of the Current LayoutKeyMode is <see cref="FontWeights.Bold"/>.
        /// Returns false otherwise.
        /// Used to bind the FontWeight to a boolean control ( for example, a <see cref="ToggleButton"/>)
        /// </summary>
        public bool FontWeightAsBool
        {
            get { return LayoutElement.GetWrappedPropertyValue( _context.Config, "FontWeight", FontWeights.Normal ).Value == FontWeights.Bold; }
            set
            {
                if( value ) _context.Config[LayoutElement]["FontWeight"] = FontWeights.Bold;
                else _context.Config[LayoutElement]["FontWeight"] = FontWeights.Normal;
            }
        }

        /// <summary>
        /// Gets whether the TextDecorationCollection of the Current LayoutKeyMode has elements.
        /// Returns false otherwise.
        /// Used to bind the TextDecorationCollection to a boolean control ( for example, a <see cref="ToggleButton"/>)
        /// </summary>
        public bool TextDecorationsAsBool
        {
            get
            {
                var val = LayoutElement.GetWrappedPropertyValue<TextDecorationCollection>( _context.Config, "TextDecorations" );
                return val.Value != null && val.Value.Count > 0;
            }
            set
            {
                if( value ) _context.Config[LayoutElement]["TextDecorations"] = TextDecorationCollectionConverter.ConvertFromString( "Underline" );
                else _context.Config[LayoutElement]["TextDecorations"] = TextDecorationCollectionConverter.ConvertFromString( "" );
            }
        }

        IEnumerable<double> _sizes;
        IEnumerable<double> GetSizes( int from, int to )
        {
            for( int i = from; i <= to; i++ ) yield return i;
        }
        public IEnumerable<double> FontSizes { get { return _sizes == null ? _sizes = GetSizes( 10, 30 ) : _sizes; } }


        #endregion

        public double FontSize
        {
            get { return LayoutElement.GetPropertyValue<double>( _context.SkinConfiguration, "FontSize", 15 ); }
            set
            {
                _context.SkinConfiguration[LayoutElement]["FontSize"] = value;
            }
        }

        #endregion


    }
}