using MlHostWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Models
{
    public abstract class RunContextBase : ICacheKey
    {
        public RunState RunState { get; protected set; }

        public string Message { get; protected set; }

        protected abstract void ClearState();

        public abstract string GetId();

        public void Start() => RunState = RunState.Startup;


        public void Clear()
        {
            RunState = RunState.Startup;
            Message = null;

            ClearState();
        }

        public void SetMessage(string message)
        {
            Clear();
            RunState = RunState.Message;
            Message = message;
        }

        public void SetError(string errorMessage)
        {
            Clear();
            RunState = RunState.Error;
            Message = errorMessage;
        }
    }
}
