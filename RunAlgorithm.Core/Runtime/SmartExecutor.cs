namespace RunAlgorithm.Core.Runtime;

class SmartExecutor : AbstractExecutor
{
    public SmartExecutor(IRunnerHost owner) : base(owner)
    {
    }

    protected override bool ExecuteStep()
    {
        GetItems( out var items );
        if (items.Count == 0)
            return false;

        if (items.Count > Environment.ProcessorCount * 2)
        {
            List<Task> tasks = new List<Task>();

            //run in pool
            foreach (var item in items)
            {
                var wrapper = new AsyncWrapper( base.Owner, item );
                //item.Execute(this);

                tasks.Add( wrapper.Task );
            }

            Task.WaitAll(tasks.ToArray());
            return false;
        }

        foreach (var item in items)
        {
            item.Execute( this );
        }

        return true;
    }
}