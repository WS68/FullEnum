namespace RunAlgorithm.Core.Runtime;

sealed class ExecItem : IExecItem
{
    private readonly IRunnerHost _host;
    private readonly IContext _context;
    private readonly IPathStep _path;
    private readonly IList<RunActorStep> _actors;

    public ExecItem(IRunnerHost host, IContext context, IPathStep path, IList<RunActorStep> actors)
    {
        _host = host;
        _context = context;
        _path = path;
        _actors = actors;
    }

    public void Execute(IExecutor executor)
    {
        Runner.ExecuteStep( _host, _context, _path, _actors, executor );
    }
}