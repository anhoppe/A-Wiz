using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Generates nodes in the graph for sequence diagrams
    /// </summary>
    internal interface ISequenceNodeGenerator
    {
        /// <summary>
        /// Access tohe current height of the lifeline in the sequence diagram.
        /// </summary>
        int CurrentLifelineHeight { get; set; }

        /// <summary>
        /// Gets the unique ID of the lifeline node in the sequence diagram.
        /// </summary>
        string LifelineId { get; }

        /// <summary>
        /// Access to the mapping of nodes to class information.
        /// </summary>
        IDictionary<INode, ClassInfo> NodeToClassInfo { get; }

        /// <summary>
        /// Gets the class info object associated with the user lifeline.
        /// </summary>
        ClassInfo UserClass { get; }

        /// <summary>
        /// The node that represents the user lifeline in the sequence diagram.
        /// </summary>
        INode UserLifeline { get; }

        void CreateClassNode(IGraph graph, ClassInfo classInfo);

        /// <summary>
        /// Creates a method call in the graph based on the passed callInfo
        /// </summary>
        /// <param name="graph">Graph the arrow that represents the method call is added to</param>
        /// <param name="callInfo">Information on the method call to be added</param>
        void CreateMethodCall(IGraph graph, CallInfo callInfo);

        bool CreateReturnCall(IGraph graph, CallInfo callInfo);

        /// <summary>
        /// Initializes the sequence node generator
        /// </summary>
        /// <param name="graph">Reference to the graph the sequence diagram is created in</param>
        void Initialize(IGraph graph);

        /// <summary>
        /// Restores the sequence diagram from the mapping of node ids to class information
        /// </summary>
        /// <param name="graph">The graph that contains the nodes of the diagram</param>
        /// <param name="nodeIdToClassInfoMapping">Mapping from node ID's to class infos</param>
        void Restore(IGraph graph, IDictionary<string, ClassInfo> nodeIdToClassInfoMapping);

        /// <summary>
        /// Sets the number of lifelines in the sequence diagram.
        /// </summary>
        /// <param name="lifelineCounter"></param>
        void SetLifelineCounter(int lifelineCounter);
    }
}
