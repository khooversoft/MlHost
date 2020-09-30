using System;
using System.Collections.Generic;
using System.Text;

namespace MlProcess.Models
{
    internal class ResponseResult
    {
        public int RequestId { get; set; }

        public string? Question { get; set; }

        public IReadOnlyList<ModelResult>? ModelResults { get; set; }

    }
}
