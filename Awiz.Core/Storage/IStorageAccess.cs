using Awiz.Core.Contract.Git;
using Gwiz.Core.Contract;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Loads and stores persistent view information
    /// </summary>
    internal interface IStorageAccess
    {
        IGraph LoadClassGraph();
        
        IGraph LoadUseCaseGraph(string useCaseName, string path);

        /// <summary>
        /// Loads the GIT information for a node from a stream
        /// </summary>
        /// <param name="stream">The stream the git info is loaded from</param>
        public Dictionary<string, IGitNodeInfo> LoadGitInfo(Stream stream);

        /// <summary>
        /// Loads persisted information for a node from a view
        /// </summary>
        /// <param name="targetNode">the target node that receives the persisted information</param>
        /// <param name="viewName">Name of the view the node is loaded for</param>
        /// <param name="stream">The stream that contains the persisted information</param>
        /// <returns>The complete view collection which is used to safe the information</returns>
        View LoadNode(INode targetNode, string viewName, Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitInfo"></param>
        /// <param name="stream"></param>
        void SaveGitInfo(Dictionary<string, IGitNodeInfo> gitInfo, Stream stream);

        /// <summary>
        /// Saves a node to the persistence storage
        /// </summary>
        /// <param name="targetNode">The node that contains the information to be persisted</param>
        /// <param name="targetView">The view the node info is in</param>
        /// <param name="viewName">Name of the view the node info is persisted for</param>
        /// <param name="stream">The stream the info is persisted to</param>
        void SaveNode(INode targetNode, View targetView, string viewName, Stream stream);
    }
}
