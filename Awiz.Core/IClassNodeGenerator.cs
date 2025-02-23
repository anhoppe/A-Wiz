using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    /// <summary>
    /// Factory class to generate A-Wiz nodes from class info
    /// </summary>
    internal interface IClassNodeGenerator
    {
        void Create(IGraph graph, ClassInfo classInfo);
    }
}
