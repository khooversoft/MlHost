using MlHost.Models;
using MlHost.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHost.Application
{
    public class FakeQuestion : IQuestion
    {
        private const string _defaultAnswer = "Default answer";
        private const double _defaultScore = 0.2;

        public FakeQuestion() { }

        public static string DefaultAnswer => _defaultAnswer;

        public static double DefaultScore => _defaultScore;

        public Task<AnswerModel> Ask(QuestionModel questionModel)
        {
            _ = questionModel ?? throw new AggregateException(nameof(questionModel));

            var result = new AnswerModel
            {
                Answer = _defaultAnswer,
                Start = 0,
                End = 0,
                Score = _defaultScore,
            };

            return Task.FromResult(result);
        }
    }
}
