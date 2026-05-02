using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RunDialog.Core.Interfaces;
using RunDialog.Core.Models;
using RunDialog.Core.Services;

namespace RunDialog.Tests;

[TestClass]
public class CommandExecutorTests
{
    [TestMethod]
    public void Execute_WithMatchingStrategy_ReturnsStrategyResult()
    {
        var command = new RunCommand("test", CommandType.Program);
        var expected = ExecutionResult.Ok("path");

        var mockStrategy = new Mock<IExecutionStrategy>();
        mockStrategy.Setup(s => s.CanExecute(command)).Returns(true);
        mockStrategy.Setup(s => s.Execute(command)).Returns(expected);

        var executor = new CommandExecutor(new[] { mockStrategy.Object });
        var result = executor.Execute(command);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("path", result.ExecutedPath);
    }

    [TestMethod]
    public void Execute_NoMatchingStrategy_ReturnsError()
    {
        var command = new RunCommand("test", CommandType.Program);

        var mockStrategy = new Mock<IExecutionStrategy>();
        mockStrategy.Setup(s => s.CanExecute(command)).Returns(false);

        var executor = new CommandExecutor(new[] { mockStrategy.Object });
        var result = executor.Execute(command);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Execute_MultipleStrategies_SelectsFirstMatching()
    {
        var command = new RunCommand("test", CommandType.Program);

        var first = new Mock<IExecutionStrategy>();
        first.Setup(s => s.CanExecute(command)).Returns(false);

        var second = new Mock<IExecutionStrategy>();
        second.Setup(s => s.CanExecute(command)).Returns(true);
        second.Setup(s => s.Execute(command)).Returns(ExecutionResult.Ok());

        var executor = new CommandExecutor(new[] { first.Object, second.Object });
        var result = executor.Execute(command);

        Assert.IsTrue(result.Success);
        second.Verify(s => s.Execute(command), Times.Once);
        first.Verify(s => s.Execute(It.IsAny<RunCommand>()), Times.Never);
    }

    [TestMethod]
    public void Execute_NullCommand_ThrowsArgumentNullException()
    {
        var executor = new CommandExecutor(Array.Empty<IExecutionStrategy>());
        Assert.ThrowsExactly<ArgumentNullException>(() => executor.Execute(null!));
    }
}
