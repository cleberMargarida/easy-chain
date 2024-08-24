using EasyChain;

internal class CarResponsibilityChainBuilder : IChainBuilder<Car>
{
    public void Configure(IChainConfig<Car> callChain)
    {
        callChain.Add<CarYearHandler>();
        callChain.Add<CarModelHandler>();
    }
}
