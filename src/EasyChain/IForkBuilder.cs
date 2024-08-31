namespace EasyChain;

/// <summary>
/// Represents a builder for handling forked chains that can later be merged back into a single chain.
/// </summary>
/// <typeparam name="TMessage">The type of message that the forked chain will handle.</typeparam>
public interface IForkBuilder<TMessage> : IBuildable<TMessage>
{
    /// <summary>
    /// Merges the forked branches back into a single chain.
    /// </summary>
    /// <returns>An <see cref="IChainBuilder{TMessage}"/> instance for continuing the chain configuration after the fork.</returns>
    IChainBuilder<TMessage> Merge();
}
