using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace Tests.Utils
{
    internal class SimpleActor: IActor
    {
        public SimpleActor(params  IStep[] steps)
        {
            Assert.AreNotEqual<int>(0, steps.Length );
            Steps = steps;
        }

        public string Name => "Simple Actor";
        public IReadOnlyList<IStep> Steps { get; }
    }
}
