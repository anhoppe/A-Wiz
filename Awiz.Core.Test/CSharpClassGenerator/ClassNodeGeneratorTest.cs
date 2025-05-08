using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
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

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

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

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

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

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

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

            _sut.NodeToClassInfoMapping = new Dictionary<INode, ClassInfo>()
            {
                [node1Mock.Object] = classInfo1,
                [node2Mock.Object] = classInfo2,
            };

            // Act
            _sut.CreateExtension(_graphMock.Object, classInfo1, classInfo2);

            // Assert
            _graphMock.Verify(m => m.AddEdge(node2Mock.Object, node1Mock.Object, Ending.ClosedArrow, Style.None));
        }
    }
}
