#region LGPL License
/*----------------------------------------------------------------------------
* This file (Library\WPF\CK.WPF.Controls\Converters\BooleanToIsMinimizedConverter.cs) is part of CiviKey. 
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
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace CK.WPF.Controls
{
    [ValueConversion( typeof( bool ), typeof( WindowState ) )]
    public class BooleanToIsMinimizedConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( value != null )
            {
                bool booleanValue;
                if( bool.TryParse( value.ToString(), out booleanValue ) )
                {
                    return booleanValue ? WindowState.Minimized : WindowState.Normal;
                }
            }
            return WindowState.Normal;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if(value != null)
            {
                WindowState ws = WindowState.Normal;
                if( Enum.TryParse<WindowState>( value.ToString(), out ws ) )
                {
                    return ws == WindowState.Minimized;
                }
            }
            return false;
        }
    }
}
