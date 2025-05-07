using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class RelationBuilderTest
    {
        private Mock<IClassNodeGenerator> _classNodeGeneratorMock = new();

        private Mock<IGraph> _graphMock = new();

        private RelationBuilder _sut = new();

        [SetUp]
        public void SetUp()
        {
            _classNodeGeneratorMock = new();
            _graphMock = new Mock<IGraph>();
            _sut = new()
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
            };  
        }

        [Test]
        public void Association1ToMany_WhenClassIsAddedAndExisingClassHasEnumearbleProperty_Then1ToNAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };

            var class2 = new ClassInfo()
            {
                Properties =
                [
                    new PropertyInfo()
                    {
                        IsEnumerable = true,
                        GenericType = new ClassInfo()
                        {
                            Name = "Class1",
                            Namespace = "This.is",
                        },
                    },
                ],
            };

            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class1, class2, "1", "*"));
        }

        [Test]
        public void Association1ToMany_WhenClassIsAddedAndThatHasHasEnumearblePropertyOfExisingClas_Then1ToNAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Properties =
                [
                    new PropertyInfo()
                    {
                        IsEnumerable = true,
                        GenericType = new ClassInfo()
                        {
                            Name = "Class2",
                            Namespace = "This.is",
                        },
                    },
                ],
            };

            var class2 = new ClassInfo()
            {
                Name = "Class2",
                Namespace = "This.is",
            };

            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class2, class1, "1", "*"));
        }

        [Test]
        public void Association1ToMultiple_WhenClassIsAddedAndExistingClassHasClassAsMultipleProperties_Then1ToManyAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };

            var class2 = new ClassInfo()
            {
                Properties =
                [
                    new PropertyInfo()
                    {
                        Name = "foo",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    },
                    new PropertyInfo()
                    {
                        Name = "bar",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    },
                    new PropertyInfo()
                    {
                        Name = "buz",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    }
                ],
            };

            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class1, class2, "1", "3"));
        }


        [Test]
        public void Association1ToMultiple_WhenClassIsAddedWithMultiplePropertiesToExistingClass_Then1ToManyAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Properties =
                [
                    new PropertyInfo()
                    {
                        Name = "foo",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    },
                    new PropertyInfo()
                    {
                        Name = "bar",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    },
                    new PropertyInfo()
                    {
                        Name = "buz",
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    }
                ],
            };

            var class2 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };


            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class2, class1, "1", "3"));
        }

        [Test]
        public void Association_WhenClassIsAddedAndExistingClassHasClassAsProperty_ThenAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };

            var class2 = new ClassInfo()
            {
                Properties = 
                [
                    new PropertyInfo()
                    {
                        TypeName = "Class1",
                        TypeNamespace = "This.is",
                    }
                ],
            };

            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class1, class2));
        }

        [Test]
        public void Association_WhenClassIsAddedThatHasAssociationToOtherClass_ThenAssociationIsAdded()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Properties =
                [
                    new PropertyInfo()
                    {
                        TypeName = "Class2",
                        TypeNamespace = "This.is",
                    }
                ],
            };

            var class2 = new ClassInfo()
            {
                Name = "Class2",
                Namespace = "This.is",
            };

            // Act
            _sut.Build(_graphMock.Object, class1, [class2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, class2, class1));
        }

        [Test]
        public void Extends_WhenBaseClassInDiagram_ThenExtensionIsAdded()
        {
            // Arrange
            var baseClass = new ClassInfo()
            {
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var derivedClass = new ClassInfo()
            {
                BaseClass = "This.is.BaseClass",
            };

            // Act
            _sut.Build(_graphMock.Object, derivedClass, [baseClass]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateExtension(_graphMock.Object, baseClass, derivedClass));
        }

        [Test]
        public void Extends_WhenBaseClassIsAddedAndDerivedClassesInDiagram_ThenAllExtensionsAreAdded()
        {
            // Arrange
            var baseClass = new ClassInfo()
            {
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var derivedClass1 = new ClassInfo()
            {
                BaseClass = "This.is.BaseClass",
            };

            var derivedClass2 = new ClassInfo()
            {
                BaseClass = "This.is.BaseClass",
            };

            // Act
            _sut.Build(_graphMock.Object, baseClass, [derivedClass1, derivedClass2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateExtension(_graphMock.Object, baseClass, derivedClass1));
            _classNodeGeneratorMock.Verify(m => m.CreateExtension(_graphMock.Object, baseClass, derivedClass2));
        }

        [Test]
        public void Implements_WhenClassIsAddedThatImplementsInterfaces_ThenImplementsRelationsAreAdded()
        {
            // Arrange
            var implementedInterface1 = new ClassInfo()
            {
                Name = "Interface1",
                Namespace = "This.is",
            };
            var implementedInterface2 = new ClassInfo()
            {
                Name = "Interface2",
                Namespace = "This.is",
            };

            var implementingClass = new ClassInfo()
            {
                ImplementedInterfaces = { "This.is.Interface1", "This.is.Interface2" },
            };

            // Act
            _sut.Build(_graphMock.Object, implementingClass, [implementedInterface1, implementedInterface2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateImplementation(_graphMock.Object, implementedInterface1, implementingClass));
            _classNodeGeneratorMock.Verify(m => m.CreateImplementation(_graphMock.Object, implementedInterface2, implementingClass));
        }


        [Test]
        public void Implements_WhenInterfaceIsAddedThatIsImplementedByClasses_ThenImplementsRelationsAreAdded()
        {
            // Arrange
            var implementedInterface = new ClassInfo()
            {
                Name = "Interface1",
                Namespace = "This.is",
            };

            var implementingClass1 = new ClassInfo()
            {
                ImplementedInterfaces = { "This.is.Interface1" },
            };

            var implementingClass2 = new ClassInfo()
            {
                ImplementedInterfaces = { "This.is.Interface1" },
            };

            // Act
            _sut.Build(_graphMock.Object, implementedInterface, [implementingClass1, implementingClass2]);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateImplementation(_graphMock.Object, implementedInterface, implementingClass1));
            _classNodeGeneratorMock.Verify(m => m.CreateImplementation(_graphMock.Object, implementedInterface, implementingClass2));
        }


    }
}
