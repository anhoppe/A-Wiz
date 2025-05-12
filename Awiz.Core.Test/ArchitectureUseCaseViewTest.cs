using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.Git;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using Wiz.Infrastructure.IO;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ArchitectureUseCaseViewTest
    {
        Mock<IFileSystem> _fileSystemMock = new();

        Mock<IStorageAccess> _storageAccessMock = new();

        ArchitectureUseCaseView _sut = new();

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new();
            _storageAccessMock = new();

            _sut = new ArchitectureUseCaseView()
            {
                FileSystem = _fileSystemMock.Object,
                StorageAccess = _storageAccessMock.Object,
            };
        }

        [Test]
        public void AddNUseCaseNode_WhenUseCaseNodeAdded_ThenNodeAddedEventIsRaised()
        {
            // Arrange
            var nodeMock = new Mock<INode>();
            nodeMock.Setup(p => p.Id).Returns("1");

            bool wasCalled = false;
            _sut.NodeAdded += (sender, node) =>
            {
                if (node == nodeMock.Object)
                {
                    wasCalled = true;
                }
            };

            // Act
            _sut.AddUseCaseNode(nodeMock.Object);

            // Assert
            Assert.That(wasCalled);
        }

        [Test]
        public void LoadUseCaseInfo_WhenUseCaseIsLoaded_ThenGitInfoIsLoaded()
        {
            // Arrange
            _sut.RepoPath = "c:\\dir\\foo\\repo";
            _sut.Name = "foobar";

            Stream stream = new MemoryStream();
            _fileSystemMock.Setup(m => m.Exists("c:\\dir\\foo\\repo\\.wiz\\req_storage\\foobar")).Returns(true);
            _fileSystemMock.Setup(m => m.OpenRead("c:\\dir\\foo\\repo\\.wiz\\req_storage\\foobar")).Returns(stream);

            var gitInfoMock = new Mock<IGitNodeInfo>();

            // Act
            _sut.Load(Mock.Of<IVersionUpdater>());

            // Assert
            _storageAccessMock.Verify(m => m.LoadGitInfo(stream));
        }

        [Test]
        public void AddUseCaseNode_WhenGitInfoIsRequestedForNodeWithoutGitInfo_ThenEmptyGitInfoIsReturned()
        {
            // Act
            var nodeMock = new Mock<INode>();
            nodeMock.Setup(p => p.Id).Returns("foo");

            // Arrange
            _sut.AddUseCaseNode(nodeMock.Object);

            // Assert
            var gitInfo = _sut.GetAssociatedCommits(nodeMock.Object);
            Assert.That(gitInfo.AssociatedCommits.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetGitInfoForNode_WhenGitInfoIsRequestedForNodeWithoutGitInfo_ThenEmptyGitInfoIsReturned()
        {
            // Act
            var nodeMock = new Mock<INode>();
            nodeMock.Setup(p => p.Id).Returns("foo");

            // Arrange
            var gitInfo = _sut.GetAssociatedCommits(nodeMock.Object);

            // Assert
            Assert.That(gitInfo.AssociatedCommits.Count, Is.EqualTo(0));
        }

        [Test]
        public void Save_WhenUseCaseIsSaved_ThenGitInfoIsSaved()
        {
            // Arrange
            _sut.RepoPath = "c:\\dir\\foo\\repo";
            _sut.Name = "foobar";
            _sut.Graph = Mock.Of<IGraph>();
            _sut.Serializer = Mock.Of<ISerializer>();

            Stream stream = new MemoryStream();
            _fileSystemMock.Setup(m => m.Exists("c:\\dir\\foo\\repo\\.wiz\\req_storage\\foobar")).Returns(true);
            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\req_storage\\foobar")).Returns(stream);
            _fileSystemMock.Setup(m => m.Create("c:\\dir\\foo\\repo\\.wiz\\reqs\\foobar")).Returns(stream);

            // Act
            _sut.Save();

            // Assert
            _storageAccessMock.Verify(m => m.SaveGitInfo(It.IsAny<Dictionary<string, IGitNodeInfo>>(), It.IsAny<Stream>()));
        }
    }
}
