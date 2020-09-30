using MlFrontEnd.Application;
using MlHostSdk.Models;

namespace MlFrontEnd.Services
{
    public class ExecuteRequest
    {
        public ExecuteRequest(ModelRequest modelRequest, HostOption hostOption)
        {
            ModelRequest = modelRequest;
            HostOption = hostOption;
        }

        public ModelRequest ModelRequest { get; }
        public HostOption HostOption { get; }
    }
}
