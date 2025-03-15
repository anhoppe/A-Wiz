using Awiz.Core.CodeInfo;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassFilterTest
    {
        private Mock<IClassProvider> _classProviderMock = new();
        
        private ClassFilter _sut = new();

        [SetUp]
        public void SetUp()
        {
            _classProviderMock = new Mock<IClassProvider>();

            _sut = new ClassFilter();
        }

        [Test]
        public void Filter_WhenWhiteListIsEmpty_ThenClassIsNotFiltered()
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
            var result = _sut.Filter(_classProviderMock.Object);

            // Assert
            Assert.That(result.Classes.Count, Is.EqualTo(1));
            Assert.That(result.Classes[0].Name, Is.EqualTo("foo"));
        }

        [Test]
        public void Filter_WhenNamespaceIsBlacklisted_ThenClassIsFiltered()
        {
            // Arrange
            _sut.Namespaces.Blacklist.AddRange(new[] { "My.W1" }.ToList());

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
            var result = _sut.Filter(_classProviderMock.Object);

            // Assert
            Assert.That(!result.Classes.Any());
        }

        [Test]
        public void Filter_WhenNamespaceWhitelisted_ThenOnlyClassesInNamespacesGenerated()
        {
            // Arrange
            _sut.Namespaces.Whitelist.AddRange(new[] { "My.W1", "My.W2" });

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
            var result = _sut.Filter(_classProviderMock.Object);

            // Assert
            Assert.That(result.Classes.Count, Is.EqualTo(2));
            Assert.That(result.Classes[0].Name, Is.EqualTo("foo"));
            Assert.That(result.Classes[1].Name, Is.EqualTo("bar"));
        }

        [Test]
        public void Filter_WhenNamespaceBlackListed_ThenClassIsNotGenerated()
        {
            // Arrange
            _sut.Namespaces.Whitelist.AddRange(new[] { "My.W1", "My.W2" });
            _sut.Namespaces.Blacklist.AddRange(new[] { "My.W1" });

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
            var result = _sut.Filter(_classProviderMock.Object);

            // Assert
            Assert.That(result.Classes.Count, Is.EqualTo(1));
            Assert.That(result.Classes[0].Name, Is.EqualTo("bar"));
        }
    }
}
