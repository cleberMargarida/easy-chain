namespace EasyChain;

/// <summary>
/// Represents a base interface for chain builders.
/// </summary>
public interface IChainConfig { }

/// <summary>
/// Represents a generic interface for chain builders that can configure a specific type of call chain.
/// </summary>
/// <typeparam name="T">The type of the message that the chain will handle.</typeparam>
public interface IChainConfig<T> : IChainConfig
{
    /// <summary>
    /// Configures the call chain using the specified chain configuration.
    /// </summary>
    /// <param name="callChain">The chain configuration to be used for configuring the chain.</param>
    void Configure(IChainBuilder<T> callChain);
}
