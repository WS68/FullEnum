using RunAlgorithm.Core;

namespace SyncRootFolders;

public sealed class Validator : IValidator
    {
    public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
        {
        var ctx = (Context)context;

        if (ctx.ReadNeedCheckImplicit())
            return CheckResult.SuccessNegative;

        if (ctx.ReadIsRootImplicit() != ctx.ReadInContentImplicit())
            return CheckResult.SuccessPositive;

        return ctx.ReadNeedCheckImplicit() ? CheckResult.SuccessNegative : CheckResult.Failure;
        }
    }
