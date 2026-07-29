using RunAlgorithm.Core;

namespace SyncRootFolders;

public sealed class Validator : IValidator
    {
    public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
        {
        var ctx = (Context)context;

        if (ctx.ReadNeedCheckImplicit(1) || ctx.ReadNeedCheckImplicit(2))
            return CheckResult.SuccessNegative;

        if (ctx.ReadIsRootImplicit() != ctx.ReadInContentImplicit())
            return CheckResult.SuccessPositive;

        return CheckResult.Failure;
        }
    }
