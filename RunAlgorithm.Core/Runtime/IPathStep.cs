using System.Text;

namespace RunAlgorithm.Core.Runtime;

internal interface IPathStep
{
    IPathStep? Parent { get; }
    void WriteTo(StringBuilder sb);
}