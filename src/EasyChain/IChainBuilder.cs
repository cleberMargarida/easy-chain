using System;

namespace EasyChain;

/// <summary>
/// Represents the configuration for building a chain of handlers for messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of the message that the chain will handle.</typeparam>
public interface IChainBuilder<TMessage> : IBuildable<TMessage>
{
    /// <summary>
    /// Adds a handler type to the chain configuration.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler to add to the chain. 
    /// Must implement <see cref="IHandler{TMessage}"/>.</typeparam>
    /// <returns>The current <see cref="IChainBuilder{TMessage}"/> instance for further configuration.</returns>
    IChainBuilder<TMessage> SetNext<THandler>() where THandler : IHandler<TMessage>;

    /// <summary>
    /// Adds a delegate as the next handler in the chain.
    /// </summary>
    /// <param name="delegate">The delegate to be added to the chain.</param>
    /// <returns>The current <see cref="IChainBuilder{TMessage}"/> instance for further configuration.</returns>
    IChainBuilder<TMessage> SetNext(Delegate @delegate);

    /// <summary>
    /// Forks the chain into two separate branches.
    /// </summary>
    /// <param name="fork">An action to configure the two branches of the fork.</param>
    /// <returns>An instance of <see cref="IForkBuilder{TMessage}"/> for further configuration of the forked chain.</returns>
    IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork);

    /// <summary>
    /// Forks the chain into three separate branches.
    /// </summary>
    /// <param name="fork">An action to configure the three branches of the fork.</param>
    /// <returns>An instance of <see cref="IForkBuilder{TMessage}"/> for further configuration of the forked chain.</returns>
    IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork);

    /// <summary>
    /// Forks the chain into four separate branches.
    /// </summary>
    /// <param name="fork">An action to configure the four branches of the fork.</param>
    /// <returns>An instance of <see cref="IForkBuilder{TMessage}"/> for further configuration of the forked chain.</returns>
    IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork);
}
