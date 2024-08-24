using Microsoft.Extensions.DependencyInjection;

namespace EasyChain.Extensions;

public class ChainOptions
{
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
}