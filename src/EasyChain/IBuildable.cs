using System;

namespace EasyChain;

/// <summary>
/// Represents a buildable chain of handlers for messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of message that the chain will handle.</typeparam>
public interface IBuildable<TMessage>
{
    /// <summary>
    /// Builds and returns a fully constructed chain of handlers.
    /// </summary>
    /// <returns>An instance of <see cref="IChain{TMessage}"/> that can process messages of type <typeparamref name="TMessage"/>.</returns>
    /// <remarks>When building without service provider make sure to include only parameterless handlers.</remarks>
    IChain<TMessage> Build();

    /// <summary>
    /// Builds and returns a fully constructed chain of handlers using the provided service provider.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve dependencies for the chain's handlers.</param>
    /// <returns>An instance of <see cref="IChain{TMessage}"/> that can process messages of type <typeparamref name="TMessage"/>.</returns>
    IChain<TMessage> Build(IServiceProvider serviceProvider);
}
