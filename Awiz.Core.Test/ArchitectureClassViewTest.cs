using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using Wiz.Infrastructure.IO;
using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ArchitectureClassViewTest
    {
        Mock<IFileSystem> _fileSystemMock = new();
        Mock<IStorageAccess> _storageAccessMock = new();

        ArchitectureClassView _sut = new();

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new();
            _storageAccessMock = new();

            _sut = new ArchitectureClassView()
            {
                FileSystem = _fileSystemMock.Object,
                StorageAccess = _storageAccessMock.Object,
            };
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

            Stream stream = new MemoryStream();
            _fileSystemMock.Setup(m => m.Exists("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(true);
            _fileSystemMock.Setup(m => m.OpenRead("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(stream);

            // Act
            _sut.AddClassNode(nodeMock.Object, classInfo);

            // Assert
            _storageAccessMock.Verify(m => m.LoadNode(nodeMock.Object, "foobar", stream));
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

            _fileSystemMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            // Act
            _sut.AddClassNode(nodeMock.Object, classInfo);

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

            _sut.RepoPath = "c:\\dir\\foo\\repo";
            _sut.Name = "barbuz";

            var stream1 = new MemoryStream();
            var stream2 = new MemoryStream();

            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\storage\\foo\\foo")).Returns(stream1);
            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\storage\\bar\\buz")).Returns(stream2);

            _sut.AddClassNode(node1Mock.Object, classInfo1);
            _sut.AddClassNode(node2Mock.Object, classInfo2);

            // We define that none of the files already existed (hence they are created)
            _fileSystemMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            // Act
            _sut.Save();

            // Assert
            _storageAccessMock.Verify(m => m.SaveNode(node1Mock.Object, It.IsAny<View>(), "barbuz", stream1));
            _storageAccessMock.Verify(m => m.SaveNode(node2Mock.Object, It.IsAny<View>(), "barbuz", stream2));
        }
    }
}
