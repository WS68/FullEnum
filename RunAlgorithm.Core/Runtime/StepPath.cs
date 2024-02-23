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

    public IPathStep Parent => _parent;

    public int ActorIndex => _actorIndex;

    public string StepName => _name;
}