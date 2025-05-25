using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ArchitectureSequenceViewTest
    {
        private Mock<IGraph> _graphMock = new();

        private Mock<INode> _headerNodeMock = new();

        private Mock<IInteractionBehavior> _interactionBehaviorMock = new();

        private Mock<INode> _lifelineNodeMock = new();

        private Mock<ISequenceNodeGenerator> _sequenceNodeGeneratorMock = new();

        private ArchitectureSequenceView _sut = new();

        [SetUp]
        public void SetUp()
        {
            _graphMock = new();
            _interactionBehaviorMock = new();
            _sequenceNodeGeneratorMock = new();

            _sut = new ArchitectureSequenceView()
            {
                Graph = _graphMock.Object,
                InteractionBehavior = _interactionBehaviorMock.Object,
                SequenceNodeGenerator = _sequenceNodeGeneratorMock.Object,
            };

            _headerNodeMock = new();
            _lifelineNodeMock = new();
        }

        [Test]
        public void AddClassNode_WhenClassNodeIsAdded_ThenStartCallSequenceButtonIsVisibleAndAddMethodCallIsInvisible()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));

            // Act
            _sut.AddClassNode(classInfo);

            // Assert
            _interactionBehaviorMock.Verify(m => m.AttachButtonBehavior(_sut, classInfo, _lifelineNodeMock.Object), Times.Once);
        }

        [Test]
        public void AddMethodCall_WhenMethodCallIsAdded_ThenCreateMethodCall()
        {
            // Arrange
            var sourceClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            var targetClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, sourceClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, targetClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));

            _sut.AddClassNode(sourceClassInfo);

            var calledMethod = new MethodInfo();

            // Act
            _sut.AddMethodCall(sourceClassInfo, targetClassInfo, calledMethod);

            // Assert
            _sequenceNodeGeneratorMock.Verify(m => m.CreateMethodCall(_graphMock.Object, sourceClassInfo, targetClassInfo, calledMethod), Times.Once);
        }

        [Test]
        public void AddMethodCall_WhenTargetClassWasAlreadyAdded_ThenTargetClassIsNotAddedAgain()
        {
            // Arrange
            var sourceClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            var targetClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, sourceClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, targetClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));

            _sut.AddClassNode(sourceClassInfo);
            _sut.AddClassNode(targetClassInfo);

            // Act
            _sut.AddMethodCall(sourceClassInfo, targetClassInfo, new MethodInfo());

            // Assert
            _sequenceNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, It.IsAny<ClassInfo>()), Times.Exactly(2), "Expected that the target class is not added again with AddMethodCall");
        }

        [Test]
        public void AddMethodCall_WhenTargetClassWasNotYetAdded_ThenTargetClassIsAdded()
        {
            // Arrange
            var sourceClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            var targetClassInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };
            
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, sourceClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, targetClassInfo)).Returns((_headerNodeMock.Object, _lifelineNodeMock.Object));

            _sut.AddClassNode(sourceClassInfo);

            // Act
            _sut.AddMethodCall(sourceClassInfo, targetClassInfo, new MethodInfo());

            // Assert
            _interactionBehaviorMock.Verify(m => m.AttachButtonBehavior(_sut, targetClassInfo, _lifelineNodeMock.Object));
        }
    }
}
