﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CK.WordPredictor.Model;

namespace CK.WordPredictor.Engines
{
    internal interface IWordPredictorEngineFactory
    {
        IWordPredictorEngine Create( string predictorName );

        void Release( IWordPredictorEngine engine );
    }

}