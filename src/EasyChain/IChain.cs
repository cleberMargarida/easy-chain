using System.Threading.Tasks;

namespace EasyChain
{
    /// <summary>
    /// Represents a chain that can process messages of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the message that the chain will handle.</typeparam>
    public interface IChain<T>
    {
        /// <summary>
        /// Runs the chain with the specified message.
        /// </summary>
        /// <param name="message">The message to be processed by the chain.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Run(T message);
    }
}
