using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class SequenceNodeGenerator : ISequenceNodeGenerator
    {
        private static readonly string _uniqueUserLifelineNodeId = "13be42a7-f36a-4aa9-bc5b-a1d05d7d8e96";

        private readonly ClassInfo _userClass = new ClassInfo()
        {
            Name = "User",
            Namespace = _uniqueUserLifelineNodeId
        };

        private IGraph? _graph = null;

        private int _lifelineCounter = 0;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();

        private Func<string, (int, int)> _textSizeCalculator = (text) => (0, 0);

        public int CurrentLifelineHeight { get; set; } = Design.SequenceLifelineHeight;

        public List<SequenceLifelineInfo> Lifelines { get; } = new();

        public IDictionary<INode, ClassInfo> NodeToClassInfo => _nodeToClassInfo;

        public ClassInfo UserClass => _userClass;

        public INode UserLifeline { get; private set; } = null!;

        internal ISourceCode? SourceCode { private get; set; }

        public void CreateClassNode(ClassInfo classInfo)
        {
            var (header, lifeline) = CreateClassNodeInternal(classInfo);

            _nodeToClassInfo[header] = classInfo;
            _nodeToClassInfo[lifeline] = classInfo;
        }

        public CallInfo CreateMethodCall(ClassInfo sourceClass, ClassInfo targetClass, MethodInfo calledMethod)
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            var sourceLifeline = GetLifelineByClass(sourceClass);
            var targetLifeline = GetLifelineByClass(targetClass);

            var callInfo = new CallInfo(sourceLifeline, targetLifeline, calledMethod);

            var methodText = GetMethodCallText(calledMethod);
            var (textWidth, _) = _textSizeCalculator(methodText);
            int requiredDistance = Math.Max(Design.SequenceClassesDistance, textWidth + Design.SequenceMethodCallPadding); // Add some padding

            // Find the lifeline index for the source node
            int sourceIndex = Lifelines.FindIndex(l => l.LifelineNodeId == sourceLifeline.Id);
            int targetIndex = Lifelines.FindIndex(l => l.LifelineNodeId == targetLifeline.Id);
            int minIndex = Math.Min(sourceIndex, targetIndex);
            int maxIndex = Math.Max(sourceIndex, targetIndex);

            // Calculate available space between minIndex and maxIndex
            int availableSpace = 0;
            for (int i = minIndex; i < maxIndex; i++)
            {
                availableSpace += Lifelines[i].DistanceToNextLifeline;
            }

            if (requiredDistance > availableSpace)
            {
                int extra = requiredDistance - availableSpace;
                // Distribute extra space to the first gap (could be improved to distribute evenly)
                Lifelines[minIndex].DistanceToNextLifeline += extra;

                // Update positions of all subsequent lifelines
                for (int i = minIndex + 1; i < Lifelines.Count; i++)
                {
                    var lifelineInfo = Lifelines[i];
                    var headerNode = _graph.Nodes.FirstOrDefault(n => n.Id == lifelineInfo.HeaderNodeId);
                    var lifelineNode = _graph.Nodes.FirstOrDefault(n => n.Id == lifelineInfo.LifelineNodeId);
                    int newX = GetLifelineXPosition(i);
                    if (headerNode != null)
                    {
                        headerNode.X = newX;
                    }
                    if (lifelineNode != null)
                    {
                        lifelineNode.X = headerNode != null ? headerNode.Width / 2 - Design.SequenceLifelineWidth / 2 + newX : newX;
                    }
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

        public bool CreateReturnCall(CallInfo callInfo)
        {
            if (callInfo.SourceNode == null || callInfo.TargetNode == null)
            {
                throw new InvalidOperationException("Source or target class is not set in the call info");
            }

            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            var edgeBuilder = _graph.AddEdge(callInfo.SourceNode, callInfo.TargetNode);
            edgeBuilder.WithFromDockingPosition(Direction.Left, callInfo.SourceNode.Height)
                .WithToDockingPosition(Direction.Right, callInfo.SourceNode.Height)
                .WithEnding(Ending.OpenArrow)
                .WithStyle(Style.Dashed)
                .Build();

            IncreaseDiagramHeight();

            return callInfo.TargetNode.Id == _uniqueUserLifelineNodeId;
        }

        public List<ClassInfo> GetLifelineClassInfosInDiagram() => Lifelines.Where(l => l.ClassInfo.Id != _userClass.Id)
                            .Select(l => l.ClassInfo)
                            .ToList();

        public void Initialize(IGraph graph)
        {
            _graph = graph;

            (var header, UserLifeline) = CreateClassNodeInternal(_userClass);

            UserLifeline.SetId(_uniqueUserLifelineNodeId);
            _nodeToClassInfo[header] = _userClass;
            _nodeToClassInfo[UserLifeline] = _userClass;

            var lifelineInfo = Lifelines.First();
            lifelineInfo.LifelineNodeId = _uniqueUserLifelineNodeId;
        }

        public void Restore(IDictionary<string, ClassInfo> nodeIdToClassInfoMapping)
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

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

        public void SetLifelineCounter(int lifelineCounter)
        {
            _lifelineCounter = lifelineCounter;
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

        private (INode header, INode lifeline) CreateClassNodeInternal(ClassInfo classInfo)
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
                WithPos(_lifelineCounter == 0 ? 0 : GetLifelineXPosition(_lifelineCounter), 0);

            var header = nodeBuilder.Build();
            nodeBuilder = _graph.AddNode("SequenceLifeline");
            nodeBuilder.
                WithSize(Design.SequenceLifelineWidth, CurrentLifelineHeight).
                WithPos(header.Width / 2 - Design.SequenceLifelineWidth / 2 + (_lifelineCounter == 0 ? 0 : GetLifelineXPosition(_lifelineCounter)), Design.SequenceHeaderHeight);

            var lifeline = nodeBuilder.Build();

            // Set the previous lifeline's distance to next
            if (Lifelines.Count > 0)
            {
                Lifelines[Lifelines.Count - 1].DistanceToNextLifeline = Design.SequenceClassesDistance;
            }

            Lifelines.Add(new SequenceLifelineInfo
            {
                HeaderNodeId = header.Id,
                LifelineNodeId = lifeline.Id,
                ClassInfo = classInfo
                // DistanceToNextLifeline is set to default by the POCO
            });

            _lifelineCounter++;

            return (header, lifeline);
        }

        private INode GetLifelineByClass(ClassInfo targetClass)
        {
            var lifeline = Lifelines.FirstOrDefault(l => l.ClassInfo.Id == targetClass.Id);

            if (lifeline == null)
            {
                throw new InvalidOperationException($"Lifeline for class {targetClass.Name} not found in the sequence diagram.");
            }

            return _nodeToClassInfo.FirstOrDefault(kvp => kvp.Key.Id == lifeline.LifelineNodeId).Key ?? throw new InvalidOperationException($"Node for class {targetClass.Name} not found in the sequence diagram.");
        }

        private int GetLifelineXPosition(int lifelineIndex)
        {
            int x = 0;
            for (int i = 0; i < lifelineIndex; i++)
            {
                x += Lifelines[i].DistanceToNextLifeline;
            }

            return x;
        }

        private void IncreaseDiagramHeight()
        {
            if (_graph == null)
            {
                throw new InvalidOperationException("Graph is not initialized");
            }

            CurrentLifelineHeight += Design.SequenceLifelineHeight;

            foreach (var lifelineInfo in Lifelines)
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
