namespace MlHost.Application
{
    public interface IOption
    {
        string? ServiceUri { get; }

        string DeploymentFolder { get; set; }

        string? PackageFile { get; set; }
    }
}