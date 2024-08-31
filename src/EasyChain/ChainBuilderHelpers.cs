using EasyChain;
using System.Threading.Tasks;

internal static class ChainBuilderHelpers<TMessage>
{
    public static readonly ChainDelegate<TMessage> TaskCompleted = static _ => Task.CompletedTask;
}