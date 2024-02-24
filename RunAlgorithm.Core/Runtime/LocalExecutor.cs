using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunAlgorithm.Core.Runtime
{
    internal abstract class AbstractExecutor: IExecutor
    {
        private IRunnerHost _owner;
        private readonly IList<IExecItem> _items = new List<IExecItem>();

        public AbstractExecutor(IRunnerHost owner)
        {
            _owner = owner;
        }

        public void PushItem(IExecItem item)
        {
            _items.Add( item );
        }

        public void StoreResults(RunStatistics results)
        {
            _owner.AddResults( results );
        }

        public void Execute()
        {
            while ( ExecuteStep() )
            {

            }
        }

        protected abstract bool ExecuteStep();

        protected void GetItems( out IList<IExecItem> output )
        {
            output = _items.ToArray();
            _items.Clear();
        }
    }
}
