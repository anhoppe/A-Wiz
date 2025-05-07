using Gwiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract;
using Awiz.Core.Storage;
using Gwiz.Core;
using System.Xml.Linq;

namespace Awiz.Core.CSharpClassGenerator
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        private static readonly float FromToLabelOffsetPerCent = 5.0f;

        private Dictionary<string, INode> _nodeMap = new();

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to)
        {
            var node1 = _nodeMap[from.Id];
            var node2 = _nodeMap[to.Id];

            graph.AddEdge(node1, node2);
        }

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to, string fromMultiplicity, string toMultiplicity)
        {
            var node1 = _nodeMap[from.Id];
            var node2 = _nodeMap[to.Id];

            graph.AddEdge(node1, node2, fromMultiplicity, toMultiplicity, FromToLabelOffsetPerCent);
        }

        public INode CreateClassNode(IGraph graph, ClassInfo classInfo)
        {
            var node = graph.AddNode("Class");
            node.Grid.Cells[0][0].Text = classInfo.Name;

            node.Grid.Cells[0][1].Text = classInfo.Properties.Aggregate("", (current, prop) => current + $"{prop}\n");
            node.Grid.Cells[0][2].Text = classInfo.Methods.Aggregate("", (current, method) => current + $"{method}\n");

            node.Width = 120;
            node.Height = 160;

            _nodeMap[classInfo.Id] = node;

            return node;
        }

        public void CreateExtension(IGraph graph, ClassInfo baseClass, ClassInfo derivedClass)
        {
            var node1 = _nodeMap[baseClass.Id];
            var node2 = _nodeMap[derivedClass.Id];

            graph.AddEdge(node2, node1, Ending.ClosedArrow, Style.None);
        }

        public void CreateImplementation(IGraph graph, ClassInfo implementedInterface, ClassInfo implementingClass)
        {
            var node1 = _nodeMap[implementedInterface.Id];
            var node2 = _nodeMap[implementingClass.Id];

            graph.AddEdge(node2, node1, Ending.ClosedArrow, Style.Dashed);
        }
    }
}
