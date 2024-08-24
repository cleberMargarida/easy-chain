using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EasyChain;

internal class Chain<TMessage>(IServiceProvider serviceProvider, Func<IServiceScope, Func<TMessage, Task>> begin) : IChain<TMessage>
{
    public async Task Run(TMessage message)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            await begin(scope).Invoke(message);
        }
    }
}
