namespace RunAlgorithm.Core;

public interface IActor
{
    public string Name { get; }

    IReadOnlyList<IStep> Steps { get; }
}

public interface IStep
{
    public string Name { get; }
    bool Execute( IContext context );
    bool IsList { get; }
    IReadOnlyList< IStep > Variants { get; }
}