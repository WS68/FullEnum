namespace RunAlgorithm.Core;

public interface IValidator
{
    CheckResult EvaluateResult(IReadOnlyList<RunResult> results);
}