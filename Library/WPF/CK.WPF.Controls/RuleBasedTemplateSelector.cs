#region LGPL License
/*----------------------------------------------------------------------------
* This file (Library\WPF\CK.WPF.Controls\RuleBasedTemplateSelector.cs) is part of CiviKey. 
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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Security;
using System.Globalization;
using Common.Logging;

namespace CK.WPF.Controls
{
    [ContentProperty( "Template" )]
    public class RuleBasedTemplateSelectorRule
    {
        public DataTemplate Template { get; set; }

        public virtual bool Match( object item, Type itemType, DependencyObject container )
        {
            return true;
        }
    }

    public class SelectorByTypeRule : RuleBasedTemplateSelectorRule
    {
        public Type Type { get; set; }

        [DefaultValue(false)]
        public bool ExactTypeMatch { get; set; }

        public override bool Match( object item, Type itemType, DependencyObject container )
        {
            bool success = false;
            if( Type != null )
            {
                if( ExactTypeMatch )
                {
                    success = Type.IsAssignableFrom( itemType );
                    //RuleBasedTemplateSelector.Log.Debug( log => log( "Exact type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
                else
                {
                    success = CK.Reflection.ReflectionHelper.CovariantMatch( Type, itemType );
                    //RuleBasedTemplateSelector.Log.Debug( log => log( "Covariant type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
            }
            //else RuleBasedTemplateSelector.Log.Debug( log => log( "Unitialized rule (no Type nor TypeDescriptor set). Rule failed." ) );
            return success;
        }
    }

    [ContentProperty( "Rules" )]
    public class RuleBasedTemplateSelector : DataTemplateSelector
    {
        static readonly internal Common.Logging.ILog Log = Common.Logging.LogManager.GetLogger<RuleBasedTemplateSelector>();

        public RuleBasedTemplateSelector()
        {
            Rules = new List<RuleBasedTemplateSelectorRule>();
        }

        public List<RuleBasedTemplateSelectorRule> Rules { get; private set; }

        public override DataTemplate SelectTemplate( object item, DependencyObject container )
        {
            Type itemType = item.GetType();
            foreach( RuleBasedTemplateSelectorRule rule in Rules )
            {
                if( rule.Match( item, itemType, container ) ) return rule.Template;
            }
            return base.SelectTemplate( item, container );
        }
    }
}
