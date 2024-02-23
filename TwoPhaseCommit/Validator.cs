using RunAlgorithm.Core;

namespace TwoPhaseCommit;

public class Validator : IValidator
{
    public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
    {
        var ctx = (Context) context;
        if (ctx.Documents > 1)
        {
            //Это нарушение бизнеса
            if ( results.All( r => r != RunResult.Failed  ) )
                return CheckResult.Failure;
        }

        if (ctx.Documents == 0 )
        {
            //а это просто прикол какой-то
            if (results.All(r => r != RunResult.Failed))
                return CheckResult.Failure;
        }

        var ok = results.Count( r => r == RunResult.RunToEnd );
        if ( ok > 1  )
            return CheckResult.Failure;

        if (ok == 1)
            return CheckResult.SuccessPositive;

        return CheckResult.SuccessNegative;
    }
}