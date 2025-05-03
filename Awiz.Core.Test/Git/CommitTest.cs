using Awiz.Core.Contract.Git;
using NUnit.Framework;

namespace Awiz.Core.Test.Git
{
    [TestFixture]
    public class CommitTest
    {
        [Test]
        public void ShortSha_WhenShaIsLongerThan7_ThenOnlyFirst7CharsAreReturned()
        {
            // Arrange
            var commit = new Commit()
            {
                Sha = "1234567890abcdef"
            };

            // Act
            var shortSha = commit.ShortSha();
            
            // Assert
            Assert.That(shortSha, Is.EqualTo("1234567"));
        }

        [Test]
        public void ShortSha_WhenShaHasLength7_Then7CharsAreReturned()
        {
            // Arrange
            var commit = new Commit()
            {
                Sha = "1234567"
            };

            // Act
            var shortSha = commit.ShortSha();
            
            // Assert
            Assert.That(shortSha, Is.EqualTo("1234567"));
        }

        [Test]
        public void ShortSha_WhenShaIsShorterThen7_ThenOnlyAvailableCharactersAreRetturned()
        {
            // Arrange
            var commit = new Commit()
            {
                Sha = "123"
            };

            // Act
            var shortSha = commit.ShortSha();
            
            // Assert
            Assert.That(shortSha, Is.EqualTo("123"));
        }
    }
}
