namespace RunAlgorithm.Core.Runtime;

internal interface IExecItem
{
    void Execute(IExecutor executor);
}