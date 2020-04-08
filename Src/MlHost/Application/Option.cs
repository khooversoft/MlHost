using System.IO;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string ServiceUri { get; set; } = "http://localhost:5003/predict";

        public string DeploymentFolder { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }
}
