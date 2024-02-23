using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RunAlgorithm.Core
{
    public class Runner
    {
        private readonly IEnumerable<IActor> _actors;
        private readonly IContext _context;
        private readonly ILogger<Runner> _logger;

        public Runner(IEnumerable<IActor> actors, IContext context, ILogger<Runner> logger)
        {
            _actors = actors;
            _context = context;
            _logger = logger;
        }

        public void Run()
        {
            IList<IActor> actors = _actors.ToList();
        }
    }
}
