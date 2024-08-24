namespace EasyChain;

/// <summary>
/// Represents the configuration for building a chain of handlers for messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="T">The type of the message that the chain will handle.</typeparam>

public interface IChainConfig<T>
{
    /// <summary>
    /// Adds a handler type to the chain configuration.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add to the chain. Must implement <see cref="IHandler{T}"/>.</typeparam>

    void Add<THandler>() where THandler : IHandler<T>;
}
