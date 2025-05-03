using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Collection of Views used for serialization
    /// </summary>
    internal class View
    {
        /// <summary>
        /// Contains the Node information from different views
        /// In multiple views (e.g. different class diagrams)
        /// the node can contain different information.
        /// </summary>
        public Dictionary<string, Node> Views { get; set; } = new();
    }
}
