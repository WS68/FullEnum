using System.Runtime.CompilerServices;
using RunAlgorithm.Core;
using TwoPhaseCommit.Steps;

namespace TwoPhaseCommit
{
    public class Actor: IActor
    {
        private IReadOnlyList<IStep> _steps;

        public Actor(string name)
        {
            Name = name;
            _steps = new IStep[]
            {
                new GetTokenStep( name ),
                new CreateDocumentStep(),
                new FixStep( name ),
            };
        }

        public string Name { get; }
        public IReadOnlyList<IStep> Steps => _steps;
    }
}