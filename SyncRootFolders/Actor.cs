using RunAlgorithm.Core;
using SyncRootFolders.Steps;

namespace SyncRootFolders;

public sealed class Actor : IActor
{
    public Actor(string name, bool inContent, bool isRoot)
    {
        Name = name;
        Steps = new IStep[] { new ReadIsRootStep(name), new OpenTransactionStep(name), new WriteInContentStep(name, inContent), new FixIsRootStep(name, isRoot), new CloseTransactionStep(name) };
    }
    public string Name { get; }
    public IReadOnlyList<IStep> Steps { get; }
}
