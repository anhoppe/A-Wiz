using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Collection of Views used foe serialization
    /// </summary>
    internal class View
    {
        public Dictionary<string, Node> Views { get; set; } = new();
    }
}
