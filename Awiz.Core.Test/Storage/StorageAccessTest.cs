using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using System.Text;

namespace Awiz.Core.Test.Storage
{
    [TestFixture]
    public class StorageAccessTest
    {
        private StorageAccess _sut = new ();

        [Test]
        public void LoadNode_WhenStreamWithValidNodeInformationIsPassed_ThenTheInfoIsSetInNode()
        {
            // Arrange
            var viewInfo =
                "Views:\n" +
                "  foo_view:\n" +
                "    X: 10\n" +
                "    Y: 20\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(viewInfo));

            var nodeMock = new Mock<INode>();

            // Act
            _sut.LoadNode(nodeMock.Object, "foo_view", stream);

            // Assert
            nodeMock.VerifySet(p => p.X = 10);
            nodeMock.VerifySet(p => p.Y = 20);
        }

        [Test]
        public void LoadNode_WhenNodesAreSaved_ThenStreamReceivesExpectedContent()
        {
            // Arrange
            var stream = new MemoryStream(10);
            var nodeMock = new Mock<INode>();

            nodeMock.Setup(p => p.X).Returns(13);
            nodeMock.Setup(p => p.Y).Returns(23);

            // Act
            _sut.SaveNode(nodeMock.Object, new View(), "foo_view", stream);

            // Assert
            var asString = Encoding.UTF8.GetString(stream.GetBuffer());
            string[] lines = asString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Assert.That("Views:", Is.EqualTo(lines[0]));
            Assert.That("  foo_view:", Is.EqualTo(lines[1]));
            Assert.That("    X: 13", Is.EqualTo(lines[2]));
            Assert.That("    Y: 23", Is.EqualTo(lines[3]));
        }
    }
}
