using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.CSharpClassGenerator
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
        public void Associations_WhenAssociationsEnabledButClassAreNotAssicoated_ThenNoAssociationIsInserted()
        {
            // Arrange
            _annotationOptions.EnableAssociations = true;

            var classInfo1 = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Namespace = "My.W2",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, It.IsAny<ClassInfo>(), It.IsAny<ClassInfo>()),
                Times.Never,
                "There should be no association between the classes since there are no properties of the other type defined");

            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, It.IsAny<ClassInfo>(), It.IsAny<ClassInfo>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never,
                "There should be no association between the classes since there are no properties of the other type defined");
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
                        TypeName = "bar"
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
                        TypeName = "foo"
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
        public void Associations_WhenClassHasMultiplePropertiesToOtherClass_ThenMultiplicityIsInsertedCorrectly()
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
                        TypeName = "bar"
                    },
                    new PropertyInfo()
                    {
                        TypeName = "bar"
                    }
                ],
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Namespace = "My.W2",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, classInfo1, classInfo2, "1", "2"),
                "Expected association with correct multiplicity since classInfo1 has two property with the type of classInfo2");
        }

        [Test]
        public void Association_WhenClassHasGenericListOfOtherClass_Then1TonMultiplicityIsSet()
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
                        TypeName = "List<bar>",
                        IsEnumerable = true,
                        GenericType = new ClassInfo()
                        {
                            Name = "bar",
                            Namespace = "My.W2",
                        },
                    },
                ],
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "bar",
                Namespace = "My.W2",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, classInfo1, classInfo2, "1", "*"),
                "Expected association with correct multiplicity since classInfo1 has List<> property with the type of classInfo2");
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
