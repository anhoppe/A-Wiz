using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using Wiz.Infrastructure.IO;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ArchitectureClassViewTest
    {
        Mock<IClassNodeGenerator> _classNodeGeneratorMock = new();
        Mock<IFileSystem> _fileSystemMock = new();
        Mock<IGraph> _graphMock = new();
        Mock<IRelationBuilder> _relationBuilderMock = new();
        Mock<IStorageAccess> _storageAccessMock = new();
        
        ArchitectureClassView _sut = new(new List<ClassInfo>());

        [SetUp]
        public void SetUp()
        {
            _classNodeGeneratorMock = new();
            _fileSystemMock = new();
            _graphMock = new();
            _relationBuilderMock = new();
            _storageAccessMock = new();

            _sut = new ArchitectureClassView(new List<ClassInfo>())
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                FileSystem = _fileSystemMock.Object,
                Graph = _graphMock.Object,
                RelationBuilder = _relationBuilderMock.Object,
                StorageAccess = _storageAccessMock.Object,
            };
        }

        [Test]
        public void AddBaseClassNode_WhenPassingNode_ThenTheBaseClassOfTheNodeIsInserted()
        {
            // Arrange
            var baseClass = new ClassInfo()
            {
                Directory = "c:/temp/foo.cs",
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var derivedClass = new ClassInfo()
            {
                Directory = "c:/temp/bar.cs",
                BaseClass = "This.is.BaseClass",
                Name = "DerivedClass"
            };

            var classInfos = new List<ClassInfo>()
            {
                baseClass,
                derivedClass
            };

            _sut = new ArchitectureClassView(classInfos)
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                FileSystem = _fileSystemMock.Object,
                Graph = _graphMock.Object,
                RelationBuilder = _relationBuilderMock.Object,
                RepoPath = "c:/temp",
                StorageAccess = _storageAccessMock.Object,
            };

            var nodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, baseClass)).Returns(nodeMock.Object);

            // Act
            _sut.AddBaseClassNode(derivedClass);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, baseClass));
            _relationBuilderMock.Verify(m => m.Build(_graphMock.Object, baseClass, It.IsAny<IList<ClassInfo>>()));
        }

        [Test]
        public void AddBaseClassNode_WhenBaseClassCannotBeFoud_ThenExceptionIsRaised()
        {
            // Arrange
            var derivedClass = new ClassInfo()
            {
                BaseClass = "This.is.BaseClass",
                Name = "DerivedClass"
            };

            var classInfos = new List<ClassInfo>()
            {
                derivedClass
            };

            _sut = new ArchitectureClassView(classInfos)
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                FileSystem = _fileSystemMock.Object,
                Graph = _graphMock.Object,
                StorageAccess = _storageAccessMock.Object,
            };

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => _sut.AddBaseClassNode(derivedClass));
        }

        [Test]
        public void AddClassNode_WhenClassIsAdded_ThenRelationBuilderIsCalledWithListOfAlreadyAddedClassInfos()
        {
            // Arrange
            var baseClass = new ClassInfo()
            {
                Directory = "c:/temp/foo.cs",
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var derivedClass1 = new ClassInfo()
            {
                Directory = "c:/temp/bar.cs",
                BaseClass = "This.is.BaseClass",
                Name = "DerivedClass1"
            };

            var derivedClass2 = new ClassInfo()
            {
                Directory = "c:/temp/buz.cs",
                BaseClass = "This.is.BaseClass",
                Name = "DerivedClass2"
            };

            var classInfos = new List<ClassInfo>()
            {
                baseClass,
                derivedClass1,
                derivedClass2
            };

            _sut = new ArchitectureClassView(classInfos)
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                FileSystem = _fileSystemMock.Object,
                Graph = _graphMock.Object,
                RelationBuilder = _relationBuilderMock.Object,
                RepoPath = "c:/temp",
                StorageAccess = _storageAccessMock.Object,
            };

            var baseClassNodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, baseClass)).Returns(baseClassNodeMock.Object);

            var derivedClass1NodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, derivedClass1)).Returns(derivedClass1NodeMock.Object);

            var derivedClass2NodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, derivedClass2)).Returns(derivedClass2NodeMock.Object);

            _sut.AddClassNode(derivedClass1);
            _sut.AddClassNode(derivedClass2);

            // Act
            _sut.AddClassNode(baseClass);

            // Assert
            _relationBuilderMock.Verify(m => m.Build(_graphMock.Object, baseClass, new List<ClassInfo>() { derivedClass1, derivedClass2 }));
        }
    }
}
