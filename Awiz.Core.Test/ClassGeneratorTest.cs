using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassGeneratorTest
    {
        private AnnotationOptions _annotationOptions = new();
        private Mock<IClassNodeGenerator> _classNodeGeneratorMock = new();
        private Mock<IClassProvider> _classProviderMock = new();

        private Mock<IGraph> _graphMock = new();

        private ClassGenerator _sut = new();

        [SetUp]
        public void SetUp()
        {
            _annotationOptions = new();

            _classNodeGeneratorMock = new Mock<IClassNodeGenerator>();
            _classProviderMock = new Mock<IClassProvider>();

            _graphMock = new Mock<IGraph>();

            var classFilterMock = new Mock<IClassFilter>();
            classFilterMock.Setup(m => m.Filter(It.IsAny<IClassProvider>())).Returns((IClassProvider cp) => cp);

            _sut = new ClassGenerator()
            {
                AnnotationOptions = _annotationOptions,
                ClassFilter = classFilterMock.Object,
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
            };
        }

        [Test]
        public void Associations_WhenAssociationsEnabledAndClassHasPropertyOfOtherClass_ThenEdgeIsInsertedBetweenClasses()
        {
            // Arrange
            _annotationOptions.EnableAssociations = true;

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
                Properties = 
                [
                    new PropertyInfo()
                    {
                        Type = "bar"
                    }
                ],
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Namespace = "My.W2",
                Properties =
                [
                    new PropertyInfo()
                    {
                        Type = "foo"
                    }
                ],
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, classInfo1, classInfo2), 
                "Expected association from classInfo1 to claaInfo2 because classInfo1 has a property with the type of classInfo2");
            
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, classInfo2, classInfo1), 
                "Expected association from classInfo2 to claaInfo1 because classInfo2 has a property with the type of classInfo1");
        }

        [Test]
        public void Implements_WhenClassImplementsInterface_ThenEdgeIsInsertedBetweenClassAndInterface()
        {
            // Arrange
            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
                Type = ClassType.Class,
            };

            classInfo1.ImplementedInterfaces.Add("My.W2.IBar");

            var classInfo2 = new ClassInfo()
            {
                Name = "IBar",
                Namespace = "My.W2",
                Type = ClassType.Interface,
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);
            
            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateImplementation(_graphMock.Object, classInfo2, classInfo1),
                "Expected that classInfo1 implements the interface classInfo2");
        }

        [Test]
        public void Extends_WhenClassExtendsOtherClass_ThenExtentionRelationIsGenerated()
        {
            // Arrange
            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
                Type = ClassType.Class,
            };

            var classInfo2 = new ClassInfo()
            {
                BaseClass = "My.W1.foo",
                Name = "IBar",
                Namespace = "My.W1",
                Type = ClassType.Class,
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateExtension(_graphMock.Object, classInfo1, classInfo2));
        }
    }
}
