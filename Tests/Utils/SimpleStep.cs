using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace Tests.Utils
{
    internal class SimpleStep: IStep
    {
        private bool _stepResult;

        public SimpleStep( bool result = true )
        {
            _stepResult = result;
        }

        public string Name => "Simple/" + _stepResult;
        public bool Execute(IContext context)
        {
            ((CountingContext)context).Exec();
            return _stepResult;
        }

        public bool IsList => false;
        public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
    }
}
