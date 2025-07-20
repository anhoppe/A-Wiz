using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System.Globalization;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    class ArchitectureSequenceView : ArchitectureView
    {
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

            SequenceNodeGenerator.CreateClassNode(classInfo);

            // Add the information to the interaction behavior of the user lifeline only if the call stack is empty
            // => button to start call sequence is available
            if (CallInfo.Count == 0)
            {
                InteractionBehavior.UpdateUserInitiaiteCallSequence(SequenceNodeGenerator.UserLifeline,
                    SequenceNodeGenerator.GetLifelineClassInfosInDiagram(),
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

            var callInfo = SequenceNodeGenerator.CreateMethodCall(sourceClass, targetClass, methodInfo);
            CallInfo.Push(callInfo);

            InteractionBehavior.UpdateButtons(callInfo , (targetClass, calledMethod) => AddMethodCall(sourceClass, targetClass, calledMethod), AddMethodCallReturn);
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

            SequenceNodeGenerator.Initialize(Graph);

            //if (!SequenceNodeGenerator.NodeToClassInfo.Any())
            //{
                InteractionBehavior.UpdateUserInitiaiteCallSequence(SequenceNodeGenerator.UserLifeline,
                    SequenceNodeGenerator.GetLifelineClassInfosInDiagram(),
                    StartCallSequenceFromUser);
            //}
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

                    SequenceNodeGenerator.Restore(mapping);
                }
            }

            // Load the current call stack
            storagePath = Path.Combine(RepoPath, $".wiz\\storage\\sequence\\state\\{Name}.yaml");
            if (FileSystem.Exists(storagePath))
            {
                using (var fileStream = FileSystem.OpenRead(storagePath))
                {
                    CallInfo = StorageAccess.LoadSequenceCallstack(fileStream, SequenceNodeGenerator.NodeToClassInfo);
                }
            }

            // Update the interaction buttons and lifeline counter
            if (SequenceNodeGenerator.UserLifeline != null)
            {
                var classInfos = SequenceNodeGenerator.GetLifelineClassInfosInDiagram();

                SequenceNodeGenerator.SetLifelineCounter(classInfos.Count + 1); // +1 for the user lifeline

                if (!CallInfo.Any())
                {
                    InteractionBehavior.UpdateUserInitiaiteCallSequence(SequenceNodeGenerator.UserLifeline,
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

                    var sourceClassForNextCall = SequenceNodeGenerator.NodeToClassInfo[callInfo.TargetNode];

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
                        SequenceNodeGenerator.CurrentLifelineHeight = deserializer.Deserialize<int>(yaml);
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

            if (SequenceNodeGenerator == null)
            {
                throw new NullReferenceException("SequenceNodeGenerator is not set");
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
                StorageAccess.SaveNodeIdToClassInfoMapping(SequenceNodeGenerator.NodeToClassInfo.Select(p => (p.Key.Id, p.Value)).ToDictionary(), fileStream);
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
                var yaml = serializer.Serialize(SequenceNodeGenerator.CurrentLifelineHeight);

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

            var callToReturnFrom = CallInfo.Pop();

            MethodInfo? callToPrevious = null;

            if (CallInfo.Count > 0)
            {
                callToPrevious = CallInfo.Peek().CalledMethod;
            }

            if (callToReturnFrom.SourceNode == null || callToReturnFrom.TargetNode == null)
            {
                throw new InvalidOperationException("Source or target class is not set in the call info");
            }

            CallInfo callInfo;

            if (callToPrevious != null)
            {
                callInfo = new CallInfo(callToReturnFrom.TargetNode, callToReturnFrom.SourceNode, callToPrevious);
            }
            else
            {
                callInfo = new CallInfo(callToReturnFrom.TargetNode, callToReturnFrom.SourceNode);
            }

            bool returnedToUserLifeline = SequenceNodeGenerator.CreateReturnCall(callInfo);
            InteractionBehavior.RemoveButtons(callToReturnFrom);

            if (!returnedToUserLifeline)
            {
                var prevMethodCallInfo = SequenceNodeGenerator.NodeToClassInfo.FirstOrDefault(kvp => kvp.Key == callInfo.TargetNode).Value;

                if (prevMethodCallInfo == null)
                {
                    throw new InvalidOperationException("Previous method call info not found for the target node");
                }

                InteractionBehavior.UpdateButtons(callInfo , (nextTargetClass, calledMethod) => 
                    AddMethodCall(prevMethodCallInfo, nextTargetClass, calledMethod), AddMethodCallReturn);
            }
            else
            {
                if (SequenceNodeGenerator.UserLifeline == null)
                {
                    throw new InvalidOperationException("User lifeline is not initialized");
                }

                InteractionBehavior.UpdateUserInitiaiteCallSequence(SequenceNodeGenerator.UserLifeline,
                    SequenceNodeGenerator.GetLifelineClassInfosInDiagram(),
                    StartCallSequenceFromUser);
            }
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
            if (SequenceNodeGenerator.UserLifeline == null)
            {
                throw new NullReferenceException("User lifeline is not initialized");
            }

            var callInfo = SequenceNodeGenerator.StartCallSequenceFromUser(targetClass, calledMethod);
            CallInfo.Push(callInfo);

            InteractionBehavior.UpdateButtons(callInfo , (nextTargetClass, calledMethod) => AddMethodCall(targetClass, nextTargetClass, calledMethod), AddMethodCallReturn);
        }
    }
}
