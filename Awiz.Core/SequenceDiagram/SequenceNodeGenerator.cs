using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class SequenceNodeGenerator : ISequenceNodeGenerator
    {
        private int _classCounter = 0;

        private int _currentLifelineHeight = Design.SequenceLifelineHeight;

        private IDictionary<ClassInfo, INode> _classToLifelineNodeMapping = new Dictionary<ClassInfo, INode>();

        internal ISourceCode? SourceCode { private get; set; }

        public (INode, INode) CreateClassNode(IGraph graph, ClassInfo classInfo)
        {
            if (SourceCode == null)
            {
                throw new InvalidOperationException("SourceCode is not set");
            }

            var head = graph.AddNode("SequenceHeader");
            head.Grid.Cells[0, 0].Text = classInfo.Name;
            head.Width = Design.SequenceHeaderWidth;
            head.Height = Design.SequenceHeaderHeight;
            head.X = _classCounter * Design.SequenceClassesDistance;
            head.Y = 0;

            var lifeline = graph.AddNode("SequenceLifeline");
            lifeline.Width = Design.SequenceLifelineWidth;
            lifeline.Height = _currentLifelineHeight;
            lifeline.X = head.Width / 2 - lifeline.Width / 2 +  _classCounter * Design.SequenceClassesDistance;
            lifeline.Y = Design.SequenceHeaderHeight;

            _classToLifelineNodeMapping[classInfo] = lifeline;

            _classCounter++;

            return (head, lifeline);
        }

        public INode CreateMethodCall(IGraph graph, ClassInfo sourceLifeline, ClassInfo targetLifeline, MethodInfo methodInfo)
        {
            if (!_classToLifelineNodeMapping.ContainsKey(sourceLifeline))
            {
                throw new InvalidOperationException($"Source lifeline {sourceLifeline.Name} was not added");
            }
            if (!_classToLifelineNodeMapping.ContainsKey(targetLifeline))
            {
                throw new InvalidOperationException($"Target lifeline {targetLifeline.Name} was not added");
            }

            var sourceLifelineNode = _classToLifelineNodeMapping[sourceLifeline];
            var targetLifelineNode = _classToLifelineNodeMapping[targetLifeline];
            var edgeBuilder = graph.AddEdge(sourceLifelineNode, targetLifelineNode);
            edgeBuilder.WithFromDockingPosition(Direction.Right, sourceLifelineNode.Height)
                .WithToDockingPosition(Direction.Left, sourceLifelineNode.Height)
                .WithEnding(Ending.OpenArrow)
                .WithText(methodInfo.ToString())
                .Build();

            _currentLifelineHeight += Design.SequenceLifelineHeight;
            
            foreach (var lifeline in _classToLifelineNodeMapping.Values)
            {
                lifeline.Height = _currentLifelineHeight;
            }

            return targetLifelineNode;
        }
    }
}
