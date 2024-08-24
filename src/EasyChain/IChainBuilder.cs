namespace EasyChain
{
    /// <summary>
    /// Represents a base interface for chain builders.
    /// </summary>
    public interface IChainBuilder { }

    /// <summary>
    /// Represents a generic interface for chain builders that can configure a specific type of call chain.
    /// </summary>
    /// <typeparam name="T">The type of the message that the chain will handle.</typeparam>
    public interface IChainBuilder<T> : IChainBuilder
    {
        /// <summary>
        /// Configures the call chain using the specified chain configuration.
        /// </summary>
        /// <param name="callChain">The chain configuration to be used for configuring the chain.</param>
        void Configure(IChainConfig<T> callChain);
    }
}
