using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps
{
    public class FailedStep : IStep
    {
        public FailedStep()
        {
        }


        public string Name => "Fail";

        public bool Execute(IContext context)
        {
            throw new ApplicationException("Stopped at FailStep");
        }

        public bool IsList => false;
        public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
    }
}