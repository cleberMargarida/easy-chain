using EasyChain;

internal class CarModelHandler : IHandler<Car>
{
    public async Task Handle(Car message, ChainDelegate<Car> next)
    {
        if (message.Model == "FooModel")
        {
            await next(message);
        }
    }
}

internal class CarModelHandler2 : IHandler<Car>
{
    public async Task Handle(Car message, ChainDelegate<Car> next)
    {
        if (message.Model == "FooModel")
        {
            await next(message);
        }
    }
}
