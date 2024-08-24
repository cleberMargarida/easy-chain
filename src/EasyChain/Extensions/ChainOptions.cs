namespace Microsoft.Extensions.DependencyInjection;

public class ChainOptions
{
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
}