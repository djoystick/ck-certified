#region LGPL License
/*----------------------------------------------------------------------------
* This file (Plugins\Advanced\ObjectExplorer\ViewModels\VMICoreElement.cs) is part of CiviKey. 
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
using CK.Plugin;
using System.Reflection;

namespace CK.Plugins.ObjectExplorer
{
    public class VMICoreElement : VMISelectableElement
    {
        public bool OnError { get; protected set; }

        public AssemblyName Assembly { get; protected set; }

        public string AssemblyFullName { get { return Assembly != null ? Assembly.FullName : String.Empty; } }

        public string AssemblyName { get { return Assembly != null ? Assembly.Name : String.Empty; } }

        public Version AssemblyVersion { get { return Assembly != null ? Assembly.Version : new Version(); } }

        public string AssemblyPath { get { return Assembly != null ? Assembly.CodeBase : String.Empty; } }

        public virtual object Data { get { return this; } }

        public VMICoreElement( VMIContextViewModel ctx, VMIBase parent )
            : base( ctx, parent )
        {
        }
    }
}
