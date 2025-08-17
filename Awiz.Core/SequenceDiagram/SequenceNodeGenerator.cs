using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class SequenceNodeGenerator : ISequenceNodeGenerator
    {
        private static readonly string _uniqueUserLifelineNodeId = "13be42a7-f36a-4aa9-bc5b-a1d05d7d8e96";

        private readonly ISequenceDiagramLayoutManager _layoutManager;

        private readonly ISequenceDiagramState _state;

        private readonly ClassInfo _userClass = new ClassInfo()
        {
            Name = "User",
            Namespace = _uniqueUserLifelineNodeId
        };

        private IGraph? _graph = null;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();

        private Func<string, (int, int)> _textSizeCalculator = (text) => (0, 0);
        
        public SequenceNodeGenerator(ISequenceDiagramLayoutManager layoutManager, ISequenceDiagramState state)
        {
            _layoutManager = layoutManager;
            _state = state;
        }

        public int CurrentLifelineHeight { get; set; } = Design.SequenceLifelineHeight;

        public IDictionary<INode, ClassInfo> NodeToClassInfo => _nodeToClassInfo;

        public ClassInfo UserClass => _userClass;

        public INode UserLifeline { get; private set; } = null!;

        public bool CreateClassNode(ClassInfo classInfo)
        {
            if (!_nodeToClassInfo.Any(kvp => kvp.Value == classInfo))
            {            
                var lifelineIndex = _state.LifelineCount;
                var (header, lifeline) = CreateClassNodeInternal(classInfo, lifelineIndex);
                _nodeToClassInfo[header] = classInfo;
                _nodeToClassInfo[lifeline] = classInfo;
            }

            return _state.IsMethodCallInProgress;
        }

        public CallInfo CreateMethodCall(ClassInfo sourceClass, ClassInfo targetClass, MethodInfo calledMethod)
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            var sourceLifeline = GetLifelineByClass(sourceClass);
            var targetLifeline = GetLifelineByClass(targetClass);
            var callInfo = new CallInfo(sourceLifeline, targetLifeline, sourceClass, targetClass, calledMethod);
            _state.PushCall(callInfo);

            var methodText = GetMethodCallText(calledMethod);
            var (textWidth, _) = _textSizeCalculator(methodText);
            int requiredDistance = Math.Max(Design.SequenceClassesDistance, textWidth + Design.SequenceMethodCallPadding);

            int sourceIndex = _state.GetLifelineIndexByClass(sourceClass);
            int targetIndex = _state.GetLifelineIndexByClass(targetClass);
            int minIndex = Math.Min(sourceIndex, targetIndex);
            int maxIndex = Math.Max(sourceIndex, targetIndex);

            _layoutManager.EnsureDistance(minIndex, maxIndex, requiredDistance);

            // Update positions of all subsequent lifelines
            for (int i = minIndex + 1; i < _state.LifelineCount; i++)
            {
                var lifelineInfo = _state.Lifelines[i];
                var headerNode = _graph.Nodes.FirstOrDefault(n => n.Id == lifelineInfo.HeaderNodeId);
                var lifelineNode = _graph.Nodes.FirstOrDefault(n => n.Id == lifelineInfo.LifelineNodeId);
                int newX = _layoutManager.GetLifelineXPosition(i);
                if (headerNode != null)
                {
                    headerNode.X = newX;
                }
                if (lifelineNode != null)
                {
                    lifelineNode.X = headerNode != null ? headerNode.Width / 2 - Design.SequenceLifelineWidth / 2 + newX : newX;
                }
            }

            var edgeBuilder = _graph.AddEdge(sourceLifeline, targetLifeline);
            edgeBuilder.WithFromDockingPosition(Direction.Right, sourceLifeline.Height)
                .WithToDockingPosition(Direction.Left, sourceLifeline.Height)
                .WithEnding(Ending.OpenArrow)
                .WithText(methodText)
                .WithTextDistance(0, -13)
                .Build();

            IncreaseDiagramHeight();

            return callInfo;
        }

        public (CallInfo?, CallInfo) CreateReturnCall()
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            // Retrieve the method to return from the call stack
            var callToReturn = _state.PopCall();
            if (callToReturn == null)
            {
                throw new InvalidOperationException("Tried to return from a call but call stack is empty");
            }

            var edgeBuilder = _graph.AddEdge(callToReturn.SourceNode, callToReturn.TargetNode);
            edgeBuilder.WithFromDockingPosition(Direction.Left, callToReturn.SourceNode.Height)
                .WithToDockingPosition(Direction.Right, callToReturn.SourceNode.Height)
                .WithEnding(Ending.OpenArrow)
                .WithStyle(Style.Dashed)
                .WithText($"return {callToReturn.CalledMethod.ReturnType}")
                .WithTextDistance(0, -13)
                .Build();

            IncreaseDiagramHeight();

            var previousCall = _state.PeekCall();

            // Return true if the source of the returned call is the user class
            return (previousCall, callToReturn);
        }

        public List<ClassInfo> GetLifelineClassInfosInDiagram() => _state.Lifelines.Where(l => l.ClassInfo.Id != _userClass.Id)
                            .Select(l => l.ClassInfo)
                            .ToList();

        public void Initialize(IGraph graph)
        {
            _graph = graph;

            (var header, UserLifeline) = CreateClassNodeInternal(_userClass, 0);

            UserLifeline.SetId(_uniqueUserLifelineNodeId);
            _nodeToClassInfo[header] = _userClass;
            _nodeToClassInfo[UserLifeline] = _userClass;

            var lifelineInfo = _state.Lifelines.First();
            lifelineInfo.LifelineNodeId = _uniqueUserLifelineNodeId;
        }

        public void Restore(IGraph graph, IDictionary<string, ClassInfo> nodeIdToClassInfoMapping)
        {
            _graph = graph;

            foreach (var (nodeId, classInfo) in nodeIdToClassInfoMapping)
            {
                var node = _graph.Nodes.FirstOrDefault(p => p.Id == nodeId);
                if (classInfo != null && node != null)
                {
                    _nodeToClassInfo[node] = classInfo;

                    if (node.Id == _uniqueUserLifelineNodeId)
                    {
                        UserLifeline = node;
                    }
                }
            }
        }

        public void Save(Stream fileStream, IStorageAccess storageAccess)
        {
            storageAccess.SaveNodeIdToClassInfoMapping(NodeToClassInfo.Select(p => (p.Key.Id, p.Value)).ToDictionary(), fileStream);
            storageAccess.SaveSequenceCallstack(_state.CallStack, fileStream);
        }

        public CallInfo StartCallSequenceFromUser(ClassInfo targetClass, MethodInfo calledMethod)
        {
            return CreateMethodCall(UserClass, targetClass, calledMethod);
        }

        internal void SetTextSizeCalculator(Func<string, (int, int)> textSizeCalculator)
        {
            _textSizeCalculator = textSizeCalculator;
        }

        private static string GetMethodCallText(MethodInfo calledMethod)
        {
            return $"{calledMethod.Name}({string.Join(", ", calledMethod.Parameters.Select(p => p.Name))})";
        }

        private (INode header, INode lifeline) CreateClassNodeInternal(ClassInfo classInfo, int lifelineIndex)
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            var nodeBuilder = _graph.AddNode("SequenceHeader");
            nodeBuilder.
                WithAutoWidth().
                WithText(0, 0, classInfo.Name).
                WithHeight(Design.SequenceHeaderHeight).
                WithPos(lifelineIndex == 0 ? 0 : _layoutManager.GetLifelineXPosition(lifelineIndex), 0);
            var header = nodeBuilder.Build();

            nodeBuilder = _graph.AddNode("SequenceLifeline");
            nodeBuilder.
                WithSize(Design.SequenceLifelineWidth, CurrentLifelineHeight).
                WithPos(header.Width / 2 - Design.SequenceLifelineWidth / 2 + (lifelineIndex == 0 ? 0 : _layoutManager.GetLifelineXPosition(lifelineIndex)), Design.SequenceHeaderHeight);
            var lifeline = nodeBuilder.Build();

            _state.AddLifeline(classInfo, header.Id, lifeline.Id);

            return (header, lifeline);
        }

        private INode GetLifelineByClass(ClassInfo targetClass)
        {
            var lifeline = _state.Lifelines.FirstOrDefault(l => l.ClassInfo.Id == targetClass.Id);

            if (lifeline == null)
            {
                throw new InvalidOperationException($"Lifeline for class {targetClass.Name} not found in the sequence diagram.");
            }

            return _nodeToClassInfo.FirstOrDefault(kvp => kvp.Key.Id == lifeline.LifelineNodeId).Key ?? throw new InvalidOperationException($"Node for class {targetClass.Name} not found in the sequence diagram.");
        }

        private void IncreaseDiagramHeight()
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            CurrentLifelineHeight += Design.SequenceLifelineHeight;

            foreach (var lifelineInfo in _state.Lifelines)
            {
                var lifelineNode = _graph.Nodes.FirstOrDefault(n => n.Id == lifelineInfo.LifelineNodeId);
                if (lifelineNode != null)
                {
                    lifelineNode.Height = CurrentLifelineHeight;
                }
            }

            if (UserLifeline == null)
            {
                throw new InvalidOperationException("User lifeline is not initialized");
            }
            UserLifeline.Height = CurrentLifelineHeight;
        }
    }
}
