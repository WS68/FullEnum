using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RunAlgorithm.Core.Runtime
{
    internal interface IRunnerHost
    {
        IValidator Validator { get; }
        ILogger Logger { get; }
        void AddResults(RunStatistics results);
    }
}
