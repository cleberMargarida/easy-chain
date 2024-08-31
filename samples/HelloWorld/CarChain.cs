using EasyChain;

internal class CarChain : IChainConfig<Car>
{
    public void Configure(IChainBuilder<Car> builder)
    {
        builder.SetNext<CarYearHandler>()
               .SetNext<CarModelHandler>();
    }
}
