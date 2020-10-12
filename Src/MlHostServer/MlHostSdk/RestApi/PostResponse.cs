using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Toolbox.Tools;

namespace MlHostSdk.RestApi
{
    public class PostResponse<T> where T : class
    {
        public PostResponse(HttpStatusCode statusCode, T? value, string content)
        {
            Starting = false;
            StatusCode = statusCode;
            Value = value;
            Content = content.VerifyNotEmpty(nameof(content));
        }

        public PostResponse(HttpStatusCode statusCode, bool starting, string content)
        {
            StatusCode = statusCode;
            Starting = starting;
            Content = content.VerifyNotEmpty(nameof(content));
        }

        public string Content { get; }
        public bool Starting { get; }
        public HttpStatusCode StatusCode { get; }
        public T? Value { get; }
    }
}
