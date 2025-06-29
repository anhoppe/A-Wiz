using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Generates nodes in the graph for sequence diagrams
    /// </summary>
    internal interface ISequenceNodeGenerator
    {
        const string LifelineId = "8dbe0ba5-ef9a-4f1d-a4a2-fc7e16c1286d";

        (INode, INode) CreateClassNode(IGraph graph, ClassInfo classInfo, int lifelineHeight);

        /// <summary>
        /// Creates a method call in the graph based on the passed callInfo
        /// </summary>
        /// <param name="graph">Graph the arrow that represents the method call is added to</param>
        /// <param name="callInfo">Information on the method call to be added</param>
        void CreateMethodCall(IGraph graph, CallInfo callInfo);

        void CreateReturnCall(IGraph graph, CallInfo callInfo);

        /// <summary>
        /// Sets the number of lifelines in the sequence diagram.
        /// </summary>
        /// <param name="lifelineCounter"></param>
        void SetLifelineCounter(int lifelineCounter);
    }
}
