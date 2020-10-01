using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Services;

namespace Toolbox.Tools
{
    public static class HttpExtensions
    {
        public static async Task<T> GetContent<T>(this Task<HttpResponseMessage> response)
        {
            string responseString = (await (await response).Content.ReadAsStringAsync())
                .VerifyNotNull("No content");

            return Json.Default.Deserialize<T>(responseString);
        }
    }
}
