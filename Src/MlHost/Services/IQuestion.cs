using MlHostApi.Models;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IQuestion
    {
        Task<AnswerResponse> Ask(QuestionRequest questionModel);
    }
}
