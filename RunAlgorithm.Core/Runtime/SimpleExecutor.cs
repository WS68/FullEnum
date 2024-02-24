namespace RunAlgorithm.Core.Runtime;

class SimpleExecutor : AbstractExecutor
{
    public SimpleExecutor(IRunnerHost owner) : base(owner)
    {
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