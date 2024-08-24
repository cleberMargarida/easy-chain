using EasyChain;

internal class CustomerChain : IChainBuilder<CustomerPayload>
{
    public void Configure(IChainConfig<CustomerPayload> callChain)
    {
        callChain.Add<AgeValidator>()
                 .Add<NameValidator>()
                 .Add<AddressValidator>()
                 .Add<CityValidator>();
    }
}
