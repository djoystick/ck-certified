﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.WordPredictor.Model;

namespace CK.WordPredictor
{
    internal class SimpleToken : IToken
    {
        public SimpleToken( string v )
        {
            Value = v;
        }
        public string Value { get; set; }
    }
        
}