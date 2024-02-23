using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RunAlgorithm.Core.Runtime;

namespace RunAlgorithm.Core
{
    public class Runner
    {
        private readonly IEnumerable<IActor> _actors;
        private readonly IContext _context;
        private readonly IValidator _validator;
        private readonly ILogger<Runner> _logger;

        public Runner(IEnumerable<IActor> actors, 
            IContext context, 
            IValidator validator,
            ILogger<Runner> logger)
        {
            _actors = actors;
            _context = context;
            _validator = validator;
            _logger = logger;
        }

        public void Run()
        {
            IList<IActor> actors = _actors.ToList();

            var path = new ExecPath( actors.Select( a => a.Name ).ToArray() );


            List<RunActorStep> steps = new List<RunActorStep>();
            foreach (var actor in actors)
            {
                var step = new RunActorStep( actor, -1, RunResult.Running );
                steps.Add(step );
            }

            ExecuteStep( _context, path, steps );
        }

        private void ExecuteStep(IContext context, IPathStep path, List<RunActorStep> actors)
        {
            bool wasExecute = false;
            for (var index = 0; index < actors.Count; index++)
            {
                var actor = actors[index];
                if ( actor.Stopped )
                    continue;

                int aindex = actor.Index + 1;
                foreach (var step in actor.GetNextSteps())
                {
                    wasExecute = true;

                    var spath = new StepPath( path, index, step.Name );

                    var result = RunResult.Running;
                    var ctx = context.Clone();
                    try
                    {
                        if (!step.Execute(context))
                        {
                            result = RunResult.Stopped;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning( e, $"Failed at {spath}" );
                        result = RunResult.Failed;
                    }

                    var ss = new RunActorStep(actor.Actor, aindex, result);
                    var nactors = new List<RunActorStep>( actors );
                    nactors[index] = ss;

                    ExecuteStep( ctx, spath, nactors );
                }
            }

            if (!wasExecute)
            {
                var results = actors.Select(s => s.FinalResult).ToArray();
                var result = _validator.EvaluateResult(results);
                if (result == CheckResult.Failure)
                {
                    _logger.LogWarning( $"Failed: {path}" );
                }
            }
        }
    }

    public class RunActorStep
    {
        private readonly IActor _actor;
        private readonly int _index;
        private readonly RunResult _result;

        public RunActorStep(IActor actor, int index, RunResult result )
        {
            _actor = actor;
            _index = index;
            _result = result;

            if (index >= _actor.Steps.Count)
                _result = RunResult.RunToEnd;
        }

        public bool Stopped => _result != RunResult.Running;

        public IActor Actor => _actor;

        public int Index => _index;
        public RunResult FinalResult =>
            _result == RunResult.Running ? 
                RunResult.RunToEnd : _result;

        public IEnumerable<IStep> GetNextSteps()
        {
            var step = _actor.Steps[_index + 1];
            if (step.IsList)
            {
                return step.Variants;
            }

            return new[] {step};
        }
    }
}
