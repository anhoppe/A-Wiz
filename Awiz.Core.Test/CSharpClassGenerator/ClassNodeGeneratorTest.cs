using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.CSharpClassGenerator
{
    [TestFixture]
    public class ClassNodeGeneratorTest
    {
        private Mock<IGraph> _graphMock = new();

        private ClassNodeGenerator _sut = new ClassNodeGenerator();

        [SetUp]
        public void SetUp()
        { 
            _graphMock = new();

            _sut = new ClassNodeGenerator();
        }

        [Test]
        public void CreateAssociation_WhenAssociationIsAddedForExistingClasses_ThenAssiciationIsAddedToTheGraph()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar"
            };

            node1Mock.Setup(p => p.Grid).Returns(MockGrid());
            node2Mock.Setup(p => p.Grid).Returns(MockGrid());

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node1Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo1);

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node2Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo2);

            // Act
            _sut.CreateAssociation(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            _graphMock.Verify(m => m.AddEdge(node1Mock.Object, node2Mock.Object));
        }

        [Test]
        public void CreateAssociationWithMultiplicity_WhenAssociationWithMultiplicityIsAdded_ThenEdgeWithFromToLabelsAreAdded()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar"
            };

            node1Mock.Setup(p => p.Grid).Returns(MockGrid());
            node2Mock.Setup(p => p.Grid).Returns(MockGrid());

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node1Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo1);

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node2Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo2);

            // Act
            _sut.CreateAssociation(_graphMock.Object, classInfo1, classInfo2, "foo", "bar");

            // Assert
            _graphMock.Verify(m => m.AddEdge(node1Mock.Object, node2Mock.Object, "foo", "bar", It.IsAny<float>()));
        }

        [Test]
        public void CreateImplementation_WhenImplementationIsAddedForClasses_ThenAnEdgeWithClosedArrowAndDashedLineInDirectionOfInterfaceIsInserted()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Type = ClassType.Interface,
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Type = ClassType.Class,
            };

            node1Mock.Setup(p => p.Grid).Returns(MockGrid());
            node2Mock.Setup(p => p.Grid).Returns(MockGrid());

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node1Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo1);

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node2Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo2);

            // Act
            _sut.CreateImplementation(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            _graphMock.Verify(m => m.AddEdge(node2Mock.Object, node1Mock.Object, Ending.ClosedArrow, Style.Dashed));
        }

        [Test]
        public void CreateExtension_WhenExtensionIsAddedForClasses_ThenAnEdgeWithClosedArrowIsInserted()
        {
            // Arrange
            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Type = ClassType.Class,
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Type = ClassType.Class,
            };

            node1Mock.Setup(p => p.Grid).Returns(MockGrid());
            node2Mock.Setup(p => p.Grid).Returns(MockGrid());

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node1Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo1);

            _graphMock.Setup(p => p.AddNode("Class")).Returns(node2Mock.Object);
            _sut.CreateClassNode(_graphMock.Object, classInfo2);

            // Act
            _sut.CreateExtension(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            _graphMock.Verify(m => m.AddEdge(node2Mock.Object, node1Mock.Object, Ending.ClosedArrow, Style.None));
        }

        private IGrid MockGrid()
        {
            var gridMock = new Mock<IGrid>();

            // This is the expected field size for a class node (1 column with 3 rows for title, props and methods)
            var cell = new IGridCell[1][];
            cell[0] = new IGridCell[3];

            cell[0][0] = Mock.Of<IGridCell>();
            cell[0][1] = Mock.Of<IGridCell>();
            cell[0][2] = Mock.Of<IGridCell>();
            gridMock.Setup(p => p.Cells).Returns(cell);

            return gridMock.Object;
        }
    }
}
