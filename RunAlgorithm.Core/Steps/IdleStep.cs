using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps
{
    public class IdleStep : IStep
    {
        public IdleStep()
        {
        }


        public string Name => "";

        public bool Execute(IContext context)
        {
            return true;
        }

        public bool IsList => false;
        public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
    }
}