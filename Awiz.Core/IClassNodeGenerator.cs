using Awiz.Core.CodeInfo;
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
        void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to);

        void CreateClassNode(IGraph graph, ClassInfo classInfo);
    }
}
