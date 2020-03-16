using MlHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public interface IQuestion
    {
        Task<AnswerModel> Ask(QuestionModel questionModel);
    }
}
