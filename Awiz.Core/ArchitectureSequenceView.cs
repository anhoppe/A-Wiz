using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;

namespace Awiz.Core
{
    class ArchitectureSequenceView : ArchitectureView
    {
        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();

        public override ArchitectureViewType Type => ArchitectureViewType.Sequence;

        internal IInteractionBehavior? InteractionBehavior { private get; set; }

        internal ISequenceNodeGenerator? SequenceNodeGenerator { private get; set; }

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

            (var headerNode, var lifelineNode) = SequenceNodeGenerator.CreateClassNode(Graph, classInfo);

            _nodeToClassInfo[headerNode] = classInfo;
            _nodeToClassInfo[lifelineNode] = classInfo;

            InteractionBehavior.AttachButtonBehavior(this, classInfo, lifelineNode);
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

            var targetClassLifelineNode = SequenceNodeGenerator.CreateMethodCall(Graph, sourceClass, targetClass, methodInfo);

            InteractionBehavior.UpdateAddMethodButtonBehavior(this, targetClass, methodInfo, targetClassLifelineNode);
        }

        public override void AddUseCaseNode(INode node)
        {
            throw new NotImplementedException();
        }

        public override void Load(IVersionUpdater versionUpdater)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        protected override void OnNodeRemoved(INode node)
        {
            throw new NotImplementedException();
        }
    }
}
