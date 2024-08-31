using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EasyChain;

/// <summary>
/// Represents a chain of responsibility pattern implementation for processing messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of message that the chain will handle.</typeparam>
public class Chain<TMessage> : IChain<TMessage>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Func<IServiceProvider, TMessage, Task> _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="Chain{TMessage}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies within the chain.</param>
    /// <param name="next">The delegate representing the next handler in the chain.</param>
    internal Chain(IServiceProvider serviceProvider, Func<IServiceProvider, TMessage, Task> next)
    {
        _serviceProvider = serviceProvider;
        _next = next;
    }

    /// <summary>
    /// Creates a new chain builder for building a chain to handle <typeparamref name="TMessage"/> messages.
    /// </summary>
    /// <returns>An instance of <see cref="IChainBuilder{TMessage}"/> to construct the chain.</returns>
    public static IChainBuilder<TMessage> CreateBuilder()
    {
        return new ChainBuilder<TMessage>();
    }

    /// <summary>
    /// Executes the chain with the given message.
    /// </summary>
    /// <param name="message">The message to be processed by the chain.</param>
    /// <returns>A task representing the asynchronous operation of running the chain.</returns>
    public async Task Run(TMessage message)
    {
        using var scope = _serviceProvider.CreateScope();
        await _next(scope.ServiceProvider, message);
    }
}
