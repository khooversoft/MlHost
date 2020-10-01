using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Toolbox.Tools;

namespace MlHostSdk.Models
{
    public static class Extensions
    {
        public static bool IsValidRequest(this BatchRequest batchRequest)
        {
            return batchRequest != null &&
                batchRequest.Request.ToNullIfEmpty() != null &&
                batchRequest.Models != null &&
                batchRequest.Models.Count > 0;
        }

        public static bool IsValidRequest(this PredictRequest predictRequest)
        {
            return predictRequest != null &&
                (predictRequest.Request.ToNullIfEmpty() ?? predictRequest.Sentence.ToNullIfEmpty()) != null;
        }

        public static string GetRequest(this PredictRequest predictRequest)
        {
            predictRequest.VerifyNotNull(nameof(predictRequest));

            return (predictRequest.Request.ToNullIfEmpty() ?? predictRequest.Sentence)
                .ToNullIfEmpty().VerifyNotNull("Request or Sentence was not specified");
        }
    }
}
