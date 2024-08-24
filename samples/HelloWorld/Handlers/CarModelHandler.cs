using EasyChain;

internal class CarModelHandler : IHandler<Car>
{
    public async Task Handle(Car message, ChainHandling<Car> next)
    {
        if (message.Model == "FooModel")
        {
            await next(message);
        }
    }
}
