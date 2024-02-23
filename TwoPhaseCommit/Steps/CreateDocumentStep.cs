using RunAlgorithm.Core;
using System.Xml.Linq;

namespace TwoPhaseCommit.Steps;

public class CreateDocumentStep : IStep
{
    public string Name => "Doc";
    public bool Execute(IContext context)
    {
        var ctx = (Context)context;
        ctx.CreateDocument();
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}