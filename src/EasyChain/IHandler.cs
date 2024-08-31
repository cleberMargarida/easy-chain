using System.Threading.Tasks;

namespace EasyChain;

/// <summary>
/// Represents a base interface for handlers in the chain.
/// </summary>
public interface IHandler { }

/// <summary>
/// Represents a generic interface for handlers that process messages of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the message that the handler will process.</typeparam>
public interface IHandler<T> : IHandler
{
    /// <summary>
    /// Handles the specified message and calls the next handler in the chain.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <param name="next">The delegate representing the next handler in the chain.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(T message, ChainDelegate<T> next);
}

/// <summary>
/// Represents a delegate that defines the signature for handling messages in the chain.
/// </summary>
/// <typeparam name="T">The type of the message that the delegate handles.</typeparam>
/// <param name="message">The message to be processed.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task ChainDelegate<T>(T message);
