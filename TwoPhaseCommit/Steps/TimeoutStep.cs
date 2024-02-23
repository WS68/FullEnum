using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps;

public class TimeoutStep : IStep
{
    private readonly string _name;

    public TimeoutStep(string name)
    {
        _name = name;
    }

    public string Name => "Timeout_" + _name;
    public bool Execute(IContext context)
    {
        var ctx = (Context)context;
        ctx.ResetToken(_name);
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}