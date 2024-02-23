using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps;

public class CreateDocumentStep : IStep
{
    public string Name => "Doc";
    public bool Execute(IContext context)
    {
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}