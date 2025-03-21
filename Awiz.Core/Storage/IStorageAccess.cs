using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Loads and stores persistent view information
    /// </summary>
    internal interface IStorageAccess
    {
        /// <summary>
        /// Loads persisted information for a node from a view
        /// </summary>
        /// <param name="targetNode">the target node that receives the persisted information</param>
        /// <param name="viewName">Name of the view the node is loaded for</param>
        /// <param name="stream">The stream that contains the persisted information</param>
        /// <returns>The complete view collection which is used to safe the information</returns>
        View LoadNode(INode targetNode, string viewName, Stream stream);

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
