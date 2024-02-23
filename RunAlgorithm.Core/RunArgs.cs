namespace RunAlgorithm.Core;

public class RunArgs: EventArgs
{
    public RunArgs(RunStatistics statistics)
    {
        Statistics = statistics;
    }

    public  RunStatistics Statistics { get; } 
}