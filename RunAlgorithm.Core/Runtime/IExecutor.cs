using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunAlgorithm.Core.Runtime
{
    internal interface IExecutor
    {
        void PushItem(IExecItem item);
        void StoreResults(RunStatistics results);
    }
}
