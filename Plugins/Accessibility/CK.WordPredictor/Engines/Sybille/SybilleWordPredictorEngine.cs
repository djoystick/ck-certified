﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.WordPredictor.Model;
using Sybille = WordPredictor;

namespace CK.WordPredictor.Engines
{
    public class SybilleWordPredictorEngine : IWordPredictorEngine, IDisposable
    {
        Sybille.WordPredictor _sybille;

        public SybilleWordPredictorEngine( string languageFileName, string userLanguageFileName, string userTextsFileName )
        {
            _sybille = new Sybille.WordPredictor( languageFileName, userLanguageFileName, userTextsFileName );
        }

        public SybilleWordPredictorEngine( string languageFileName, string userLanguageFileName, string userTextsFileName, string semMatrix, string semWords, string semLambdas )
        {
            _sybille = new Sybille.WordPredictor( languageFileName, userLanguageFileName, userTextsFileName, semMatrix, semWords, semLambdas );
            _sybille.SemanticPredictorOn = true;
        }

        public IEnumerable<IWordPredicted> Predict( ITextualContextService textualService, int maxSuggestedWords )
        {
            return _sybille
                .Predict( String.Join( " ", textualService.Tokens.Select( t => t.Value ).ToArray() ), maxSuggestedWords )
                .Select( t => new WeightlessWordPredicted( t ) );
        }

        public bool IsWeightedPrediction
        {
            get { return false; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if( _sybille != null )
            {
                // Do we need to save user prediction ?
                // _sybille.SaveUserPredictor();
            }
        }

        #endregion
    }
}