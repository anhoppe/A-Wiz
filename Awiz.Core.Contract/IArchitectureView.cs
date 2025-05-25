using Gwiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.Git;

namespace Awiz.Core.Contract
{
    /// <summary>
    /// Information on a loaded architecture view
    /// </summary>
    public interface IArchitectureView
    {
        /// <summary>
        /// Event is raised when a class is selected
        /// </summary>
        event EventHandler<ClassInfo>? ClassSelected;

        /// <summary>
        /// Event is raised when a node is added
        /// </summary>
        event EventHandler<INode>? NodeAdded;

        /// <summary>
        /// Event is raised when the version diff of a class node is shown
        /// </summary>
        event EventHandler<ClassInfo>? ShowVersionDiff;

        /// <summary>
        /// The graph showing the view
        /// </summary>
        IGraph? Graph { get; }

        /// <summary>
        /// Name of the architecture view
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the type of the represented architecture view
        /// </summary>
        ArchitectureViewType Type { get; }

        /// <summary>
        /// Adds the base class of the passed class to the diagram.
        /// 
        /// </summary>
        /// <param name="derivedClassInfo">The derived class the base class is inserted for</param>
        void AddBaseClassNode(ClassInfo derivedClassInfo);

        /// <summary>
        /// Adds a node to the graph, only possible when the loaded view is a class diagram
        /// </summary>
        /// <param name="classInfo"></param>
        void AddClassNode(ClassInfo classInfo);

        /// <summary>
        /// Adds a call from one class to another in a sequence diagram
        /// </summary>
        /// <param name="sourceClass"></param>
        /// <param name="targetClass"></param>
        /// <param name="methodInfo"></param>
        void AddMethodCall(ClassInfo sourceClass, ClassInfo targetClass, MethodInfo methodInfo);

        /// <summary>
        /// Adds a node to the graph, only possible when the loaded view is a use case diagram
        /// </summary>
        /// <param name="node"></param>
        void AddUseCaseNode(INode node);

        /// <summary>
        /// Gets the commits associated with a node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IGitNodeInfo GetAssociatedCommits(INode node);

        /// <summary>
        /// Loads the information that is associated with the view
        /// </summary>
        void Load(IVersionUpdater versioUpdater);

        /// <summary>
        /// Saves the view to disc
        /// </summary>
        void Save();
    }
}
