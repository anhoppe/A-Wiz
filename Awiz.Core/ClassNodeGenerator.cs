using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        public void Create(IGraph graph, ClassInfo classInfo)
        {
            var node = graph.AddNode("Class");

            node.Grid.FieldText[0][0] = classInfo.Name;

            node.Grid.FieldText[0][1] = classInfo.Properties.Aggregate("", (current, prop) => current + $"{prop}\n");
            node.Grid.FieldText[0][2] = classInfo.Methods.Aggregate("", (current, method) => current + $"{method}\n");

            node.Width = 120;
            node.Height = 160;
        }
    }
}
