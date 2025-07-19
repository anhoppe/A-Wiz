using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class SequenceNodeGenerator : ISequenceNodeGenerator
    {
        private static readonly string _lifelineId = "8dbe0ba5-ef9a-4f1d-a4a2-fc7e16c1286d";
        
        private static readonly string _uniqueUserLifelineNodeId = "13be42a7-f36a-4aa9-bc5b-a1d05d7d8e96";

        private readonly ClassInfo _userClass = new ClassInfo()
        {
            Name = "User",
            Namespace = _uniqueUserLifelineNodeId
        };

        private int _lifelineCounter = 0;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();

        private INode? _userLifeline = null;

        public int CurrentLifelineHeight { get; set; } = Design.SequenceLifelineHeight;

        public string LifelineId => _lifelineId;

        public IDictionary<INode, ClassInfo> NodeToClassInfo => _nodeToClassInfo;

        public ClassInfo UserClass => _userClass;
        public INode UserLifeline => _userLifeline ?? throw new InvalidOperationException("SequenceNodeGenerator was not initialized but user lifeline was requested");

        internal ISourceCode? SourceCode { private get; set; }
        public void CreateClassNode(IGraph graph, ClassInfo classInfo)
        {
            var (header, lifeline) = CreateClassNodeInternal(graph, classInfo);

            _nodeToClassInfo[header] = classInfo;
            _nodeToClassInfo[lifeline] = classInfo;
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

            IncreaseDiagramHeight();
        }

        public bool CreateReturnCall(IGraph graph, CallInfo callInfo)
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

            IncreaseDiagramHeight();

            return callInfo.TargetNode.Id == _uniqueUserLifelineNodeId;
        }

        public void Initialize(IGraph graph)
        {
            (var header, _userLifeline) = CreateClassNodeInternal(graph, _userClass);

            _userLifeline.SetId(_uniqueUserLifelineNodeId);
            _nodeToClassInfo[header] = _userClass;
            _nodeToClassInfo[_userLifeline] = _userClass;
        }

        public void Restore(IGraph graph, IDictionary<string, ClassInfo> nodeIdToClassInfoMapping)
        {
            foreach (var (nodeId, classInfo) in nodeIdToClassInfoMapping)
            {
                var node = graph.Nodes.FirstOrDefault(p => p.Id == nodeId);
                if (classInfo != null && node != null)
                {
                    _nodeToClassInfo[node] = classInfo;

                    if (node.Id == _uniqueUserLifelineNodeId)
                    {
                        _userLifeline = node;
                    }
                }
            }
        }

        public void SetLifelineCounter(int lifelineCounter)
        {
            _lifelineCounter = lifelineCounter;
        }

        private static string GetMethodCallText(MethodInfo calledMethod)
        {
            return $"{calledMethod.Name}({string.Join(", ", calledMethod.Parameters.Select(p => p.Name))})";
        }

        private (INode header, INode lifeline) CreateClassNodeInternal(IGraph graph, ClassInfo classInfo)
        {
            var nodeBuilder = graph.AddNode("SequenceHeader");

            nodeBuilder.
                WithAutoWidth().
                WithText(0, 0, classInfo.Name).
                WithHeight(Design.SequenceHeaderHeight).
                WithPos(_lifelineCounter * Design.SequenceClassesDistance, 0);

            var header = nodeBuilder.Build();
            nodeBuilder = graph.AddNode("SequenceLifeline");
            nodeBuilder.
                WithSize(Design.SequenceLifelineWidth, CurrentLifelineHeight).
                WithPos(header.Width / 2 - Design.SequenceLifelineWidth / 2 + _lifelineCounter * Design.SequenceClassesDistance, Design.SequenceHeaderHeight);

            var lifeline = nodeBuilder.Build();
            lifeline.SetId($"{_lifelineId}:{lifeline.Id}");

            _lifelineCounter++;

            return (header, lifeline);
        }
        private void IncreaseDiagramHeight()
        {
            CurrentLifelineHeight += Design.SequenceLifelineHeight;

            foreach (var kvp in _nodeToClassInfo)
            {
                var node = kvp.Key;
                if (node.Id.StartsWith(_lifelineId))
                {
                    node.Height = CurrentLifelineHeight;
                }
            }

            if (_userLifeline == null)
            {
                throw new InvalidOperationException("User lifeline is not initialized");
            }
            _userLifeline.Height = CurrentLifelineHeight;
        }
    }
}
