using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RunDialog.Core.Interfaces;
using RunDialog.Core.Services;

namespace RunDialog.Tests;

[TestClass]
public class HistoryRepositoryTests
{
    private Mock<IRunDialogSettings> _settingsMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _settingsMock = new Mock<IRunDialogSettings>();
        _settingsMock.Setup(s => s.MaxHistoryItems).Returns(5);
    }

    [TestMethod]
    public void Add_NewItem_InsertsAtTop()
    {
        var repo = new InMemoryHistoryRepository(_settingsMock.Object);
        repo.Add("cmd1");
        repo.Add("cmd2");

        var history = repo.GetHistory();
        Assert.AreEqual("cmd2", history[0]);
        Assert.AreEqual("cmd1", history[1]);
    }

    [TestMethod]
    public void Add_DuplicateItem_MovesToTop()
    {
        var repo = new InMemoryHistoryRepository(_settingsMock.Object);
        repo.Add("cmd1");
        repo.Add("cmd2");
        repo.Add("cmd1");

        var history = repo.GetHistory();
        Assert.HasCount(2, history);
        Assert.AreEqual("cmd1", history[0]);
        Assert.AreEqual("cmd2", history[1]);
    }

    [TestMethod]
    public void Add_ExceedsMax_RemovesOldest()
    {
        var repo = new InMemoryHistoryRepository(_settingsMock.Object);
        repo.Add("cmd1");
        repo.Add("cmd2");
        repo.Add("cmd3");
        repo.Add("cmd4");
        repo.Add("cmd5");
        repo.Add("cmd6");

        var history = repo.GetHistory();
        Assert.HasCount(5, history);
        Assert.IsFalse(history.Contains("cmd1"));
    }

    [TestMethod]
    public void Add_Whitespace_Ignored()
    {
        var repo = new InMemoryHistoryRepository(_settingsMock.Object);
        repo.Add("  ");

        Assert.IsEmpty(repo.GetHistory());
    }

    [TestMethod]
    public void Clear_RemovesAll()
    {
        var repo = new InMemoryHistoryRepository(_settingsMock.Object);
        repo.Add("cmd1");
        repo.Clear();

        Assert.IsEmpty(repo.GetHistory());
    }
}
