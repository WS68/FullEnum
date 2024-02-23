using RunAlgorithm.Core;

namespace RunAlgorithm.Core.Steps;

public sealed class FailedStep : IStep
{
    public string Name => "Fail";

    public bool Execute(IContext context)
    {
        throw new ApplicationException("Stopped at FailStep");
    }

    public bool IsList => false;
    public IReadOnlyList<IStep> Variants => throw new NotImplementedException();
}