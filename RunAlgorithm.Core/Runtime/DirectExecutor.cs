namespace RunAlgorithm.Core.Runtime;

class DirectExecutor : AbstractExecutor
{
    public DirectExecutor(IRunnerHost owner) : base(owner)
    {
    }

    public override void PushItem(IExecItem item)
    {
        item.Execute( this );
        PushStat();
    }

    protected override bool ExecuteStep()
    {
        GetItems( out var items );
        if (items.Count == 0)
            return false;

        foreach (var item in items)
        {
            item.Execute( this );
        }

        return true;
    }
}