namespace RunAlgorithm.Core.Runtime;

internal class ExecPath : IPathStep
{
    private readonly string[] _initString;

    public ExecPath(string[] initString)
    {
        _initString = initString;
    }

    public IPathStep Parent => null;
}