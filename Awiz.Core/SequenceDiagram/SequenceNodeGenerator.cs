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

            var nodeBuilder = graph.AddNode("SequenceHeader");

            nodeBuilder.
                WithAutoWidth().
                WithText(0, 0, classInfo.Name).
                WithHeight(Design.SequenceHeaderHeight).
                WithPos(_lifelineCounter * Design.SequenceClassesDistance, 0);

            var header = nodeBuilder.Build();

            nodeBuilder = graph.AddNode("SequenceLifeline");
            nodeBuilder.
                WithSize(Design.SequenceLifelineWidth, lifelineHeight).
                WithPos(header.Width / 2 - Design.SequenceLifelineWidth / 2 + _lifelineCounter * Design.SequenceClassesDistance, Design.SequenceHeaderHeight);

            var lifeline = nodeBuilder.Build();
            lifeline.SetId($"{ISequenceNodeGenerator.LifelineId}:{lifeline.Id}");

            _lifelineCounter++;

            return (header, lifeline);
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
                .WithText(GetMethodCallText(callInfo.CalledMethod))
                .WithTextDistance(0, -13)
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

        private static string GetMethodCallText(MethodInfo calledMethod)
        {
            return $"{calledMethod.Name}({string.Join(", ", calledMethod.Parameters.Select(p => p.Name))})";
        }
    }
}
