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

        private int successOk;
        private int successBad;
        private int failed;

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

            if (failed > 0)
            {
                _logger.LogWarning($"RESULT: OK={successOk}, Negative={successBad}, Failures={failed}");
            }
            else
            {
                _logger.LogInformation($"RESULT: OK={successOk}, Negative={successBad}, Failures={failed}");
            }
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
                        if (!step.Execute(ctx))
                        {
                            result = RunResult.Stopped;
                        }
                    }
                    catch (ApplicationException e)
                    {
                        _logger.LogInformation($"Failed at {spath} ctx=[{ctx}]: {e.Message}");
                        result = RunResult.Failed;
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation( e, $"Failed at {spath} ctx=[{ctx}]" );
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
                    _logger.LogWarning( $"Failed Validation: {path}" );
                    Interlocked.Increment(ref failed);
                }
                else if (result == CheckResult.SuccessNegative)
                {
                    Interlocked.Increment(ref successBad);
                }
                else if ( result == CheckResult.SuccessPositive )
                {
                    Interlocked.Increment(ref successOk);
                }
                else
                {
                    _logger.LogError($"Unxepected result {result} as {path}");
                }
            }
        }
    }
}
