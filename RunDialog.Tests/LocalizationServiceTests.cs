using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunDialog.Core.Services;
using System.Globalization;

namespace RunDialog.Tests;

[TestClass]
public class LocalizationServiceTests
{
    private readonly LocalizationService _localization = new();

    [TestMethod]
    public void Keys_AreInEnglish()
    {
        // This test documents the design decision: resource keys MUST be in English.
        var keys = new[]
        {
            "WindowTitle", "OpenPrompt", "DescriptionText", "OkButton",
            "CancelButton", "BrowseButton", "RunAsAdmin", "Placeholder",
            "ErrorNotFound", "ErrorNoStrategy", "ErrorInvalidUri",
            "ErrorInvalidFolder", "HistoryEmpty", "SettingsTitle"
        };

        _localization.CurrentCulture = "en-US";

        foreach (var key in keys)
        {
            var value = _localization.GetString(key);
            Assert.DoesNotStartWith("[", value, $"Key '{key}' was not found in resources.");
        }
    }

    [TestMethod]
    public void Polish_ReturnsPolishText()
    {
        _localization.CurrentCulture = "pl-PL";

        var title = _localization.GetString("WindowTitle");
        var cancel = _localization.GetString("CancelButton");

        Assert.AreEqual("Uruchamianie", title);
        Assert.AreEqual("Anuluj", cancel);
    }

    [TestMethod]
    public void English_ReturnsEnglishText()
    {
        _localization.CurrentCulture = "en-US";

        var title = _localization.GetString("WindowTitle");
        var cancel = _localization.GetString("CancelButton");

        Assert.AreEqual("Run", title);
        Assert.AreEqual("Cancel", cancel);
    }

    [TestMethod]
    public void Toggle_BetweenCultures_SwitchesText()
    {
        _localization.CurrentCulture = "en-US";
        Assert.AreEqual("Run", _localization.GetString("WindowTitle"));

        _localization.CurrentCulture = "pl-PL";
        Assert.AreEqual("Uruchamianie", _localization.GetString("WindowTitle"));
    }

    [TestMethod]
    public void UnsupportedCulture_ThrowsCultureNotFoundException()
    {
        Assert.ThrowsExactly<CultureNotFoundException>(() => _localization.CurrentCulture = "de-DE");
    }

    [TestMethod]
    public void SupportedCultures_ContainsPolishAndEnglish()
    {
        var cultures = _localization.SupportedCultures;
        CollectionAssert.Contains(cultures.ToList(), "pl-PL");
        CollectionAssert.Contains(cultures.ToList(), "en-US");
    }

    [TestMethod]
    public void MissingKey_ReturnsBracketedKey()
    {
        _localization.CurrentCulture = "en-US";
        var result = _localization.GetString("NonExistentKey");

        Assert.AreEqual("[NonExistentKey]", result);
    }
}
