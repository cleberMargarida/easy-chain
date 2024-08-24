using System.Diagnostics.CodeAnalysis;

namespace EasyChain.TestsShared;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class TestChainBuilder : IChainBuilder<object>
{
    public void Configure(IChainConfig<object> callChain)
    {
        callChain.Add<TestHandler>();
        callChain.Add<TestHandler2>();
    }
}
