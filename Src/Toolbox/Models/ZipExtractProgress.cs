using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Models
{
    public struct ZipExtractProgress
    {
        public ZipExtractProgress(int total, int count)
        {
            Total = total;
            Count = count;
        }

        public int Total { get; }
        public int Count { get; }
    }
}
