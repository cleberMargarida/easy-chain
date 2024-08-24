using System;
using System.Collections.Generic;

namespace EasyChain
{
    internal class ChainConfig<TMessage> : IChainConfig<TMessage>
    {
        public IChainConfig<TMessage> Add<THandler>() where THandler : IHandler<TMessage>
        {
            ChainTypes.Push(typeof(THandler));
            return this;
        }

        /// <summary>
        /// Gets the stack of handler types that make up the chain.
        /// </summary>
        /// <value>A stack containing the types of handlers in the order they were added.</value>
        public Stack<Type> ChainTypes { get; } = new Stack<Type>();
    }
}
