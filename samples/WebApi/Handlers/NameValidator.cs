using EasyChain;

internal class NameValidator : IHandler<CustomerPayload>
{
    public async Task Handle(CustomerPayload message, ChainHandling<CustomerPayload> next)
    {
        if (!string.IsNullOrWhiteSpace(message.Name))
        {
            await next(message);
        }
    }
}
