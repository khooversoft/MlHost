using System.IO;
using System.Reflection;
using Toolbox.Tools;

namespace MlHost.Application
{
    internal class Option : IOption
    {
        public string ServiceUri { get; set; } = "http://localhost:5003/predict";

        public string DeploymentFolder { get; set; } = Assembly.GetExecutingAssembly().Location
                .Func(x => Path.GetDirectoryName(x)!)
                .Func(x => Path.Combine(x, "MlPackageDeploy"));
    }
}
