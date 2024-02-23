using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps;

public class FixStep : IStep
{
    private readonly string _name;

    public FixStep(string name)
    {
        _name = name;
    }

    public string Name => "Token_" + _name;
    public bool Execute(IContext context)
    {
        var ctx = (Context)context;
        ctx.FixToken(_name);
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}