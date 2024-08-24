using System.Diagnostics.CodeAnalysis;

namespace EasyChain.TestsShared;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class TestHandler : IHandler<object>
{
    public int Invocations { get; private set; }

    public async Task Handle(object message, ChainHandling<object> next)
    {
        Invocations++;
        if (message is not "stop on first")
        {
            await next(message);
        }
    }
}

public class TestHandler2 : TestHandler, IHandler<object> { }
