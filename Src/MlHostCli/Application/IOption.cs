namespace MlHostCli.Application
{
    internal interface IOption
    {
        bool Help { get; }

        bool Upload { get; }
        bool Download { get; }
        bool Delete { get; }
        bool List { get; }
        bool Activate { get; }
        bool Deactivate { get; }

        bool Force { get; }

        string? ModelName { get; }
        string? VersionId { get; }

        string? HostName { get; }

        string? SecretId { get; }

        string? ZipFile { get; }

        BlobStoreOption? BlobStore { get; }
    }
}