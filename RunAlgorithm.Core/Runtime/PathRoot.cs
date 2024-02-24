using System.Text;

namespace RunAlgorithm.Core.Runtime;

internal class PathRoot : IPathStep
{
    public IPathStep? Parent => null;

    void IPathStep.WriteTo(StringBuilder sb)
    {
    }

    public override string ToString()
    {
        return "Root Path???";
    }
}