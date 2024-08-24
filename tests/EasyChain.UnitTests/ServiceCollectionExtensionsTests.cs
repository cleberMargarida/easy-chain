using EasyChain.TestsShared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace EasyChain.UnitTests;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddChain_WithServiceLifetimeOptions_ShouldApplyServiceLifetimeChainOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddChain<TestChainBuilder>(opt => opt.ServiceLifetime = ServiceLifetime.Singleton);

        // Assert
        services.First(x => x.ServiceType == typeof(TestHandler)).Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
