using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassGeneratorTest
    {
        private Mock<IClassNodeGenerator> _classNodeGeneratorMock = new();
        private Mock<IClassProvider> _classProviderMock = new();

        private Config _config = new();

        private Mock<IGraph> _graphMock = new();

        private ClassGenerator _sut = new();

        [SetUp]
        public void SetUp()
        {
            _classNodeGeneratorMock = new Mock<IClassNodeGenerator>();
            _classProviderMock = new Mock<IClassProvider>();

            _config = new Config();

            _graphMock = new Mock<IGraph>();

            _sut = new ClassGenerator()
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                Config = _config,
            };
        }

        [Test]
        public void Generate_WhenWhiteListIsEmpty_ThenClassIsGenerated()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo), Times.Exactly(1));
        }

        [Test]
        public void Generate_WhenNamespaceIsBlckListed_ThenClassIsNotGenerated()
        {
            // Arrange
            _config.Namespaces.Blacklist.AddRange(new[] { "My.W1" }.ToList());

            var classInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.W1",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo,
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo), Times.Never);
        }


        [Test]
        public void Generate_WhenNamespaceWhitelisted_ThenOnlyClassesInNamespacesGenerated()
        {
            // Arrange
            _config.Namespaces.Whitelist.AddRange(new [] { "My.W1", "My.W2" });

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

            var classInfo3 = new ClassInfo()
            {
                Name = "buz",
                Namespace = "My.W3",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
                classInfo3
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo1), Times.Exactly(1));
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo2), Times.Exactly(1));
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo3), Times.Never);
        }

        [Test]
        public void Generate_WhenNamespaceBlackListed_ThenClassIsNotGenerated()
        {
            // Arrange
            _config.Namespaces.Whitelist.AddRange(new[] { "My.W1", "My.W2" });
            _config.Namespaces.Blacklist.AddRange(new[] { "My.W1" });

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

            var classInfo3 = new ClassInfo()
            {
                Name = "buz",
                Namespace = "My.W3",
            };

            _classProviderMock.Setup(p => p.Classes).Returns(new[]
            {
                classInfo1,
                classInfo2,
                classInfo3
            }.ToList());

            // Act
            _sut.Generate(_classProviderMock.Object, _graphMock.Object);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo1), Times.Never);
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo2), Times.Exactly(1));
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo3), Times.Never);
        }

        [Test]
        public void Generate_WhenAssociationsEnabledAndClassHasPropertyOfOtherClass_ThenEdgeIsInsertedBetweenClasses()
        {
            // Arrange
            _config.EnableAssociations = true;
            _config.Namespaces.Whitelist.AddRange(new[] { "My.W1", "My.W2" });

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
                "Expected to have an association from classInfo1 to claaInfo2 because classInfo1 has a property with the type of classInfo2");
            
            _classNodeGeneratorMock.Verify(m => m.CreateAssociation(_graphMock.Object, classInfo2, classInfo1), 
                "Expected to have an association from classInfo2 to claaInfo1 because classInfo2 has a property with the type of classInfo1");
        }
    }
}
