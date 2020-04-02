using MlHostApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Tools;

namespace MlHostApi.Models
{
    public static class HostConfigurationModelExtensions
    {
        public static IDictionary<string, ModelId> ToDictionary(this HostConfigurationModel subject) =>
            subject.HostAssignments.VerifyNotNull(nameof(subject.HostAssignments))
                .ToDictionary(x => x.HostName!, x => new ModelId(x.ModelId!), StringComparer.OrdinalIgnoreCase);

        public static HostConfigurationModel ToModel(this IDictionary<string, ModelId> subject) =>
            new HostConfigurationModel
            {
                HostAssignments = subject
                    .Select(x => new HostAssignment { HostName = x.Key, ModelId = x.Value })
                    .ToList(),
            };

        public static void Verify(this HostConfigurationModel hostConfigurationModel)
        {
            hostConfigurationModel.VerifyNotNull(nameof(hostConfigurationModel));

            hostConfigurationModel.HostAssignments?.ForEach(x => x.Verify());
        }

        public static void Verify(this HostAssignment hostAssignment)
        {
            hostAssignment.VerifyNotNull(nameof(hostAssignment));

            hostAssignment.HostName.VerifyNotEmpty($"{nameof(hostAssignment.HostName)} is required");
            hostAssignment.ModelId.VerifyNotEmpty($"{nameof(hostAssignment.ModelId)} is required");
        }
    }
}
