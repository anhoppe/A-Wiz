using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassNodeGeneratorTest
    {
        private ClassNodeGenerator _sut = new ClassNodeGenerator();
        [SetUp]
        public void SetUp()
        { 
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

            var graphMock = new Mock<IGraph>();

            node1Mock.Setup(p => p.Grid).Returns(MockGrid());
            node2Mock.Setup(p => p.Grid).Returns(MockGrid());

            graphMock.Setup(p => p.AddNode("Class")).Returns(node1Mock.Object);
            _sut.CreateClassNode(graphMock.Object, classInfo1);

            graphMock.Setup(p => p.AddNode("Class")).Returns(node2Mock.Object);
            _sut.CreateClassNode(graphMock.Object, classInfo2);

            // Act
            _sut.CreateAssociation(graphMock.Object, classInfo1, classInfo2);

            // Assert
            graphMock.Verify(m => m.AddEdge(node1Mock.Object, node2Mock.Object));
        }

        private IGrid MockGrid()
        {
            var gridMock = new Mock<IGrid>();

            // This is the expected field size for a class node (1 column with 3 rows for title, props and methods)
            var fieldText = new string[1][];
            fieldText[0] = new string[3];

            gridMock.Setup(p => p.FieldText).Returns(fieldText);

            return gridMock.Object;
        }
    }
}
