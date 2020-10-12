namespace FakeMlHost.Application
{
    public interface IOption
    {
        int Port { get; set; }
        string ModelName { get; }
    }
}