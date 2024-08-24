using EasyChain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace EasyChain.UnitTests;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class ChainOptionsTests
{
    [Fact]
    public void New_WithoutInitServiceLifetime_ShouldBeServiceLifetimeScoped()
    {
        var options = new ChainOptions();

        var serviceLifetime = options.ServiceLifetime;

        Assert.Equal(ServiceLifetime.Scoped, serviceLifetime);
    }
    
    [Fact]
    public void New_WithInitServiceLifetime_ShouldBeServiceLifetimeInitialized()
    {
        var options = new ChainOptions { ServiceLifetime = ServiceLifetime.Singleton };

        var serviceLifetime = options.ServiceLifetime;

        Assert.Equal(ServiceLifetime.Singleton, serviceLifetime);
    }
}
