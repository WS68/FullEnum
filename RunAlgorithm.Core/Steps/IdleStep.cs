namespace RunAlgorithm.Core.Steps;

public sealed class IdleStep : IStep
{
    public string Name => "idle";

    public bool Execute(IContext context)
    {
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}