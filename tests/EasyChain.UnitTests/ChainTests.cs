using EasyChain.TestsShared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace EasyChain.Tests;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class ChainTests
{
    [Fact]
    public void Run_WithMultipleHandlers_ShouldInvokeAllHandlersOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.AddChain<TestChainBuilder>(x => x.ServiceLifetime = ServiceLifetime.Singleton).BuildServiceProvider();
        var chain = serviceProvider.GetService<IChain<object>>();
        var handler1 = serviceProvider.GetService<TestHandler>();
        var handler2 = serviceProvider.GetService<TestHandler2>();

        // Act
        chain.Run(new());

        // Assert
        handler1.Invocations.Should().Be(1);
        handler2.Invocations.Should().Be(1);
    }

    [Fact]
    public void Run_WithNoHandlers_ShouldNotInvokeHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.AddChain<EmptyChainBuilder>(x => x.ServiceLifetime = ServiceLifetime.Singleton).BuildServiceProvider();
        var chain = serviceProvider.GetService<IChain<object>>();
        var handler1 = serviceProvider.GetService<TestHandler>();
        var handler2 = serviceProvider.GetService<TestHandler2>();

        // Act
        chain.Run(new());

        // Assert
        handler1.Invocations.Should().Be(0);
        handler2.Invocations.Should().Be(0);
    }

    [Fact]
    public void Run_WithMultipleHandlers_AndStoppingOnFirst_ShouldInvokeJustFirstHandlerOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.AddChain<TestChainBuilder>(x => x.ServiceLifetime = ServiceLifetime.Singleton).BuildServiceProvider();
        var chain = serviceProvider.GetService<IChain<object>>();
        var handler1 = serviceProvider.GetService<TestHandler>();
        var handler2 = serviceProvider.GetService<TestHandler2>();

        // Act
        chain.Run("stop on first");

        // Assert
        handler1.Invocations.Should().Be(1);
        handler2.Invocations.Should().Be(0);
    }

    [Fact]
    public void Run_WithBuildingInline_ShouldHandleCorrectly()
    {
        // Arrange
        var chain = Chain<object>.CreateBuilder()
                                 .SetNext<TestHandler>()
                                 .SetNext<TestHandler2>()
                                 .Build();

        // Act & Assert
        chain.Run(string.Empty);
    }
}
