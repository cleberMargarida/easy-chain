using EasyChain;

internal class CityValidator : IHandler<CustomerPayload>
{
    public async Task Handle(CustomerPayload message, ChainHandling<CustomerPayload> next)
    {
        if (!string.IsNullOrWhiteSpace(message.City))
        {
            await next(message);
        }
    }
}
