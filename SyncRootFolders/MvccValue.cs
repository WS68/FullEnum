namespace SyncRootFolders;

public sealed class MvccValue<T>
{
    private readonly List<(int Transaction, T Value)> _versions;

    public MvccValue(T initialValue) { _versions = new() { (0, initialValue) }; }
    private MvccValue(List<(int Transaction, T Value)> versions) { _versions = versions; }
    public void Write(int transaction, T value) => _versions.Add((transaction, value));
    public T Read(int transaction) => _versions.Where(v => v.Transaction <= transaction).OrderByDescending(v => v.Transaction).First().Value;
    public T Read(IReadOnlySet<int> completedTransactions) => _versions.Where(v => completedTransactions.Contains(v.Transaction)).OrderByDescending(v => v.Transaction).First().Value;
    public MvccValue<T> Clone() => new(new List<(int Transaction, T Value)>(_versions));
}
