using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunDialog.Core.Services;

namespace RunDialog.Tests;

[TestClass]
public class RunDialogSettingsTests
{
    [TestMethod]
    public void DefaultValues_AreCorrect()
    {
        var settings = new RunDialogSettings();

        Assert.IsTrue(settings.ShowAutoComplete);
        Assert.AreEqual(25, settings.MaxHistoryItems);
        Assert.IsFalse(settings.RunAsAdministrator);
    }

    [TestMethod]
    public void RunAsAdministrator_CanBeChanged()
    {
        var settings = new RunDialogSettings();
        settings.RunAsAdministrator = true;

        Assert.IsTrue(settings.RunAsAdministrator);
    }
}
