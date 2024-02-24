namespace RunAlgorithm.Core.Runtime;

internal sealed class AsyncWrapper
{
    private readonly IRunnerHost _owner;
    private readonly IExecItem _item;
    private readonly TaskCompletionSource _task = new TaskCompletionSource();

    public AsyncWrapper(IRunnerHost owner, IExecItem item)
    {
        _owner = owner;
        _item = item;

        ThreadPool.QueueUserWorkItem(Exec);
    }

    private void Exec(object? state)
    {
        try
        {
            var executor = new DirectExecutor(_owner);
            executor.PushItem(_item);
            executor.Execute();
        }
        finally
        {
            _task.TrySetResult();
        }
    }

    public Task Task => _task.Task;
}