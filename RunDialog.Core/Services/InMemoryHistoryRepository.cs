using RunDialog.Core.Interfaces;

namespace RunDialog.Core.Services;

/// <summary>
/// In-memory implementation of history repository.
/// Can be replaced with file-backed or registry-backed version without changing consumers.
/// </summary>
public sealed class InMemoryHistoryRepository : IHistoryRepository
{
    private readonly List<string> _history = new();
    private readonly IRunDialogSettings _settings;

    public InMemoryHistoryRepository(IRunDialogSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();

    public void Add(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return;

        _history.Remove(command);
        _history.Insert(0, command);

        while (_history.Count > _settings.MaxHistoryItems)
        {
            _history.RemoveAt(_history.Count - 1);
        }
    }

    public void Clear() => _history.Clear();
}
