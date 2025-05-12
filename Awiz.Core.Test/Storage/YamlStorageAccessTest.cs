using Awiz.Core.Contract.Git;
using Awiz.Core.Git;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;
using System.Text;

namespace Awiz.Core.Test.Storage
{
    [TestFixture]
    public class YamlStorageAccessTest
    {
        private YamlStorageAccess _sut = new();
        
        [Test]
        public void LoadGitNodeInfo_WhenStreamContainsGitInfo_ThenTheInfoIsFoundInReturnedObject2()
        {
            // Arrange
            var viewInfo =
                "foo:\n" +
                "    AssociatedCommits:\n" +
                "      - Author: foo\n" +
                "        Message: buz\n" +
                "        Sha: bar\n" +
                "      - Author: foofoo\n" +
                "        Message: buzbuz\n" +
                "        Sha: barbar\n" +
                "      - Author: foofoofoo\n" +
                "        Message: buzbuzbuz\n" +
                "        Sha: barbarbar\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(viewInfo));

            // Act
            var gitInfo = _sut.LoadGitInfo(stream);

            // Assert
            Assert.That(gitInfo["foo"].AssociatedCommits.Count, Is.EqualTo(3));
            Assert.That(gitInfo["foo"].AssociatedCommits[0].Message, Is.EqualTo("buz"));
            Assert.That(gitInfo["foo"].AssociatedCommits[1].Message, Is.EqualTo("buzbuz"));
            Assert.That(gitInfo["foo"].AssociatedCommits[2].Message, Is.EqualTo("buzbuzbuz"));
        }

        [Test]
        public void SaveGitInfo_WhenGitInfoIsSaved_ThenStreamReceivesExpectedContent()
        {
            // Arrange
            var stream = new MemoryStream(10);
            var gitInfo = new Dictionary<string, IGitNodeInfo>
            {
                { 
                    "foo", 
                    new GitNodeInfo(new List<Commit> 
                    { 
                        new Commit()
                        {
                            Author = "foo",
                            Message = "bar",
                            Sha = "buz",
                        },
                        new Commit() 
                        { 
                            Author = "foofoo",
                            Message = "barbar" ,
                            Sha = "buzbuz",
                        } 
                    }) 
                }
            };

            // Act
            _sut.SaveGitInfo(gitInfo, stream);
            
            // Assert
            var asString = Encoding.UTF8.GetString(stream.GetBuffer());
            string[] lines = asString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Assert.That("foo:", Is.EqualTo(lines[0]));
            Assert.That("  AssociatedCommits:", Is.EqualTo(lines[1]));
            Assert.That("  - Author: foo", Is.EqualTo(lines[2]));
            Assert.That("    Message: bar", Is.EqualTo(lines[3]));
            Assert.That("    Sha: buz", Is.EqualTo(lines[4]));
            Assert.That("  - Author: foofoo", Is.EqualTo(lines[5]));
            Assert.That("    Message: barbar", Is.EqualTo(lines[6]));
            Assert.That("    Sha: buzbuz", Is.EqualTo(lines[7]));
        }
    }
}
