using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps;

public class GetTokenStep : IStep
{
    private readonly string _name;

    public GetTokenStep(string name)
    {
        _name = name;
    }

    public string Name => "Token_" + _name;
    public bool Execute(IContext context)
    {
        var ctx = (Context)context;
        ctx.GetToken(_name);
        return true;
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}