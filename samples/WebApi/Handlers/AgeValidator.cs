using EasyChain;

internal class AgeValidator : IHandler<CustomerPayload>
{
    public async Task Handle(CustomerPayload message, ChainHandling<CustomerPayload> next)
    {
        if (message.Age >= 0)
        {
            await next(message);
        }
    }
}
