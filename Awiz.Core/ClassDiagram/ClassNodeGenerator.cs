using Gwiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.ClassDiagram
{
    internal class ClassNodeGenerator : IClassNodeGenerator
    {
        private static readonly float FromToLabelOffsetPerCent = 5.0f;

        public IDictionary<INode, ClassInfo>? NodeToClassInfoMapping { private get; set; }

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to)
        {
            var (node1, node2) = GetNodes(from, to);

            var edgeBuilder = graph.AddEdge(node1, node2);

            edgeBuilder.Build();
        }

        public void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to, string fromMultiplicity, string toMultiplicity)
        {
            var (node1, node2) = GetNodes(from, to);

            var edgeBuilder = graph.AddEdge(node1, node2);
            edgeBuilder.WithFromLabel(fromMultiplicity).
                WithToLabel(toMultiplicity).
                WithLabelOffsetPerCent(FromToLabelOffsetPerCent).
                Build();
        }

        public INode CreateClassNode(IGraph graph, ClassInfo classInfo, Action<ClassInfo> updateAction)
        {
            var nodeBuilder = graph.AddNode("Class");

            nodeBuilder.WithSize(Design.ClassNodeWidth, Design.ClassNodeHeight);

            var node = nodeBuilder.Build();
            UpdateClassNode(node, classInfo, updateAction);

            return node;
        }

        public void CreateExtension(IGraph graph, ClassInfo baseClass, ClassInfo derivedClass)
        {
            var (node1, node2) = GetNodes(baseClass, derivedClass);

            var edgeBuilder = graph.AddEdge(node2, node1);
            edgeBuilder.WithEnding(Ending.ClosedArrow).
                Build();
        }

        public void CreateImplementation(IGraph graph, ClassInfo implementedInterface, ClassInfo implementingClass)
        {
            var (node1, node2) = GetNodes(implementedInterface, implementingClass);

            var edgeBuilder = graph.AddEdge(node2, node1);
            edgeBuilder.WithEnding(Ending.ClosedArrow).
                WithStyle(Style.Dashed).
                Build();

        }

        public void UpdateClassNode(INode node, ClassInfo classInfo, Action<ClassInfo> updateCallback)
        {
            node.Grid.Cells[0, 0].Text = classInfo.Name;

            node.Grid.Cells[0, 1].Text = classInfo.Properties.Aggregate("", (current, prop) => current + $"{prop}\n");
            node.Grid.Cells[0, 2].Text = classInfo.Methods.Aggregate("", (current, method) => current + $"{method}\n");

            var hasUpdateInfo = classInfo.AddedProperties.Any() ||
                classInfo.DeletedProperties.Any() ||
                classInfo.AddedMethods.Any() ||
                classInfo.DeletedMethods.Any();

            var button = node.GetButtonById("VersionUpdateInfo");

            if (hasUpdateInfo)
            {
                button.Clicked += (sender, args) =>
                {
                    updateCallback.Invoke(classInfo);
                };
            }
            button.Visible = hasUpdateInfo;
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
