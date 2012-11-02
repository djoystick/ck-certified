﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.WordPredictor.Model;
using CK.Plugin;
using CommonServices;
using System.Text.RegularExpressions;
using BasicCommandHandlers;
using CK.Plugin.Config;

namespace CK.WordPredictor
{
    [Plugin( "{669622D4-4E7E-4CCE-96B1-6189DC5CD5D6}", PublicName = "WordPredictedService", Categories = new string[] { "Advanced" } )]
    public class WordPredictedService : BasicCommandHandler, IWordPredictedService
    {
        public const string CMDSendPredictedWord = "sendPredictedWord";

        public event EventHandler<WordPredictionSuccessfulEventArgs> WordPredictionSuccessful;

        public void WordHasBeenChosen( string word )
        {
            if( WordPredictionSuccessful != null )
                WordPredictionSuccessful( this, new WordPredictionSuccessfulEventArgs( word ) );
        }

        protected override void OnCommandSent( object sender, CommandSentEventArgs e )
        {
            CommandParser p = new CommandParser( e.Command );
            string str;
            if( p.IsIdentifier( out str ) && !e.Canceled && str == CMDSendPredictedWord )
            {
                p.GetNextToken();
                WordHasBeenChosen( p.StringValue );
            }
        }
    }
    //The WordPredicted Service is now a CommandHandler that handle sendPredictedWord command
   
}
