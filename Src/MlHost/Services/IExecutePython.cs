using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IExecutePython
    {
        Task Run(string deploymentFolder);
    }
}