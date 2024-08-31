using EasyChain;

internal class CarYearHandler : IHandler<Car>
{
    public async Task Handle(Car message, ChainDelegate<Car> next)
    {
        if (message.Year > 1960)
        {
            await next(message);
        }
    }
}
