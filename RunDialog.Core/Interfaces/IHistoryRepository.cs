namespace RunDialog.Core.Interfaces;

/// <summary>
/// Repository Pattern: abstracts persistence of command history.
/// Interface Segregation: minimal surface area for history operations.
/// </summary>
public interface IHistoryRepository
{
    IReadOnlyList<string> GetHistory();
    void Add(string command);
    void Clear();
}
