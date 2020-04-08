using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IQuestion
    {
        Task<dynamic> Ask(dynamic request);
    }
}
