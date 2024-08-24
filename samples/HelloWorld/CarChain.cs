using EasyChain;

internal class CarChain : IChainBuilder<Car>
{
    public void Configure(IChainConfig<Car> callChain)
    {
        callChain.Add<CarYearHandler>();
        callChain.Add<CarModelHandler>();
    }
}
