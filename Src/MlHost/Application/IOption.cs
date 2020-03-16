namespace MlHost.Application
{
    public interface IOption
    {
        string? ServiceUri { get; }

        bool ForceDeployment { get; }
    }
}