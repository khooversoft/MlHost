using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class FrontEndRestApi : RestApiBase<BatchRequest, BatchResponse>
    {
        public FrontEndRestApi(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public FrontEndRestApi(HttpClient httpClient, string baseAddress)
            : base(httpClient, baseAddress)
        {
        }
    }
}
