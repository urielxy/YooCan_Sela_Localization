using Nager.AmazonProductAdvertising.Model;
using System;

namespace Yooocan.Logic.Amazon
{
    class AmazonApiException : Exception
    {
        public AmazonApiException(AmazonErrorResponse errorResponse) : 
            base($"Error received from amazon: code: {errorResponse.Error.Code}, message: {errorResponse.Error.Message}")
        {
        }
    }
}
