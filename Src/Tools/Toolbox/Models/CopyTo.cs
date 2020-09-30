using System.Diagnostics;

namespace Toolbox.Models
{
    [DebuggerDisplay("Source={Source}, Destination={Destination}")]
    public struct CopyTo
    {
        public string Source { get; set; }

        public string Destination { get; set; }
    }
}
