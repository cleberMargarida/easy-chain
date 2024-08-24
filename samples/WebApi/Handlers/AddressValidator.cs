using EasyChain;

internal class AddressValidator : IHandler<CustomerPayload>
{
    public async Task Handle(CustomerPayload message, ChainHandling<CustomerPayload> next)
    {
        if (!string.IsNullOrWhiteSpace(message.Address))
        {
            await next(message);
        }
    }
}
