using MlHostSdk.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MlHostSdk.Package
{
    public class BuildResults
    {
        internal BuildResults(MlPackageSpec option, int fileCount)
        {
            Option = option;
            FileCount = fileCount;
        }

        public MlPackageSpec Option { get; }
        public int FileCount { get; }
    }
}
