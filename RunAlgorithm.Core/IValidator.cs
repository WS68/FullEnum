namespace RunAlgorithm.Core;

public interface IValidator
{
    CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results);
}