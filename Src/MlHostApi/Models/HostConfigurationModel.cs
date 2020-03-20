using MlHostApi.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostApi.Models
{
    public class HostConfigurationModel
    {
        public IList<HostAssignment>? HostAssignments { get; set; }

        public void Verify()
        {
            HostAssignments?.ForEach(x => x.Verify());
        }
    }
}
