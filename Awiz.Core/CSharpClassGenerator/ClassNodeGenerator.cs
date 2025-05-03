using Gwiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract;

namespace Awiz.Core.CSharpClassGenerator
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        private static readonly float FromToLabelOffsetPerCent = 5.0f;

        private Dictionary<string, INode> _nodeMap = new();

        public IArchitectureView ArchitectureView { get; set; } = new ArchitectureClassView();

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

        public void CreateClassNode(IGraph graph, ClassInfo classInfo)
        {
            var node = graph.AddNode("Class");

            ArchitectureView.AddClassNode(node, classInfo);

            _nodeMap[classInfo.Id] = node;

            node.Grid.Cells[0][0].Text = classInfo.Name;

            node.Grid.Cells[0][1].Text = classInfo.Properties.Aggregate("", (current, prop) => current + $"{prop}\n");
            node.Grid.Cells[0][2].Text = classInfo.Methods.Aggregate("", (current, method) => current + $"{method}\n");

            node.Width = 120;
            node.Height = 160;
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
