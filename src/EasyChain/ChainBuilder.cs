using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
        // Create factories to generate IHandler<TMessage> instances using IServiceScope
        var handlerFactories = config.ChainTypes.Select<Type, Func<IServiceScope, IHandler<TMessage>>>(
            static t => s => (IHandler<TMessage>)s.ServiceProvider.GetService(t));

        // Define the parameter expression representing the message passed to the chain
        ParameterExpression messageParam = Expression.Parameter(typeof(TMessage));

        // Initialize the last invocation in the chain as a no-op that completes immediately
        ChainHandling<TMessage> lastInvocation = _ => Task.CompletedTask;

        // Start with the base delegate and call expression
        var callExpression = Expression.Call(Expression.Constant(lastInvocation.Target), lastInvocation.Method, messageParam);

        // Define a variable expression for the IServiceScope parameter
        Expression scopeParam = Expression.Variable(typeof(IServiceScope));

        // Get the method info for IHandler<TMessage>.Handle
        MethodInfo handleMethod = typeof(IHandler<TMessage>)
            .GetMethod(nameof(IHandler<TMessage>.Handle), BindingFlags.Public | BindingFlags.Instance)
            ?? throw new NullReferenceException("Handle method not found on IHandler<TMessage>");

        // Iterate over handler factories to build the chain of responsibility
        foreach (Func<IServiceScope, IHandler<TMessage>> handlerFactory in handlerFactories)
        {
            // Create an expression to call the handler factory method with the scope parameter
            var handlerExpression = Expression.Call(Expression.Constant(handlerFactory.Target), handlerFactory.Method, scopeParam);

            // Create an expression for the next invocation in the chain
            var nextInvocation = Expression.Lambda<ChainHandling<TMessage>>(callExpression, messageParam).Reduce();

            // Build the call to IHandler<TMessage>.Handle with the current handler, message parameter, and next invocation
            callExpression = Expression.Call(handlerExpression, handleMethod, messageParam, nextInvocation);
        }

        // Compile the final expression into a delegate
        var finalExpressionTree = Expression.Lambda<Func<TMessage, Task>>(callExpression, messageParam);

        // Return a new Chain<TMessage> that uses the compiled delegate
        return new Chain<TMessage>(services, s => message => finalExpressionTree.Rewrite(s).Compile().Invoke(message));
    }

    // Rewrites the expression tree, injecting the IServiceScope into the expression
    private static Expression<Func<TMessage, Task>> Rewrite<TMessage>(this Expression<Func<TMessage, Task>> tree, IServiceScope scope)
    {
        return new ServiceScopeParameterRewriter().Rewrite(tree, scope);
    }
}

// This class rewrites an expression tree, replacing parameters of type IServiceScope with a constant
internal sealed class ServiceScopeParameterRewriter : ExpressionVisitor
{
    private Expression _constant = default!;

    /// <summary>
    /// Visit parameters and replace IServiceScope parameters with a constant expression
    /// </summary>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node.Type == typeof(IServiceScope) ? _constant : base.VisitParameter(node);
    }

    /// <summary>
    /// Rewrite the expression tree by replacing IServiceScope parameters with the provided value
    /// </summary>
    internal Expression<Func<TInput, TOutput>> Rewrite<TInput, TOutput>(Expression<Func<TInput, TOutput>> expression, IServiceScope scope)
    {
        _constant = Expression.Constant(scope);
        return (Expression<Func<TInput, TOutput>>)Visit(expression);
    }
}
