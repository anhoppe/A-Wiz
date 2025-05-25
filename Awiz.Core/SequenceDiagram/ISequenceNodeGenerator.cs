using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Generates nodes in the graph for sequence diagrams
    /// </summary>
    internal interface ISequenceNodeGenerator
    {
        (INode, INode) CreateClassNode(IGraph graph, ClassInfo classInfo);

        INode CreateMethodCall(IGraph graph, ClassInfo sourceLifeline, ClassInfo targetLifeline, MethodInfo methodInfo);
    }
}
