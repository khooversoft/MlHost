namespace MlHost.Services
{
    internal interface IJson
    {
        T Deserialize<T>(string subject);

        string Serialize<T>(T subject);
    }
}