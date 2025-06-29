using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.Git;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Loads and stores persistent view information
    /// </summary>
    internal interface IStorageAccess
    {
        /// <summary>
        /// Loads a callstack used in a sequnce diagram from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        Stack<CallInfo> LoadSequenceCallstack(Stream stream, IDictionary<INode, ClassInfo> nodeToClassInfoMapping);

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
        /// Loads the mapping of node ids to class ids from a stream
        /// </summary>
        /// <param name="stream">Strean the mapping is loaded from</param>
        /// <returns>Dictionary with the mapping</returns>
        IDictionary<string, ClassInfo> LoadNodeIdToClassInfoMapping(Stream stream);

        /// <summary>
        /// Saves the callstack used for a sequnce diagram to a stream
        /// </summary>
        void SaveSequenceCallstack(Stack<CallInfo> callstack, Stream stream);

        /// <summary>
        /// Saves the git info to a stream
        /// </summary>
        /// <param name="gitInfo"></param>
        /// <param name="stream"></param>
        void SaveGitInfo(Dictionary<string, IGitNodeInfo> gitInfo, Stream stream);

        /// <summary>
        /// Saves mapping fom node to class id to stream.
        /// The mapping is used to determine to which class info the node was created from.
        /// </summary>
        /// <param name="nodeToClassMapping"></param>
        /// <param name="stream"></param>
        void SaveNodeIdToClassInfoMapping(IDictionary<string, ClassInfo> nodeToClassMapping, Stream stream);
    }
}
