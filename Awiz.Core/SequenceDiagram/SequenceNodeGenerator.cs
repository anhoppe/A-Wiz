using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class SequenceNodeGenerator : ISequenceNodeGenerator
    {
        private int _lifelineCounter = 0;

        internal ISourceCode? SourceCode { private get; set; }

        public (INode, INode) CreateClassNode(IGraph graph, ClassInfo classInfo, int lifelineHeight)
        {
            if (SourceCode == null)
            {
                throw new InvalidOperationException("SourceCode is not set");
            }

            var head = graph.AddNode("SequenceHeader");
            head.Grid.Cells[0, 0].Text = classInfo.Name;
            head.Width = Design.SequenceHeaderWidth;
            head.Height = Design.SequenceHeaderHeight;
            head.X = _lifelineCounter * Design.SequenceClassesDistance;
            head.Y = 0;

            var lifeline = graph.AddNode("SequenceLifeline");
            lifeline.SetId($"{ISequenceNodeGenerator.LifelineId}:{lifeline.Id}");
            lifeline.Width = Design.SequenceLifelineWidth;
            lifeline.Height = lifelineHeight;
            lifeline.X = head.Width / 2 - lifeline.Width / 2 + _lifelineCounter * Design.SequenceClassesDistance;
            lifeline.Y = Design.SequenceHeaderHeight;

            _lifelineCounter++;

            return (head, lifeline);
        }

        public void CreateMethodCall(IGraph graph, CallInfo callInfo)
        {
            if (callInfo.SourceNode == null || callInfo.TargetNode == null)
            {
                throw new InvalidOperationException("Source or target class is not set in the call info");
            }

            var edgeBuilder = graph.AddEdge(callInfo.SourceNode, callInfo.TargetNode);
            edgeBuilder.WithFromDockingPosition(Direction.Right, callInfo.SourceNode.Height)
                .WithToDockingPosition(Direction.Left, callInfo.SourceNode.Height)
                .WithEnding(Ending.OpenArrow)
                .WithText(callInfo.CalledMethod.ToString())
                .Build();
        }

        public void CreateReturnCall(IGraph graph, CallInfo callInfo)
        {
            if (callInfo.SourceNode == null || callInfo.TargetNode == null)
            {
                throw new InvalidOperationException("Source or target class is not set in the call info");
            }

            var edgeBuilder = graph.AddEdge(callInfo.SourceNode, callInfo.TargetNode);
            edgeBuilder.WithFromDockingPosition(Direction.Left, callInfo.SourceNode.Height)
                .WithToDockingPosition(Direction.Right, callInfo.SourceNode.Height)
                .WithEnding(Ending.OpenArrow)
                .WithStyle(Style.Dashed)
                .Build();
        }

        public void SetLifelineCounter(int lifelineCounter)
        {
            _lifelineCounter = lifelineCounter;
        }
    }
}
