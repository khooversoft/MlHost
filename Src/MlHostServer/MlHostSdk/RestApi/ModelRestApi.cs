﻿using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class ModelRestApi : RestApiBase<PredictRequest, PredictResponse>
    {
        public ModelRestApi(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public ModelRestApi(HttpClient httpClient, string baseAddress)
            : base(httpClient, baseAddress)
        {
        }
    }
}
