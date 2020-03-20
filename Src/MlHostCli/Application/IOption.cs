namespace MlHostCli.Application
{
    internal interface IOption
    {
        bool Activate { get; }

        BlobStoreOption BlobStore { get; }

        bool Delete { get; }

        bool Download { get; }

        bool Force { get; }

        bool Help { get; }

        string HostName { get; }

        bool List { get; }

        string ModelName { get; }

        string SecretId { get; }

        bool Upload { get; }

        string VersionId { get; }

        string ZipFile { get; }
    }
}