using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    class ArchitectureSequenceView : ArchitectureView
    {
        private static readonly string _unique_user_lifeline_node_id = "13be42a7-f36a-4aa9-bc5b-a1d05d7d8e96";

        private int _currentLifelineHeight = Design.SequenceLifelineHeight;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();

        private ClassInfo _userClass = new ClassInfo()
        {
            Name = "User",
            Namespace = _unique_user_lifeline_node_id
        };

        private INode? _userLifeline = null;

        public Stack<CallInfo> CallInfo { get; private set; } = new Stack<CallInfo>();
        
        public override ArchitectureViewType Type => ArchitectureViewType.Sequence;

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        internal IInteractionBehavior? InteractionBehavior { private get; set; }

        internal ISequenceNodeGenerator? SequenceNodeGenerator { private get; set; }

        internal ISerializer Serializer { get; set; } = new YamlSerializer();

        public override void AddBaseClassNode(ClassInfo derivedClassInfo)
        {
            throw new NotImplementedException();
        }

        public override void AddClassNode(ClassInfo classInfo)
        {
            if (_nodeToClassInfo.ContainsValue(classInfo))
            {
                return;
            }

            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }

            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior is not set");
            }

            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }

            if (_userLifeline == null)
            {
                throw new NullReferenceException("User lifeline is not initialized");
            }

            (var headerNode, var lifelineNode) = SequenceNodeGenerator.CreateClassNode(Graph, classInfo, _currentLifelineHeight);

            _nodeToClassInfo[headerNode] = classInfo;
            _nodeToClassInfo[lifelineNode] = classInfo;

            // Add the information to the interaction behavior of the user lifeline only if the call stack is empty => button to start call sequence is available
            if (CallInfo.Count == 0)
            {
                InteractionBehavior.UpdateUserInitiaiteCallSequence(_userLifeline,
                    GetLifelineClassInfosInDiagram(),
                    StartCallSequenceFromUser);

            }
        }

        public override void AddMethodCall(ClassInfo sourceClass, ClassInfo targetClass, MethodInfo methodInfo)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }

            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior is not set");
            }
            
            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }

            AddClassNode(targetClass);

            var sourceNode = GetLifelineNodeForClass(sourceClass);
            var targetNode = GetLifelineNodeForClass(targetClass);

            var callInfo = new CallInfo(sourceNode, targetNode, methodInfo);
            CallInfo.Push(callInfo);

            SequenceNodeGenerator.CreateMethodCall(Graph, callInfo);
            InteractionBehavior.UpdateButtons(callInfo , (targetClass, calledMethod) => AddMethodCall(sourceClass, targetClass, calledMethod), AddMethodCallReturn);

            IncreaseDiagramHeight();
        }

        public override void AddUseCaseNode(INode node)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior is not set");
            }
            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }

            if (!_nodeToClassInfo.Any())
            {
                (var header, _userLifeline) = SequenceNodeGenerator.CreateClassNode(Graph, _userClass, _currentLifelineHeight);
                InteractionBehavior.UpdateUserInitiaiteCallSequence(_userLifeline,
                    GetLifelineClassInfosInDiagram(),
                    StartCallSequenceFromUser);

                _userLifeline.SetId(_unique_user_lifeline_node_id);
                _nodeToClassInfo[header] = _userClass;
                _nodeToClassInfo[_userLifeline] = _userClass;
            }
        }

        public override void Load(IVersionUpdater versionUpdater)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior is not set");
            }
            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }

            // Load node mapping to classes
            var storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\mapping\\{Name}.yaml");

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    var mapping = StorageAccess.LoadNodeIdToClassInfoMapping(stream);

                    foreach (var (nodeId, classInfo) in mapping)
                    {
                        var node = Graph.Nodes.FirstOrDefault(p => p.Id == nodeId);
                        if (classInfo != null && node != null)
                        {
                            _nodeToClassInfo[node] = classInfo;

                            if (node.Id == _unique_user_lifeline_node_id)
                            {
                                _userLifeline = node;
                            }
                        }
                    }
                }
            }

            // Load the current call stack
            storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\state\\{Name}.yaml");
            if (FileSystem.Exists(storagePath))
            {
                using (var fileStream = FileSystem.OpenRead(storagePath))
                {
                    CallInfo = StorageAccess.LoadSequenceCallstack(fileStream, _nodeToClassInfo);
                }
            }

            // Update the interaction buttons and lifeline counter
            if (_userLifeline != null)
            {
                var classInfos = GetLifelineClassInfosInDiagram();

                SequenceNodeGenerator.SetLifelineCounter(classInfos.Count + 1); // +1 for the user lifeline

                if (!CallInfo.Any())
                {
                    InteractionBehavior.UpdateUserInitiaiteCallSequence(_userLifeline,
                        classInfos,
                        StartCallSequenceFromUser);
                }
                else 
                {
                    var callInfo = CallInfo.Peek();
                    if (callInfo.TargetNode == null)
                    {
                        throw new NullReferenceException("CallInfo has no valid TargetNode when Loading");
                    }

                    var sourceClassForNextCall = _nodeToClassInfo[callInfo.TargetNode];

                    InteractionBehavior.UpdateButtons(callInfo,
                        (nextTargetClass, calledMethod) => AddMethodCall(sourceClassForNextCall, nextTargetClass, calledMethod),
                        AddMethodCallReturn);
                }
            }

            // Save the current lifeline height.
            // ToDo: Needs to be in the serializer when the method call serialization is implemented
            storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\{Name}_height.yaml");
            if (FileSystem.Exists(storagePath))
            {
                using (var fileStream = FileSystem.OpenRead(storagePath))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var yaml = reader.ReadToEnd();
                        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.PascalCaseNamingConvention.Instance)
                            .Build();
                        _currentLifelineHeight = deserializer.Deserialize<int>(yaml);
                    }
                }
            }
        }

        public override void Save()
        {
            // Save the graph
            if (Graph == null)
            {
                throw new InvalidOperationException("No class diagram loaded");
            }

            var graphStoragePath = Path.Combine(RepoPath, $".wiz\\sequence\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(graphStoragePath))
            {
                Serializer.Serialize(fileStream, Graph);
            }

            // Save the mapping to the classes
            var storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\mapping\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(storagePath))
            {
                StorageAccess.SaveNodeIdToClassInfoMapping(_nodeToClassInfo.Select(p => (p.Key.Id, p.Value)).ToDictionary(), fileStream);
            }

            // Save the current call stack
            storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\state\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(storagePath))
            {
                StorageAccess.SaveSequenceCallstack(CallInfo, fileStream);
            }

            // Save the current lifeline height.
            // ToDo: Needs to be in the serializer when the method call serialization is implemented
            storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\{Name}_height.yaml");
            using (var fileStream = FileSystem.Create(storagePath))
            {
                var serializer = new YamlDotNet.Serialization.SerializerBuilder()
                    .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.PascalCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(_currentLifelineHeight);

                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(yaml);
                }
            }
        }

        protected override void OnNodeRemoved(INode node)
        {
            throw new NotImplementedException();
        }

        private void AddMethodCallReturn()
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior not set");
            }
            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }
            if (CallInfo.Count == 0)
            {
                throw new InvalidOperationException("Cannot return from method call because no call was pushed on the call stack");
            }

            var callToReturn = CallInfo.Pop();

            MethodInfo? callToPrevious = null;

            if (CallInfo.Count > 0)
            {
                callToPrevious = CallInfo.Peek().CalledMethod;
            }

            if (callToReturn.SourceNode == null || callToReturn.TargetNode == null)
            {
                throw new InvalidOperationException("Source or target class is not set in the call info");
            }

            CallInfo callInfo;

            if (callToPrevious != null)
            {
                callInfo = new CallInfo(callToReturn.TargetNode, callToReturn.SourceNode, callToPrevious);
            }
            else
            {
                callInfo = new CallInfo(callToReturn.TargetNode, callToReturn.SourceNode);
            }

            SequenceNodeGenerator.CreateReturnCall(Graph, callInfo);
            InteractionBehavior.RemoveButtons(callToReturn);

            if (callInfo.TargetNode?.Id != _unique_user_lifeline_node_id)
            {
                var prevMethodCallInfo = _nodeToClassInfo.FirstOrDefault(kvp => kvp.Key == callInfo.TargetNode).Value;

                if (prevMethodCallInfo == null)
                {
                    throw new InvalidOperationException("Previous method call info not found for the target node");
                }

                InteractionBehavior.UpdateButtons(callInfo , (nextTargetClass, calledMethod) => 
                    AddMethodCall(prevMethodCallInfo, nextTargetClass, calledMethod), AddMethodCallReturn);
            }
            else
            {
                if (_userLifeline == null)
                {
                    throw new InvalidOperationException("User lifeline is not initialized");
                }

                InteractionBehavior.UpdateUserInitiaiteCallSequence(_userLifeline,
                    GetLifelineClassInfosInDiagram(),
                    StartCallSequenceFromUser);
            }

            IncreaseDiagramHeight();
        }

        private INode GetLifelineNodeForClass(ClassInfo classInfo)
        {
            var lifelineNode = _nodeToClassInfo.Where(kvp =>
            {
                var ids = kvp.Key.Id.Split(":");
                return kvp.Key.Id.StartsWith(ISequenceNodeGenerator.LifelineId);
            }).FirstOrDefault(kvp => kvp.Value == classInfo).Key;

            if (lifelineNode == null)
            {
                throw new InvalidOperationException($"Lifeline node for class {classInfo.Name} not found in the sequence diagram");
            }

            return lifelineNode;
        }

        private List<ClassInfo> GetLifelineClassInfosInDiagram()
        {
            var lifelineNodes = _nodeToClassInfo.Where(kvp =>
                {
                    var ids = kvp.Key.Id.Split(":");
                    return kvp.Key.Id.StartsWith(ISequenceNodeGenerator.LifelineId);
                }).ToDictionary();

            return lifelineNodes.Values.Where(classInfo => classInfo != _userClass).ToList();
        }

        private void IncreaseDiagramHeight()
        {
            _currentLifelineHeight += Design.SequenceLifelineHeight;

            foreach (var kvp in _nodeToClassInfo)
            {
                var node = kvp.Key;
                if (node.Id.StartsWith(ISequenceNodeGenerator.LifelineId))
                {
                    node.Height = _currentLifelineHeight;
                }
            }

            if (_userLifeline == null)
            {
                throw new InvalidOperationException("User lifeline is not initialized");
            }
            _userLifeline.Height = _currentLifelineHeight;
        }

        private void StartCallSequenceFromUser(ClassInfo targetClass, MethodInfo calledMethod)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (InteractionBehavior == null)
            {
                throw new NullReferenceException("InteractionBehavior is not set");
            }
            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
            }
            if (_userLifeline == null)
            {
                throw new NullReferenceException("User lifeline is not initialized");
            }

            var callInfo = new CallInfo(_userLifeline, GetLifelineNodeForClass(targetClass), calledMethod);
            CallInfo.Push(callInfo);
            SequenceNodeGenerator.CreateMethodCall(Graph, callInfo);

            InteractionBehavior.UpdateButtons(callInfo , (nextTargetClass, calledMethod) => AddMethodCall(targetClass, nextTargetClass, calledMethod), AddMethodCallReturn);

            IncreaseDiagramHeight();
        }
    }
}
