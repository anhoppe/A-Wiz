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
        Mock<ISerializer> _serializerMock = new();
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
                Serializer = _serializerMock.Object,
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

        [Test]
        public void AddClassNode_WhenClassIsAddedTwice_ThenItIsOnlyOnceAddedToTheGraph()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Directory = "c:/temp/foo.cs",
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var nodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo)).Returns(nodeMock.Object);

            // Act
            _sut.AddClassNode(classInfo);
            _sut.AddClassNode(classInfo);
            
            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo), Times.Once);
        }

        [Test]
        public void Load_WhenLoading_ThenTheNodeToClassInfoMappingIsRestored()
        {
            // Arrang
            var classInfo1 = new ClassInfo()
            {
                Name = "Class1",
                Namespace = "This.is",
            };

            var classInfo2 = new ClassInfo()
            {
                Name = "Class2",
                Namespace = "This.is",
            };

            _sut = new ArchitectureClassView([classInfo1, classInfo2])
            {
                ClassNodeGenerator = _classNodeGeneratorMock.Object,
                FileSystem = _fileSystemMock.Object,
                Graph = _graphMock.Object,
                Name = "foobar",
                RelationBuilder = _relationBuilderMock.Object,
                RepoPath = "c:\\temp",
                Serializer = _serializerMock.Object,
                StorageAccess = _storageAccessMock.Object,
            };

            var node1Mock = new Mock<INode>();
            var node2Mock = new Mock<INode>();
            node1Mock.Setup(m => m.Id).Returns("foo");
            node2Mock.Setup(m => m.Id).Returns("bar");
            _graphMock.Setup(m => m.Nodes).Returns([node1Mock.Object, node2Mock.Object]);

            var mapping = new Dictionary<string, string>()
            {
                { "foo", "This.is.Class1" },
                { "bar", "This.is.Class2" }
            };

            _storageAccessMock.Setup(m => m.LoadNodeIdToClassIdMapping(It.IsAny<Stream>())).Returns(mapping);
            _fileSystemMock.Setup(m => m.Exists("c:\\temp\\.wiz\\storage\\foobar.yaml")).Returns(true);
            _fileSystemMock.Setup(m => m.OpenRead("c:\\temp\\.wiz\\storage\\foobar.yaml")).Returns(Mock.Of<Stream>());

            // Unfortunately, we have to test if the mapping is correct by calling Save and check that the passed map is corrrect
            bool mappingCorrect = false;
            _storageAccessMock.Setup(m => m.SaveNodeIdToClassIdMapping(It.IsAny<IDictionary<string, string>>(), It.IsAny<Stream>())).Callback((IDictionary<string, string> mapping, Stream stream) =>
            {
                if (mapping.Count == 2 && 
                    mapping.ContainsKey("foo") && mapping["foo"] == "This.is.Class1" &&
                    mapping.ContainsKey("bar") && mapping["bar"] == "This.is.Class2")
                {
                    mappingCorrect = true;
                }
            });

            // Act
            _sut.Load();

            // Assert
            _sut.Save();
            Assert.That(mappingCorrect);
        }

        [Test]
        public void Save_WhenSaved_ThenStorageSavesNodeToClassInfoMap()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Directory = "c:/temp/foo.cs",
                Name = "BaseClass",
                Namespace = "This.is",
            };

            var nodeMock = new Mock<INode>();
            nodeMock.Setup(m => m.Id).Returns("foo");

            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo)).Returns(nodeMock.Object);
            _sut.AddClassNode(classInfo);

            _fileSystemMock.Setup(m => m.Create(It.IsAny<string>())).Returns(Mock.Of<Stream>());

            bool mappingCorrect = false;
            _storageAccessMock.Setup(m => m.SaveNodeIdToClassIdMapping(It.IsAny<IDictionary<string, string>>(), It.IsAny<Stream>())).Callback((IDictionary<string, string> mapping, Stream stream) =>
            {
                if (mapping.Count == 1 && mapping.ContainsKey("foo") && mapping["foo"] == "This.is.BaseClass")
                {
                    mappingCorrect = true;
                }
            });

            // Act
            _sut.Save();

            // Assert
            Assert.That(mappingCorrect);
        }
    }
}
