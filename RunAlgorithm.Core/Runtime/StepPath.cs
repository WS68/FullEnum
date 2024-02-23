using System.Text;

namespace RunAlgorithm.Core.Runtime;

internal class StepPath : IPathStep
{
    private readonly IPathStep _parent;
    private readonly int _actorIndex;
    private readonly string _name;

    public StepPath(IPathStep parent, int actorIndex, string name)
    {
        _parent = parent;
        _actorIndex = actorIndex;
        _name = name;
    }

    public IPathStep? Parent => _parent;
    public void WriteTo(StringBuilder sb)
    {
        sb.Append(ActorIndex);
        sb.Append(':');
        sb.Append(StepName);
    }

    public int ActorIndex => _actorIndex;

    public string StepName => _name;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        List<IPathStep> parents = new List<IPathStep>
        {
            this
        };

        var parent = this.Parent;
        while (parent != null )
        {
            parents.Add( parent );
            parent = parent.Parent;
        }

        parents.Reverse();
        foreach (var pathStep in parents)
        {
            if (sb.Length > 0)
                sb.Append(',');

            pathStep.WriteTo( sb );
        }

        return sb.ToString();
    }
}