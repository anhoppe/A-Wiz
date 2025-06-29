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
        private Mock<IEdgeBuilder> edgeBuilderMock = null!;

        private Mock<IGraph> _graphMock = null!;

        private Mock<INode> _headerNodeMock = null!;

        private Mock<INode> _lifelineNodeMock = null!;

        private Mock<ISourceCode> _sourceCodeMock = null!;

        private SequenceNodeGenerator _sut = null!;

        [SetUp]
        public void SetUp()
        {
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

            _headerNodeMock = new Mock<INode>();
            _headerNodeMock.Setup(m => m.Grid).Returns(gridMock.Object);

            _graphMock.Setup(p => p.AddNode("SequenceHeader")).Returns(_headerNodeMock.Object);
            _graphMock.Setup(p => p.AddNode("SequenceLifeline")).Returns(_lifelineNodeMock.Object);

            edgeBuilderMock = new();
            edgeBuilderMock.Setup(p => p.WithFromDockingPosition(It.IsAny<Direction>(), It.IsAny<int>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithToDockingPosition(It.IsAny<Direction>(), It.IsAny<int>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithEnding(It.IsAny<Ending>())).Returns(edgeBuilderMock.Object);
            edgeBuilderMock.Setup(p => p.WithText(It.IsAny<string>())).Returns(edgeBuilderMock.Object);

            _graphMock.Setup(m => m.AddEdge(It.IsAny<INode>(), It.IsAny<INode>())).Returns(edgeBuilderMock.Object);
        }

        [Test]
        public void CreateClassNode_WhenCalled_ThenReturnsCorrectNodes()
        {
            // Arrange
            var classInfo = new ClassInfo();
            
            // Act
            var (header, lifeline) = _sut.CreateClassNode(_graphMock.Object, classInfo, 100);
            
            // Assert
            Assert.That(header == _headerNodeMock.Object);
            Assert.That(lifeline == _lifelineNodeMock.Object);

            _headerNodeMock.VerifySet(p => p.Width = Design.SequenceHeaderWidth, Times.Once);
            _headerNodeMock.VerifySet(p => p.Height= Design.SequenceHeaderHeight, Times.Once);

            _lifelineNodeMock.VerifySet(p => p.Width = Design.SequenceLifelineWidth, Times.Once);
            _lifelineNodeMock.VerifySet(p => p.Height = 100, Times.Once);
        }
    }
}
