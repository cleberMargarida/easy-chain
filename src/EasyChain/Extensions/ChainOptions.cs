using Microsoft.Extensions.DependencyInjection;

namespace EasyChain.Extensions;

/// <summary>
/// Represents the configuration options for the chain, 
/// including the service lifetime for the handlers.
/// </summary>
public class ChainOptions
{
    /// <summary>
    /// Gets or sets the service lifetime that determines how the services 
    /// (such as chain handlers) are instantiated and shared within a service container.
    /// Defaults to <see cref="ServiceLifetime.Scoped"/>.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
}
