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

        private long _successOk;
        private long _successBad;
        private long _failed;

        public AbstractExecutor(IRunnerHost owner)
        {
            _owner = owner;
        }

        public IRunnerHost Owner => _owner;

        public virtual void PushItem(IExecItem item)
        {
            _items.Add( item );
        }

        public void StoreResults(RunStatistics results)
        {
            _failed += results.Failures;
            _successOk += results.PositiveResults;
            _successBad += results.NegativeResults;

            //_owner.AddResults( results );
        }

        public void Execute()
        {
            while ( ExecuteStep() )
            {
                PushStat();
            }
            PushStat();
        }

        protected void PushStat()
        {
            if ( _successBad + _successOk + _failed == 0 )
                return;

            var results = new RunStatistics( _successOk, _successBad, _failed );
            _successOk = _successBad = _failed = 0;
            _owner.AddResults( results );
        }

        protected abstract bool ExecuteStep();

        protected void GetItems( out IList<IExecItem> output )
        {
            output = _items.ToArray();
            _items.Clear();
        }
    }
}
