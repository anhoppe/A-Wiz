using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.SequenceDiagram
{
    [TestFixture]
    public class SequenceNodeGeneratorTest
    {
        private Mock<IEdgeBuilder> edgeBuilderMock = new();

        private Mock<IGraph> _graphMock = new();

        private Mock<INode> _lifelineNodeMock = new();

        private Mock<ISourceCode> _sourceCodeMock = new();

        private SequenceNodeGenerator _sut = new();

        [SetUp]
        public void SetUp()
        {
            var headerNodeMock = new Mock<INode>();
            _lifelineNodeMock = new Mock<INode>();

            _graphMock = new();
            _sourceCodeMock = new();

            _sut = new SequenceNodeGenerator()
            {
                SourceCode = _sourceCodeMock.Object,
            };

            var gridMock = new Mock<IGrid>();
            var cell = new Mock<IGridCell>();

            gridMock.Setup(m => m.Cells).Returns(new IGridCell[1, 1]
            {
                {
                    cell.Object,
                }
            });
            headerNodeMock.Setup(m => m.Grid).Returns(gridMock.Object);

            _graphMock.Setup(p => p.AddNode("SequenceHeader")).Returns(headerNodeMock.Object);
            _graphMock.Setup(p => p.AddNode("SequenceLifeline")).Returns(_lifelineNodeMock.Object);

            edgeBuilderMock = new();
            edgeBuilderMock.Setup(p => p.WithFromDockingPosition(It.IsAny<Direction>(), It.IsAny<int>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithToDockingPosition(It.IsAny<Direction>(), It.IsAny<int>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithEnding(It.IsAny<Ending>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithText(It.IsAny<string>())).Returns(edgeBuilderMock.Object);

            _graphMock.Setup(m => m.AddEdge(It.IsAny<INode>(), It.IsAny<INode>())).Returns(edgeBuilderMock.Object);
        }


        [Test]
        public void CreateClassNode_WhenClassNodeIsAdded_ThenTwoNodesAreInsertedIntoGraph()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            // Act
            _sut.CreateClassNode(_graphMock.Object, classInfo);

            // Assert
            _graphMock.Verify(m => m.AddNode("SequenceHeader"));
            _graphMock.Verify(m => m.AddNode("SequenceLifeline"));
        }

        [Test]
        public void CreateMethodCall_WhenSourceWasNotAdded_ThenExceptionThrown()
        {
            // Arrange
            var sourceLifeline = new ClassInfo();
            var targetLifeline = new ClassInfo();
            var methodInfo = new MethodInfo();

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => _sut.CreateMethodCall(_graphMock.Object, sourceLifeline, targetLifeline, methodInfo));
        }

        [Test]
        public void CreateMethodCall_WhenTargetWasAdded_ThenExceptionThrown()
        {
            // Arrange
            var sourceLifeline = new ClassInfo();
            var targetLifeline = new ClassInfo();
            var methodInfo = new MethodInfo();
            var (headerNode, lifelineNode) = _sut.CreateClassNode(_graphMock.Object, sourceLifeline);

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => _sut.CreateMethodCall(_graphMock.Object, sourceLifeline, targetLifeline, methodInfo));
        }

        [Test]
        public void CreateMethodCall_WhenCalled_ThenAllLiflinesExtendedInHeight()
        {
            // Arrange
            var sourceLifeline = new ClassInfo();
            var targetLifeline = new ClassInfo();
            var methodInfo = new MethodInfo();

            _lifelineNodeMock.Setup(p => p.Height).Returns(Design.SequenceLifelineHeight);

            _ = _sut.CreateClassNode(_graphMock.Object, sourceLifeline);
            _ = _sut.CreateClassNode(_graphMock.Object, targetLifeline);

            // Act
            _sut.CreateMethodCall(_graphMock.Object, sourceLifeline, targetLifeline, methodInfo);

            // Assert
            _lifelineNodeMock.VerifySet(p => p.Height = Design.SequenceLifelineHeight * 2);
        }

        [Test]
        public void CreateMethodCall_WhenCalled_ThenEdgeCreated()
        {
            var sourceLifeline = new ClassInfo();
            var targetLifeline = new ClassInfo();
            var methodInfo = new MethodInfo();

            _ = _sut.CreateClassNode(_graphMock.Object, sourceLifeline);
            _ = _sut.CreateClassNode(_graphMock.Object, targetLifeline);

            _lifelineNodeMock.Setup(p => p.Height).Returns(Design.SequenceLifelineHeight);

            // Act
            _sut.CreateMethodCall(_graphMock.Object, sourceLifeline, targetLifeline, methodInfo);

            // Assert
            edgeBuilderMock.Verify(m => m.WithFromDockingPosition(Direction.Right, Design.SequenceLifelineHeight));
            edgeBuilderMock.Verify(m => m.WithToDockingPosition(Direction.Left, Design.SequenceLifelineHeight));
            edgeBuilderMock.Verify(m => m.Build());
        }
    }
}
