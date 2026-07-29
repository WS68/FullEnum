using RunAlgorithm.Core;

namespace SyncRootFolders;

public sealed class Validator : IValidator
{
    public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
    {
        var ctx = (Context)context;
        return ctx.ReadInContentImplicit() == ctx.ReadIsRootImplicit() ? CheckResult.Failure : CheckResult.SuccessPositive;
    }
}
