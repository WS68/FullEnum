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
    public class Runner: IRunnerHost
    {
        private const int ProgressItemCount = 100000;
        private readonly IEnumerable<IActor> _actors;
        private readonly IContext _context;
        private readonly IValidator _validator;
        private readonly ILogger<Runner> _logger;

        private long _successOk;
        private long _successBad;
        private long _failed;

        private long _totals;
        private long _nextFire = ProgressItemCount;

        private readonly object _lockStat = new object();

        public EventHandler<RunArgs> Progress;

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

        public RunStatistics Run()
        {
            IList<IActor> actors = _actors.ToList();

            var path = new PathRoot();


            List<RunActorStep> steps = new List<RunActorStep>();
            foreach (var actor in actors)
            {
                var step = new RunActorStep( actor, -1, RunResult.Running );
                steps.Add(step );
            }

            var executor = new SmartExecutor(this);
            ExecuteStep(this, _context, path, steps, executor );
            executor.Execute();

            return new RunStatistics(_successOk, _successBad, _failed);
        }

        internal static void ExecuteStep(IRunnerHost host, 
            IContext context, 
            IPathStep path, 
            IList<RunActorStep> actors,
            IExecutor executor )
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
                        host.Logger.LogDebug("Failed at {0} ctx=[{1}]: {2}", spath, ctx, e.Message);
                        result = RunResult.Failed;
                    }
                    catch (Exception e)
                    {
                        host.Logger.LogInformation( e, "Failed at {0} ctx=[{1}]", spath, ctx);
                        result = RunResult.Failed;
                    }

                    var ss = new RunActorStep(actor.Actor, aindex, result);
                    var nactors = new List<RunActorStep>( actors );
                    nactors[index] = ss;

                    var item = new ExecItem( host, ctx, spath, nactors );  
                    //ExecuteStep(host, ctx, spath, nactors);

                    executor.PushItem( item );
                }
            }

            if (!wasExecute)
            {
                long successOk = 0;
                long successBad = 0;
                long failed = 0;

                var results = actors.Select(s => s.FinalResult).ToArray();
                var result = host.Validator.EvaluateResult(context, results);
                if (result == CheckResult.Failure)
                {
                    host.Logger.LogWarning( $"Failed Validation: {path}" );
                    failed++;
                    //CheckFire();
                }
                else if (result == CheckResult.SuccessNegative)
                {
                    successBad++;
                    //CheckFire();
                }
                else if ( result == CheckResult.SuccessPositive )
                {
                    successOk++;
                    //CheckFire();
                }
                else
                {
                    host.Logger.LogError($"Unxepected result {result} as {path}");
                }

                executor.StoreResults( new RunStatistics( successOk, successBad, failed  ) );
            }
        }

        private void Fire()
        {
            var stat = new RunStatistics(_successOk, _successBad, _failed);
            Progress?.Invoke( this, new RunArgs( stat ) );
        }

        IValidator IRunnerHost.Validator => _validator;
        ILogger IRunnerHost.Logger => _logger;

        void IRunnerHost.AddResults(RunStatistics results)
        {
            lock (_lockStat)
            {
                _failed += results.Failures;
                _successOk += results.PositiveResults;
                _successBad += results.NegativeResults;

                _totals += results.Total;
                if (_totals >= _nextFire)
                {
                    _nextFire += ProgressItemCount;
                    Fire();
                }
            }
        }
    }
}
