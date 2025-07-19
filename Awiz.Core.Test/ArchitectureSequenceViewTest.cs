using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using Wiz.Infrastructure.IO;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ArchitectureSequenceViewTest
    {
        private Mock<IGraph> _graphMock = new();

        private Mock<IInteractionBehavior> _interactionBehaviorMock = new();

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

            // Mocking here for the user lifeline that is created in the Initialize method
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, It.IsAny<ClassInfo>()));
            _sut.Initialize();
        }

        [Test]
        public void AddClassNode_WhenClassNodeIsAdded_ThenUpdateUserInitiaiteCallSequenceIsCalled()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            // Act
            _sut.AddClassNode(classInfo);

            // Assert
            _interactionBehaviorMock.Verify(m => m.UpdateUserInitiaiteCallSequence(It.IsAny<INode>(), It.IsAny<IList<ClassInfo>>(), It.IsAny<Action<ClassInfo, MethodInfo>>()), Times.Exactly(2), "Expected to be called twice, once during initializaiton and once during test");
        }

        [Test]
        public void AddMethodCall_WhenMethodCallIsAdded_ThenCreateMethodCallAndMethodCallInfoAddedToCallStack()
        {
            // Arrange
            var sourceClassInfo = new ClassInfo()
            {
                Name = "SourceClass",
                Namespace = "MyNamespace",
            };

            var targetClassInfo = new ClassInfo()
            {
                Name = "TargetClass",
                Namespace = "MyNamespace",
            };

            (var sourceHeaderNode, var sourceLifelineNode) = CreateHeaderAndLifelineNodes();
            (var targetHeaderNode, var targetLifelineNode) = CreateHeaderAndLifelineNodes();

            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, sourceClassInfo));
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, targetClassInfo));

            _sut.AddClassNode(sourceClassInfo);

            var calledMethod = new MethodInfo();

            // Act
            _sut.AddMethodCall(sourceClassInfo, targetClassInfo, calledMethod);

            // Assert
            _sequenceNodeGeneratorMock.Verify(m => m.CreateMethodCall(_graphMock.Object, It.IsAny<CallInfo>()), Times.Once);
            
            var methodCallInfo = _sut.CallInfo.Peek();
            Assert.That(methodCallInfo.CalledMethod, Is.EqualTo(calledMethod));
            Assert.That(methodCallInfo.SourceNode, Is.EqualTo(sourceLifelineNode));
            Assert.That(methodCallInfo.TargetNode, Is.EqualTo(targetLifelineNode));
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

            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, sourceClassInfo));
            _sequenceNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, targetClassInfo));

            _sut.AddClassNode(sourceClassInfo);
            _sut.AddClassNode(targetClassInfo);

            // Act
            _sut.AddMethodCall(sourceClassInfo, targetClassInfo, new MethodInfo());

            // Assert
            _sequenceNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, It.IsAny<ClassInfo>()), Times.Exactly(3), 
                "Expected that only three class nodes are added, 1 for the user, 1 for source and 1 for target class. Target class is not added again with AddMethodCall");
        }

        [Test, Ignore("Refactor Loading / Saving, then bring back test for it")]
        public void Load_WhenLoaded_ThenNodeToClassInfoStored()
        {
            // Arrang
            var classInfo1 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "Class2",
                Namespace = "This.is",
            };

            var fileSystemMock = new Mock<IFileSystem>();
            var serializerMock = new Mock<ISerializer>();
            var storageAccessMock = new Mock<IStorageAccess>();

            _sut = new ArchitectureSequenceView()
            {
                FileSystem = fileSystemMock.Object,
                Graph = _graphMock.Object,
                InteractionBehavior = _interactionBehaviorMock.Object,
                Name = "foobar",
                RepoPath = "c:\\temp",
                SequenceNodeGenerator = _sequenceNodeGeneratorMock.Object,
                Serializer = serializerMock.Object,
                StorageAccess = storageAccessMock.Object,
            };

            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();
            node1Mock.Setup(m => m.Id).Returns("foo");
            node2Mock.Setup(m => m.Id).Returns("bar");
            _graphMock.Setup(m => m.Nodes).Returns([node1Mock.Object, node2Mock.Object]);

            var mapping = new Dictionary<string, ClassInfo>()
            {
                { "foo", classInfo1 },
                { "bar", classInfo2 }
            };

            storageAccessMock.Setup(m => m.LoadNodeIdToClassInfoMapping(It.IsAny<Stream>())).Returns(mapping);
            fileSystemMock.Setup(m => m.Exists("c:\\temp\\.wiz\\storage\\sequence\\mapping\\foobar.yaml")).Returns(true);
            fileSystemMock.Setup(m => m.OpenRead("c:\\temp\\.wiz\\storage\\sequence\\mapping\\foobar.yaml")).Returns(Mock.Of<Stream>());

            // Unfortunately, we have to test if the mapping is correct by calling Save and check that the passed map is corrrect
            bool mappingCorrect = false;
            storageAccessMock.Setup(m => m.SaveNodeIdToClassInfoMapping(It.IsAny<IDictionary<string, ClassInfo>>(), It.IsAny<Stream>())).Callback((IDictionary<string, ClassInfo> mapping, Stream stream) =>
            {
                if (mapping.Count == 2 &&
                    mapping.ContainsKey("foo") && mapping["foo"] == classInfo1 &&
                    mapping.ContainsKey("bar") && mapping["bar"] == classInfo2)
                {
                    mappingCorrect = true;
                }
            });

            // Act
            _sut.Load(Mock.Of<IVersionUpdater>());

            // Assert
            _sut.Save();
            Assert.That(mappingCorrect);
        }

        private (INode headerNode, INode lifelineNode) CreateHeaderAndLifelineNodes()
        {
            var headerNodeMock = new Mock<INode>();
            var lifelineNodeMock = new Mock<INode>();

            headerNodeMock.Setup(p => p.Id).Returns("");
            //lifelineNodeMock.Setup(p => p.Id).Returns($"{ISequenceNodeGenerator.LifelineId}:bar");

            return (headerNodeMock.Object, lifelineNodeMock.Object);
        }

    }
}
