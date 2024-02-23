using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace Tests.Utils
{
    internal class SmartValidator: IValidator
    {
        public SmartValidator()
        {
        }

        public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
        {
            foreach (var result in results)
            {
                switch (result)
                {
                    case RunResult.Stopped:
                        return CheckResult.SuccessNegative;
                    case RunResult.RunToEnd:
                        continue;
                    default:
                        return CheckResult.Failure;
                }
            }
            return CheckResult.SuccessPositive;
        }
    }
}
