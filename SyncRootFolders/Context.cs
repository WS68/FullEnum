using RunAlgorithm.Core;

namespace SyncRootFolders;

public sealed class Context : IContext
    {
    private int _currentTransaction;
    private readonly HashSet<int> _activeTransactions;
    private readonly HashSet<int> _completedTransactions;
    private readonly Dictionary<string, int> _actorTransactions;
    private readonly Dictionary<string, bool> _savedIsRoot;
    private readonly Dictionary<string, bool> _savedInContent;
    private readonly Dictionary<string, int> _readNeedCheckFlags;
    private MvccValue<bool> _isRoot;
    private MvccValue<bool> _inContent;
    private MvccValue<bool> _needCheck1;
    private MvccValue<bool> _needCheck2;

    public Context()
        {
        _activeTransactions = new();
        _completedTransactions = new() { 0 };
        _actorTransactions = new();
        _savedIsRoot = new();
        _savedInContent = new();
        _readNeedCheckFlags = new();
        _isRoot = new MvccValue<bool>(true);
        _inContent = new MvccValue<bool>(false);
        _needCheck1 = new MvccValue<bool>(false);
        _needCheck2 = new MvccValue<bool>(false);
        }

    private Context(Context source)
        {
        _currentTransaction = source._currentTransaction;
        _activeTransactions = new(source._activeTransactions);
        _completedTransactions = new(source._completedTransactions);
        _actorTransactions = new(source._actorTransactions);
        _savedIsRoot = new(source._savedIsRoot);
        _savedInContent = new(source._savedInContent);
        _readNeedCheckFlags = new(source._readNeedCheckFlags);
        _isRoot = source._isRoot.Clone();
        _inContent = source._inContent.Clone();
        _needCheck1 = source._needCheck1.Clone();
        _needCheck2 = source._needCheck2.Clone();
        }

    public int CurrentTransaction => _currentTransaction;
    public int GetTransaction(string actor)
        {
        if (_actorTransactions.ContainsKey(actor))
            throw new ApplicationException($"Actor {actor} already has an open transaction");
        var transaction = ++_currentTransaction;
        _activeTransactions.Add(transaction);
        _actorTransactions.Add(actor, transaction);
        return transaction;
        }
    public bool ReadIsRootImplicit() => _isRoot.Read(new HashSet<int>(_completedTransactions));
    public bool ReadInContentImplicit() => _inContent.Read(new HashSet<int>(_completedTransactions));
    public bool ReadNeedCheckImplicit() => ReadNeedCheckImplicit(1) || ReadNeedCheckImplicit(2);
    public bool ReadNeedCheckImplicit(int flag) => NeedCheckFor(flag).Read(new HashSet<int>(_completedTransactions));
    public void SaveReadNeedCheckFlag(string actor, int flag) => _readNeedCheckFlags[actor] = flag;
    public void SaveIsRoot(string actor, bool value) => _savedIsRoot[actor] = value;
    public bool SavedIsRoot(string actor) => _savedIsRoot.TryGetValue(actor, out var value) ? value : throw new ApplicationException($"Actor {actor} has not read isRoot");
    public bool ReadIsRoot(string actor) => _isRoot.Read(TransactionFor(actor));
    public bool ReadInContent(string actor) => _inContent.Read(TransactionFor(actor));
    public void SaveInContent(string actor, bool value) => _savedInContent[actor] = value;
    public bool SavedInContent(string actor) => _savedInContent.TryGetValue(actor, out var value) ? value : throw new ApplicationException($"Actor {actor} has not read inContent");
    public void WriteInContent(string actor, bool value) => _inContent.Write(TransactionFor(actor), value);
    public void WriteIsRoot(string actor, bool value) => _isRoot.Write(TransactionFor(actor), value);
    public void WriteNeedCheck(string actor, int flag, bool value) => NeedCheckFor(flag).Write(TransactionFor(actor), value);
    public void WriteNeedCheck(string actor, bool value) => WriteNeedCheck(actor, 1, value);
    public int ReadSavedNeedCheckFlag(string actor) => _readNeedCheckFlags.TryGetValue(actor, out var flag) ? flag : throw new ApplicationException($"Actor {actor} has not read needCheck");
    public void CloseTransaction(string actor)
        {
        var transaction = TransactionFor(actor);
        _actorTransactions.Remove(actor);
        _activeTransactions.Remove(transaction);
        _completedTransactions.Add(transaction);
        }
    public IContext Clone() => new Context(this);
    private int TransactionFor(string actor) => _actorTransactions.TryGetValue(actor, out var transaction) ? transaction : throw new ApplicationException($"Actor {actor} has no open transaction");
    private MvccValue<bool> NeedCheckFor(int flag) => flag switch
        {
            1 => _needCheck1,
            2 => _needCheck2,
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, "NeedCheck flag must be 1 or 2")
            };
    }
