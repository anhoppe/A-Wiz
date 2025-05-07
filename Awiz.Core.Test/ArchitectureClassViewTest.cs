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
        public void AddClassNode_WhenNodeIsAddedAndPersistenceInformationIsFound_ThenPersistenceInformationIsAppliedToNodeFromView()
        {
            // Arrange
            ClassInfo classInfo = new ClassInfo()
            {
                Directory = "c:\\dir\\foo\\repo\\foo\\foo.cs",
            };

            _sut.RepoPath = "c:\\dir\\foo\\repo";
            _sut.Name = "foobar";

            var nodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo)).Returns(nodeMock.Object);

            Stream stream = new MemoryStream();
            _fileSystemMock.Setup(m => m.Exists("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(true);
            _fileSystemMock.Setup(m => m.OpenRead("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(stream);

            // Act
            _sut.AddClassNode(classInfo);

            // Assert
            _classNodeGeneratorMock.Verify(m => m.CreateClassNode(_graphMock.Object, classInfo));
            _storageAccessMock.Verify(m => m.LoadNode(nodeMock.Object, "foobar", stream));
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
        public void AddClassNode_WhenNodeIsAddedAndPersistenceInformationIsNotFound_ThenPersistenceInformationIsNotAppliedToNode()
        {
            // Arrange
            ClassInfo classInfo = new ClassInfo()
            {
                Directory = "c:/dir/foo/repo/foo/foo.cs",
            };

            _sut.RepoPath = "c:/dir/foo/repo";
            var nodeMock = new Mock<INode>();
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo)).Returns(nodeMock.Object);

            _fileSystemMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            // Act
            _sut.AddClassNode(classInfo);

            // Assert
            _storageAccessMock.Verify(m => m.LoadNode(nodeMock.Object, It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
        }

        [Test]
        public void Save_WhenNewNodesAreAddedAndSavedToNewFile_ThenAllNodesAreStoredInCorrectViewAndFile()
        {
            // Arrange
            var classInfo1 = new ClassInfo()
            {
                Directory = "c:\\dir\\foo\\repo\\foo\\foo.cs",
            };

            var node1Mock = new Mock<INode>();
            node1Mock.Setup(p => p.X).Returns(11);
            node1Mock.Setup(p => p.Y).Returns(12);

            var classInfo2 = new ClassInfo()
            {
                Directory = "c:\\dir\\foo\\repo\\bar\\buz.cs"
            };

            var node2Mock = new Mock<INode>();
            node2Mock.Setup(p => p.X).Returns(13);
            node2Mock.Setup(p => p.Y).Returns(14);
            
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo1)).Returns(node1Mock.Object);
            _classNodeGeneratorMock.Setup(m => m.CreateClassNode(_graphMock.Object, classInfo2)).Returns(node2Mock.Object);

            _sut.RepoPath = "c:\\dir\\foo\\repo";
            _sut.Name = "barbuz";

            var stream1 = new MemoryStream();
            var stream2 = new MemoryStream();

            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(stream1);
            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\storage\\bar\\buz")).Returns(stream2);

            _sut.AddClassNode(classInfo1);
            _sut.AddClassNode(classInfo2);

            // We define that none of the files already existed (hence they are created)
            _fileSystemMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            // Act
            _sut.Save();

            // Assert
            _storageAccessMock.Verify(m => m.SaveNode(node1Mock.Object, It.IsAny<View>(), "barbuz", stream1));
            _storageAccessMock.Verify(m => m.SaveNode(node2Mock.Object, It.IsAny<View>(), "barbuz", stream2));
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
