using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyChain;

/// <summary>
/// Represents a descriptor for a call chain, which can be a type of chain handler or a forked chain of handlers.
/// </summary>
public readonly struct CallChainDescriptor<TMessage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CallChainDescriptor{TMessage}"/> struct with the specified type.
    /// </summary>
    /// <param name="type">The type of the chain handler.</param>
    internal CallChainDescriptor(Type type)
    {
        Type = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallChainDescriptor{TMessage}"/> struct with a fork that accepts two <see cref="ChainBuilder{TMessage}"/> objects.
    /// </summary>
    /// <param name="fork">The action representing a fork in the chain with two branches.</param>
    internal CallChainDescriptor(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        Fork = fork;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallChainDescriptor{TMessage}"/> struct with a fork that accepts three <see cref="ChainBuilder{TMessage}"/> objects.
    /// </summary>
    /// <param name="fork">The action representing a fork in the chain with three branches.</param>
    internal CallChainDescriptor(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        Fork = fork;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallChainDescriptor{TMessage}"/> struct with a fork that accepts four <see cref="ChainBuilder{TMessage}"/> objects.
    /// </summary>
    /// <param name="fork">The action representing a fork in the chain with four branches.</param>
    internal CallChainDescriptor(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        Fork = fork;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delegate"></param>
    public CallChainDescriptor(Delegate @delegate) : this()
    {
        Delegate = @delegate;
    }

    /// <summary>
    /// Gets the type of the chain handler.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Gets the delegate representing the fork in the chain, which may have multiple branches.
    /// </summary>
    public Delegate? Fork { get; }

    /// <summary>
    /// 
    /// </summary>
    public Delegate? Delegate { get; }

    internal IEnumerable<ChainBuilder<TMessage>> GetBranches()
    {
        if (Fork == null)
        {
            return Enumerable.Empty<ChainBuilder<TMessage>>();
        }

        var length = Fork.Method.GetParameters().Length;

        var branches = new ChainBuilder<TMessage>[length];

        for (int i = 0; i < length; i++)
        {
            branches[i] = new ChainBuilder<TMessage>();
        }

        Fork.DynamicInvoke(branches);

        return branches;
    }

    public static implicit operator CallChainDescriptor<TMessage>(Type type)
    {
        return new CallChainDescriptor<TMessage>(type);
    }

    public static implicit operator CallChainDescriptor<TMessage>(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        return new CallChainDescriptor<TMessage>(fork);
    }

    public static implicit operator CallChainDescriptor<TMessage>(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        return new CallChainDescriptor<TMessage>(fork);
    }

    public static implicit operator CallChainDescriptor<TMessage>(Action<IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>, IChainBuilder<TMessage>> fork)
    {
        return new CallChainDescriptor<TMessage>(fork);
    }

    public static implicit operator CallChainDescriptor<TMessage>(Delegate @delegate)
    {
        return new CallChainDescriptor<TMessage>(@delegate);
    }
}
