using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Responsible for persistence of view information,
    /// e.g. when the position of a node is changed it is 
    /// stored in the .wiz folder structure
    /// </summary>
    public interface IViewPersistence
    {
        /// <summary>
        /// Adds a node to the persistence mechanism
        /// </summary>
        /// <param name="node">Node to be persisted</param>
        /// <param name="classInfo">Class info associated with the node</param>
        void AddNode(INode node, ClassInfo classInfo);
    }
}
