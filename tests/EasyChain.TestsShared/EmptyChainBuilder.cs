using System.Diagnostics.CodeAnalysis;

namespace EasyChain.TestsShared;

[ExcludeFromCodeCoverage(Justification = "Test file.")]
public class EmptyChainBuilder : IChainConfig<object>
{
    public void Configure(IChainBuilder<object> callChain) { }
}
