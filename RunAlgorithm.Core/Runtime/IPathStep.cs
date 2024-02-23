namespace RunAlgorithm.Core.Runtime;

internal interface IPathStep
{
    IPathStep Parent { get; }
}