namespace MlHost.Services
{
    internal interface ICleanupProcess
    {
        void KillAnyRunningProcesses();
    }
}