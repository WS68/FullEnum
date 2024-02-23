using RunAlgorithm.Core;

namespace TwoPhaseCommit;

public class Validator : IValidator
{
    public CheckResult EvaluateResult(IReadOnlyList<RunResult> results)
    {
        return CheckResult.Failure;
    }
}