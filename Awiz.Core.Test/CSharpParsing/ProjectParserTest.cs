using Awiz.Core.CSharpParsing;
using NUnit.Framework;

namespace Awiz.Core.Test.CSharpParsing
{
    [TestFixture]
    public class ProjectParserTest
    {
        [Test]
        public void Parse_WhenProjectIsParsed_ThenSourceFilesAreAssignedToCorrectProjects()
        {
            // Arrange
            var sut = new ProjectParser();

            // Act
            sut.ParseProject("Assets/");

            // Assert
            Assert.That(sut.GetProject("Assets\\ExtendsImplements\\Subdir\\Class3.cs"), Is.EqualTo("ExtendsImplements"));
            Assert.That(sut.GetProject("Assets\\GenericSupport\\Class2.cs"), Is.EqualTo("GenericSupport"));
        }
    }
}
