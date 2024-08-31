using EasyChain;
using EasyChain.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the necessary services for a chain of responsibility pattern, including chain handlers and the chain builder itself.
    /// </summary>
    /// <typeparam name="T">The type of the chain builder, which must implement <see cref="IChainConfig"/> and have a parameterless constructor.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the chain services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddChain<T>(this IServiceCollection services, Action<ChainOptions>? options = null)
        where T : class, IChainConfig, new()
    {
        ChainOptions config = new();

        options?.Invoke(config);

        Type builderType = typeof(T);
        Type handlerBaseType = typeof(IHandler);

        Type messageType = GetMessageType(builderType);
        Type[] handlerTypes = GetHandlerTypes(builderType.Assembly, handlerBaseType);

        RegisterHandlers(services, handlerTypes, config);
        RegisterChain(services, builderType, messageType, config);

        return services.AddSingleton<T>();
    }

    private static Type GetMessageType(Type builderType)
    {
        return builderType.GetInterfaces()
                          .First(i => i.IsGenericType && i.Name.StartsWith(nameof(IChainConfig)))
                          .GetGenericArguments()
                          .First();
    }

    private static Type[] GetHandlerTypes(Assembly assembly, Type handlerBaseType)
    {
        return assembly.GetTypes()
                       .Where(t => handlerBaseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                       .ToArray();
    }

    private static void RegisterHandlers(IServiceCollection services, Type[] handlerTypes, ChainOptions config)
    {
        foreach (var handler in handlerTypes)
        {
            var descriptor = new ServiceDescriptor(handler, handler, config.ServiceLifetime);
            services.TryAdd(descriptor);
        }
    }

    private static void RegisterChain(IServiceCollection services, Type builderType, Type messageType, ChainOptions config)
    {
        MethodInfo addChainMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(AddChainInternal), BindingFlags.NonPublic | BindingFlags.Static);
        addChainMethod.MakeGenericMethod(builderType, messageType)
                      .Invoke(null, new object[] { services });
    }

    private static void AddChainInternal<TConfig, TMessage>(IServiceCollection services)
        where TConfig : class, IChainConfig<TMessage>, new()
    {
        var config = new ChainBuilder<TMessage>();
        var descriptor = BuildChain<TConfig, TMessage>(config);
        services.TryAddEnumerable(descriptor);
    }

    private static ServiceDescriptor BuildChain<TConfig, TMessage>(ChainBuilder<TMessage> builder)
        where TConfig : class, IChainConfig<TMessage>,
        new() => ServiceDescriptor.Singleton<IChain<TMessage>, Chain<TMessage>>(provider =>
    {
        TConfig config = provider.GetService<TConfig>();
        config.Configure(builder);
        return (Chain<TMessage>)builder.Build(provider);
    });
}
