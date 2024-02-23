using RunAlgorithm.Core;

namespace TwoPhaseCommit;

public class Validator : IValidator
{
    public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
    {
        var ok = results.Count( r => r == RunResult.RunToEnd );
        if ( ok > 1  )
            return CheckResult.Failure;

        if (ok == 1)
            return CheckResult.SuccessPositive;

        return CheckResult.SuccessNegative;
    }
}