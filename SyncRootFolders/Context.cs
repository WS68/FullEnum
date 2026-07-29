using RunAlgorithm.Core;

namespace SyncRootFolders;

public sealed class Context : IContext
{
    private int _currentTransaction;
    private readonly HashSet<int> _activeTransactions;
    private readonly HashSet<int> _completedTransactions;
    private readonly Dictionary<string, int> _actorTransactions;
    private readonly Dictionary<string, bool> _savedIsRoot;
    private MvccValue<bool> _isRoot;
    private MvccValue<bool> _inContent;

    public Context()
    {
        _activeTransactions = new(); _completedTransactions = new() { 0 };
        _actorTransactions = new(); _savedIsRoot = new();
        _isRoot = new MvccValue<bool>(true); _inContent = new MvccValue<bool>(false);
    }

    private Context(Context source)
    {
        _currentTransaction = source._currentTransaction;
        _activeTransactions = new(source._activeTransactions); _completedTransactions = new(source._completedTransactions);
        _actorTransactions = new(source._actorTransactions); _savedIsRoot = new(source._savedIsRoot);
        _isRoot = source._isRoot.Clone(); _inContent = source._inContent.Clone();
    }

    public int CurrentTransaction => _currentTransaction;
    public int GetTransaction(string actor)
    {
        if (_actorTransactions.ContainsKey(actor)) throw new ApplicationException($"Actor {actor} already has an open transaction");
        var transaction = ++_currentTransaction; _activeTransactions.Add(transaction); _actorTransactions.Add(actor, transaction); return transaction;
    }
    public bool ReadIsRootImplicit() => _isRoot.Read(new HashSet<int>(_completedTransactions));
    public bool ReadInContentImplicit() => _inContent.Read(new HashSet<int>(_completedTransactions));
    public void SaveIsRoot(string actor, bool value) => _savedIsRoot[actor] = value;
    public bool SavedIsRoot(string actor) => _savedIsRoot.TryGetValue(actor, out var value) ? value : throw new ApplicationException($"Actor {actor} has not read isRoot");
    public void WriteInContent(string actor, bool value) => _inContent.Write(TransactionFor(actor), value);
    public void WriteIsRoot(string actor, bool value) => _isRoot.Write(TransactionFor(actor), value);
    public void CloseTransaction(string actor)
    {
        var transaction = TransactionFor(actor); _actorTransactions.Remove(actor); _activeTransactions.Remove(transaction); _completedTransactions.Add(transaction);
    }
    public IContext Clone() => new Context(this);
    private int TransactionFor(string actor) => _actorTransactions.TryGetValue(actor, out var transaction) ? transaction : throw new ApplicationException($"Actor {actor} has no open transaction");
}
