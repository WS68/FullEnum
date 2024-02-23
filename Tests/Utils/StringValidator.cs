using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunAlgorithm.Core;

namespace Tests.Utils
{
    internal class StringValidator: IValidator
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly CheckResult _result;

        public StringValidator(CheckResult checkResult = CheckResult.SuccessPositive)
        {
            _result = checkResult;
        }

        public CheckResult EvaluateResult(IContext context, IReadOnlyList<RunResult> results)
        {
            foreach (var result in results)
            {
                switch (result)
                {
                    case RunResult.Running:
                        _builder.Append('R');
                        break;
                    case RunResult.RunToEnd:
                        _builder.Append('E');
                        break;
                    case RunResult.Stopped:
                        _builder.Append('S');
                        break;
                    case RunResult.Failed:
                        _builder.Append('F');
                        break;
                    default:
                        _builder.Append('_');
                        break;
                }
            }
            return _result;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
