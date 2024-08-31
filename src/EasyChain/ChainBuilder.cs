using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyChain;

internal partial class ChainBuilder<TMessage> : IChainBuilder<TMessage>, IForkBuilder<TMessage>
{
    public IChainBuilder<TMessage> SetNext<THandler>() where THandler : IHandler<TMessage>
    {
        CallChainDescriptors.Push(typeof(THandler));
        return this;
    }

    public IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        CallChainDescriptors.Push(fork);
        return this;
    }

    public IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        CallChainDescriptors.Push(fork);
        return this;
    }

    public IForkBuilder<TMessage> Fork(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        CallChainDescriptors.Push(fork);
        return this;
    }

    public IChainBuilder<TMessage> SetNext(Delegate @delegate)
    {
        CallChainDescriptors.Push(@delegate);
        return this;
    }

    public IChainBuilder<TMessage> Merge()
    {
        return this;
    }

    /// <summary>
    /// Gets the stack of handler types that make up the chain.
    /// </summary>
    /// <value>A stack containing the types of handlers in the order they were Pushed.</value>
    internal Stack<CallChainDescriptor<TMessage>> CallChainDescriptors { get; } = new Stack<CallChainDescriptor<TMessage>>();
}

internal partial class ChainBuilder<TMessage>
{
    private static readonly MethodInfo _whenAllMethod = ((MethodCallExpression)((Expression<Func<Task[], Task>>)(arr => Task.WhenAll(arr))).Body).Method;
    private static readonly MethodInfo _continueWithMethod = ((MethodCallExpression)((Expression<Func<Task, Func<Task, Task>, Task>>)((t, f) => t.ContinueWith(f))).Body).Method;
    private static readonly ParameterExpression _serviceProviderParam = Expression.Parameter(typeof(IServiceProvider));
    private static readonly ParameterExpression _typeParam = Expression.Parameter(typeof(Type));
    private static readonly MethodCallExpression _getServiceMethodCall = Expression.Call(instance: _serviceProviderParam, typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService)), _typeParam);
    private static readonly ParameterExpression _messageParam = Expression.Parameter(typeof(TMessage));
    private static readonly MethodInfo _handleMethod = typeof(IHandler<TMessage>).GetMethod(nameof(IHandler<TMessage>.Handle), BindingFlags.Public | BindingFlags.Instance)!;
    private static readonly IServiceProvider _parameterlessServiceProvider = new ParameterLessServiceServiceProvider();

    public IChain<TMessage> Build()
    {
        return Build(_parameterlessServiceProvider);
    }

    /// <summary>
    /// This method builds a <see cref="Chain{TMessage}"/> using the provided <see cref="IServiceProvider"/> and <see cref="ChainBuilder{TMessage}"/>  
    /// </summary>
    public IChain<TMessage> Build(IServiceProvider services)
    {
        var callExpression = ResolveChainMethodCall(
            this,
            _messageParam,
            _getServiceMethodCall,
            _typeParam);

        var chainLambda = Expression.Lambda<Func<TMessage, Task>>(callExpression, _messageParam);
        var chainInvokation = Expression.Invoke(chainLambda, _messageParam);
        var next = Expression.Lambda<Func<IServiceProvider, TMessage, Task>>(chainInvokation, _serviceProviderParam, _messageParam);

        return new Chain<TMessage>(services, next.Compile());
    }

    private static MethodCallExpression ResolveChainMethodCall(
        ChainBuilder<TMessage> config,
        ParameterExpression messageParam,
        MethodCallExpression getServiceMethodCall,
        ParameterExpression typeParam)
    {
        MethodCallExpression next = Expression.Call(Expression.Constant(ChainBuilderHelpers<TMessage>.TaskCompleted.Target), ChainBuilderHelpers<TMessage>.TaskCompleted.Method, messageParam);

        foreach (var descriptor in config.CallChainDescriptors)
        {
            next = ResolveChainMethodCallInternal(messageParam, getServiceMethodCall, typeParam, _handleMethod, next, descriptor);
        }

        return next;
    }

    private static MethodCallExpression ResolveChainMethodCallInternal(
        ParameterExpression messageParam,
        MethodCallExpression getServiceMethodCall,
        ParameterExpression typeParam,
        MethodInfo handleMethod,
        MethodCallExpression callExpression,
        CallChainDescriptor<TMessage> descriptor)
    {
        if (descriptor.Type != null)
        {
            var getServiceMethodCallLambda = Expression.Lambda(getServiceMethodCall, parameters: [typeParam]);
            var instanceAsObject = Expression.Invoke(getServiceMethodCallLambda, Expression.Constant(descriptor.Type));
            var instance = Expression.Convert(instanceAsObject, descriptor.Type);
            var nextInvocation = Expression.Lambda<ChainDelegate<TMessage>>(callExpression, messageParam);

            return Expression.Call(instance, handleMethod, messageParam, nextInvocation);
        }

        if (descriptor.Fork != null)
        {
            List<InvocationExpression> invocationExpressions = [];

            foreach (var branch in descriptor.GetBranches())
            {
                var innerCallExpression = ResolveChainMethodCall(branch, messageParam, getServiceMethodCall, typeParam);
                var innerCallLambda = Expression.Lambda<ChainDelegate<TMessage>>(innerCallExpression, messageParam);
                invocationExpressions.Add(Expression.Invoke(innerCallLambda, messageParam));
            }

            var taskArray = Expression.NewArrayInit(typeof(Task), invocationExpressions);
            var combinedExpression = Expression.Call(_whenAllMethod, taskArray);
            var chainHandling = Expression.Lambda<ChainDelegate<TMessage>>(combinedExpression, messageParam);
            var chainHandlingInvoke = Expression.Invoke(chainHandling, messageParam);
            var continuation = Expression.Lambda<Func<Task, Task>>(callExpression, Expression.Parameter(typeof(Task)));

            return Expression.Call(chainHandlingInvoke, _continueWithMethod, continuation);
        }

        if (descriptor.Delegate != null)
        {
            var chainHandling = Expression.Lambda<ChainDelegate<TMessage>>(callExpression, messageParam);
            var @delegate = descriptor.Delegate;
            var instance = Expression.Constant(@delegate.Target);
            var parameters = @delegate.Method
                .GetParameters()
                .Select(p =>
                {
                    if (p.ParameterType == typeof(TMessage))
                    {
                        return messageParam.Reduce();
                    }

                    if (p.ParameterType == typeof(ChainDelegate<TMessage>))
                    {
                        return Expression.Lambda<ChainDelegate<TMessage>>(callExpression, messageParam).Reduce();
                    }

                    var parameterType = Expression.Constant(p.ParameterType);
                    var getServiceMethodCallLambda = Expression.Lambda(getServiceMethodCall, parameters: [typeParam]);
                    var instanceAsObject = Expression.Invoke(getServiceMethodCallLambda, Expression.Constant(p.ParameterType));

                    return Expression.Convert(instanceAsObject, p.ParameterType).Reduce();

                });

            return Expression.Call(instance, @delegate.Method, parameters);
        }

        throw new InvalidOperationException();
    }
}

file class ParameterLessServiceServiceProvider : IServiceProvider, IServiceScope, IServiceScopeFactory
{
    public IServiceProvider ServiceProvider => this;

    public IServiceScope CreateScope() => this;

    public void Dispose() { }

    public object GetService(Type serviceType)
    {
        if (typeof(IServiceScope).IsAssignableFrom(serviceType))
        {
            return this;
        }

        if (typeof(IServiceScopeFactory).IsAssignableFrom(serviceType))
        {
            return this;
        }

        return Activator.CreateInstance(serviceType);
    }
}
