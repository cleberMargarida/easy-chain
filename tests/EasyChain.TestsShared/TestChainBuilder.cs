using System.Diagnostics.CodeAnalysis;

namespace EasyChain.TestsShared;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class TestChainBuilder : IChainConfig<object>
{
    public void Configure(IChainBuilder<object> builder)
    {
        builder.SetNext<TestHandler>()
               .Fork((b1, b2) => { })
               .Merge()
               .SetNext<TestHandler2>()
               .SetNext(async () => { });
    }
}
