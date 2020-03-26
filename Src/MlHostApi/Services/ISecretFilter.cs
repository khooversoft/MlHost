namespace MlHostApi.Services
{
    public interface ISecretFilter
    {
        string? FilterSecrets(string? data, string replaceSecretWith = "***");
    }
}