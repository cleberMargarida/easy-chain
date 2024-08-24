using System.Diagnostics.CodeAnalysis;

namespace EasyChain.TestsShared;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class EmptyChainBuilder : IChainBuilder<object>
{
    public void Configure(IChainConfig<object> callChain) { }
}
