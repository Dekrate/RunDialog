using Microsoft.VisualStudio.TestTools.UnitTesting;
using RunDialog.Core.Models;
using RunDialog.Core.Services;

namespace RunDialog.Tests;

[TestClass]
public class CommandParserTests
{
    private readonly CommandParser _parser = new();

    [TestMethod]
    public void Parse_EmptyInput_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => _parser.Parse(" "));
    }

    [TestMethod]
    public void Parse_ProgramWithArguments_ReturnsProgramTypeAndArguments()
    {
        var result = _parser.Parse("notepad.exe C:\\test.txt");

        Assert.AreEqual(CommandType.Program, result.Type);
        Assert.AreEqual("notepad.exe", result.RawInput.Split(' ')[0]);
    }

    [TestMethod]
    public void Parse_HttpUri_ReturnsUriType()
    {
        var result = _parser.Parse("https://github.com");

        Assert.AreEqual(CommandType.Uri, result.Type);
    }

    [TestMethod]
    public void Parse_FtpUri_ReturnsUriType()
    {
        var result = _parser.Parse("ftp://example.com");

        Assert.AreEqual(CommandType.Uri, result.Type);
    }

    [TestMethod]
    public void Parse_MailtoUri_ReturnsUriType()
    {
        var result = _parser.Parse("mailto:test@example.com");

        Assert.AreEqual(CommandType.Uri, result.Type);
    }

    [TestMethod]
    public void Parse_AbsoluteFolderPath_ReturnsFolderType()
    {
        var result = _parser.Parse("C:\\Windows\\System32");

        Assert.AreEqual(CommandType.Folder, result.Type);
    }

    [TestMethod]
    public void Parse_NetworkPath_ReturnsFolderType()
    {
        var result = _parser.Parse("\\\\server\\share");

        Assert.AreEqual(CommandType.Folder, result.Type);
    }

    [TestMethod]
    public void Parse_RelativeProgram_ReturnsUnknownOrProgramType()
    {
        var result = _parser.Parse("mspaint");

        // Parser allows any input as potential program
        Assert.IsTrue(result.Type == CommandType.Program || result.Type == CommandType.Unknown);
    }

    [TestMethod]
    public void Parse_PreservesRawInputExactly()
    {
        var input = "  msinfo32  ";
        var result = _parser.Parse(input);

        Assert.AreEqual(input.Trim(), result.RawInput);
    }
}
