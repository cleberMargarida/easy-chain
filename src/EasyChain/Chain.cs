using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EasyChain;

internal class Chain<TMessage>(IServiceProvider serviceProvider, Func<IServiceScope, TMessage, Task> begin) : IChain<TMessage>
{
    public async Task Run(TMessage message)
    {
        using var scope = serviceProvider.CreateScope();
        await begin(scope, message);
    }
}
