namespace RunAlgorithm.Core.Runtime;

internal class RunActorStep
{
    private readonly IActor _actor;
    private readonly int _index;
    private readonly RunResult _result;

    public RunActorStep(IActor actor, int index, RunResult result)
    {
        _actor = actor;
        _index = index;
        _result = result;

        if (index >= _actor.Steps.Count)
            _result = RunResult.RunToEnd;
    }

    public bool Stopped => _result != RunResult.Running || _index >= _actor.Steps.Count - 1;

    public IActor Actor => _actor;

    public int Index => _index;
    public RunResult FinalResult =>
        _result == RunResult.Running ?
            RunResult.RunToEnd : _result;

    public IEnumerable<IStep> GetNextSteps()
    {
        var step = _actor.Steps[_index + 1];
        if (step.IsList)
        {
            return step.Variants;
        }

        return new[] { step };
    }
}