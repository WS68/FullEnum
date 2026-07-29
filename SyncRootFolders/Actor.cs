using RunAlgorithm.Core;
using SyncRootFolders.Steps;

namespace SyncRootFolders;

public sealed class Actor : IActor
    {
    public Actor(string name, int flag, bool inContent, bool isRoot)
        {
        Name = name;
        Steps = new IStep[] { new OpenTransactionStep(name), new ReadIsRootStep(name), new WriteInContentStep(name, inContent), new FixIsRootStep(name, flag, isRoot), new CloseTransactionStep(name) };
        }
    public Actor(string name, bool inContent, bool isRoot) : this(name, 1, inContent, isRoot) { }
    public string Name
        {
        get;
        }
    public IReadOnlyList<IStep> Steps
        {
        get;
        }
    }

public sealed class CheckActor : IActor
    {
    public CheckActor(string name = "Check")
        {
        Name = name;
        Steps = new IStep[]
            {
            new ReadNeedCheckStep(name),
            new OpenTransactionStep(name),
            new ReadTransactionValuesStep(name),
            new CorrectIsRootStep(name),
            new ResetNeedCheckStep(name),
            new CloseTransactionStep(name)
            };
        }
    public string Name
        {
        get;
        }
    public IReadOnlyList<IStep> Steps
        {
        get;
        }
    }
