using Gwiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.CSharpClassGenerator
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        private static readonly float FromToLabelOffsetPerCent = 5.0f;

        public IDictionary<INode, ClassInfo>? NodeToClassInfoMapping { private get; set; }

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to)
        {
            var (node1, node2) = GetNodes(from, to);

            graph.AddEdge(node1, node2);
        }

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to, string fromMultiplicity, string toMultiplicity)
        {
            var (node1, node2) = GetNodes(from, to);

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

            return node;
        }

        public void CreateExtension(IGraph graph, ClassInfo baseClass, ClassInfo derivedClass)
        {
            var (node1, node2) = GetNodes(baseClass, derivedClass);

            graph.AddEdge(node2, node1, Ending.ClosedArrow, Style.None);
        }

        public void CreateImplementation(IGraph graph, ClassInfo implementedInterface, ClassInfo implementingClass)
        {
            var (node1, node2) = GetNodes(implementedInterface, implementingClass);

            graph.AddEdge(node2, node1, Ending.ClosedArrow, Style.Dashed);
        }

        private (INode node1, INode node2) GetNodes(ClassInfo class1, ClassInfo class2)
        {
            if (NodeToClassInfoMapping == null)
            {
                throw new NullReferenceException("NodeToClassInfoMapping is not set.");
            }

            var node1 = NodeToClassInfoMapping.FirstOrDefault(p => p.Value == class1).Key;
            if (node1 == null)
            {
                throw new ArgumentException($"No node found for class info {class1.Id}");
            }

            var node2 = NodeToClassInfoMapping.FirstOrDefault(p => p.Value == class2).Key;
            if (node2 == null)
            {
                throw new ArgumentException($"No node found for 'to' class info {class2.Id}");
            }

            return (node1, node2);
        }
    }
}
