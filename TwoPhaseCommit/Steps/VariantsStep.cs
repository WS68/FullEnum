using RunAlgorithm.Core;

namespace TwoPhaseCommit.Steps;

internal class VariantsStep : IStep
{
    private readonly IStep[] _step;

    public VariantsStep(params IStep[] step)
    {
        if (step == null || step.Length == 0)
            throw new ArgumentNullException(nameof(step));

        if ( step.Any( s => s.IsList ))
            throw new ArgumentException("Nested compound steps are not allowed");

        _step = step;
    }

    public string Name => "Variants";
    public bool Execute(IContext context)
    {
        throw new NotImplementedException();
    }

    public bool IsList => true;
    public IReadOnlyList<IStep> Variants => _step;
}