using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Storage;
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

        /// <summary>
        /// Creates a new class node in the sequence diagram based on the provided ClassInfo.
        /// </summary>
        /// <param name="classInfo">The class info the node is generated for</param>
        /// <returns>true if currently a call sequence is in progress</returns>
        bool CreateClassNode(ClassInfo classInfo);

        /// <summary>
        /// Creates a method call in the graph based on the passed callInfo
        /// </summary>
        /// <param name="graph">Graph the arrow that represents the method call is added to</param>
        /// <param name="callInfo">Information on the method call to be added</param>
        CallInfo CreateMethodCall(ClassInfo sourceClass, ClassInfo targetClass, MethodInfo calledMethod);

        /// <summary>
        /// Returns the last call in the call stack and creates a return call for it.
        /// </summary>
        /// <returns>Tuple containing previous call and returned call. Previous call is null when the call returned to the user</returns>
        (CallInfo?, CallInfo) CreateReturnCall();

        /// <summary>
        /// Returns all class information of classes in the sequence diagram.
        /// UserClass is excluded from the list.
        /// </summary>
        /// <returns>List of all classes represented in the diagram</returns>
        List<ClassInfo> GetLifelineClassInfosInDiagram();

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
        /// Starts a call sequence from the user, allowing the user to select the target class and method to call.
        /// </summary>
        /// <param name="graph">The graph the call is added to</param>
        /// <param name="targetClass">The class the method call is going to</param>
        /// <param name="calledMethod"></param>
        /// <returns></returns>
        CallInfo StartCallSequenceFromUser(ClassInfo targetClass, MethodInfo calledMethod);
    }
}
