namespace Toolbox.Services
{
    public interface ISecretFilter
    {
        string? FilterSecrets(string? data, string replaceSecretWith = "***");
    }
}