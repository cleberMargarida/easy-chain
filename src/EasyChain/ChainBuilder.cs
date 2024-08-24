using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyChain;

internal static class ChainBuilder
{
    /// <summary>
    /// This method builds a <see cref="Chain{TMessage}"/> using the provided <see cref="IServiceProvider"/> and <see cref="ChainConfig{TMessage}"/>  
    /// </summary>
    public static Chain<TMessage> Build<TBuilder, TMessage>(this TBuilder _, IServiceProvider services, ChainConfig<TMessage> config)
        where TBuilder : class, IChainBuilder, new()
    {
        // Define the parameter expression representing the message passed to the chain
        ParameterExpression messageParam = Expression.Parameter(typeof(TMessage));

        // Initialize the last invocation in the chain as a no-op that completes immediately
        ChainHandling<TMessage> lastInvocation = _ => Task.CompletedTask;

        // Start with the base delegate and call expression
        MethodCallExpression callExpression = Expression.Call(Expression.Constant(lastInvocation.Target), lastInvocation.Method, messageParam);

        // Define a variable expression for the IServiceScope parameter
        Expression scopeParam = Expression.Variable(typeof(IServiceScope));

        // Get the method info for IHandler<TMessage>.Handle
        MethodInfo handleMethod = typeof(IHandler<TMessage>).GetMethod(nameof(IHandler<TMessage>.Handle), BindingFlags.Public | BindingFlags.Instance)!;

        // Iterate over handler factories to build the chain of responsibility
        foreach (var type in config.ChainTypes)
        {
            // Create an expression for the next invocation in the chain
            var nextInvocation = Expression.Lambda<ChainHandling<TMessage>>(callExpression, messageParam).Reduce();

            // Build the call to IHandler<TMessage>.Handle with the current handler, message parameter, and next invocation
            callExpression = Expression.Call(Expression.Parameter(type), handleMethod, messageParam, nextInvocation);
        }

        // Compile the final expression into a delegate
        var tree = Expression.Lambda<Func<TMessage, Task>>(callExpression, messageParam);

        // Return a new Chain<TMessage> that uses the compiled delegate
        return new Chain<TMessage>(services, (s, m) => tree.Rewrite(s).Compile()(m));
    }

    // Rewrites the expression tree, injecting the IServiceScope into the expression
    private static Expression<Func<TMessage, Task>> Rewrite<TMessage>(this Expression<Func<TMessage, Task>> tree, IServiceScope scope)
    {
        return new ChainExpressionRewriter().Rewrite(tree, scope);
    }
}

internal sealed class ChainExpressionRewriter : ExpressionVisitor
{
    private IServiceProvider _services = default!;

    /// <summary>
    /// Visit parameters and replace IServiceScope parameters with a constant expression
    /// </summary>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return typeof(IHandler).IsAssignableFrom(node.Type) ? Expression.Constant(_services.GetService(node.Type)) : base.VisitParameter(node);
    }

    /// <summary>
    /// Rewrite the expression tree by replacing IServiceScope parameters with the provided value
    /// </summary>
    internal Expression<Func<TInput, TOutput>> Rewrite<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression, IServiceScope scope)
    {
        _services = scope.ServiceProvider;
        return (Expression<Func<TInput, TOutput>>)Visit(expression);
    }
}
