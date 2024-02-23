using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace Tests.Utils
{
    internal class CountingContext: IContext
    {
        private int _count;

        public int Count => _count;

        public void Exec()
        {
            _count++;
        }

        public IContext Clone()
        {
            return this;
        }
    }
}
