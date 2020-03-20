using MlHostApi.Models;
using MlHostApi.Repository;
using MlHostApi.Services;
using MlHostApi.Tools;
using MlHostApi.Types;
using MlHostCli.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MlHostCli.Activity
{
    internal class ListModelActivity
    {
        private readonly IModelRepository _modelRepository;

        public ListModelActivity(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public async Task List(CancellationToken token)
        {
            HostConfigurationModel model = await _modelRepository.ReadConfiguration(token);

            const string fmt = "{0,-20} {1}";
            string line = new string('=', 20);

            Console.WriteLine(fmt, "Host name", "Model ID");
            Console.WriteLine(fmt, line, line);

            model.HostAssignments
                ?.ForEach(x => Console.WriteLine(string.Format(fmt, x.HostName, x.ModelId)));
        }
    }
}
