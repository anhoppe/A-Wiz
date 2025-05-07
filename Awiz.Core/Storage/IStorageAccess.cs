using Awiz.Core.Contract.Git;
using Gwiz.Core.Contract;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Loads and stores persistent view information
    /// </summary>
    internal interface IStorageAccess
    {

        /// <summary>
        /// Loads a graph that belongs to a diagram
        /// </summary>
        /// <param name="name">Name of the diagram the graph is loaded for</param>
        /// <param name="path">Path to the graph that is loaded</param>
        /// <returns></returns>
        IGraph LoadDiagramGraph(string name, string path);

        /// <summary>
        /// Loads the GIT information for a node from a stream
        /// </summary>
        /// <param name="stream">The stream the git info is loaded from</param>
        Dictionary<string, IGitNodeInfo> LoadGitInfo(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitInfo"></param>
        /// <param name="stream"></param>
        void SaveGitInfo(Dictionary<string, IGitNodeInfo> gitInfo, Stream stream);
    }
}
