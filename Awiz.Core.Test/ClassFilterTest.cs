using Awiz.Core.CodeInfo;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassFilterTest
    {
        private Mock<IClassProvider> _classProviderMock = new();
        
        [SetUp]
        public void SetUp()
        {
            _classProviderMock = new Mock<IClassProvider>();
        }

        [Test]
        public void ctor_WhenPathToRepoAndViewNamePassed_ThenTheFilterIsAppliedAsDefinedInTheView()
        {
            // Arrange
            var class1 = new ClassInfo()
            {
                Directory = "Assets\\ExtendsImplements\\Class1.cs",
                Namespace = "Awiz.Core.Test.Assets.ExtendsImplements",
                Name = "Class1",
            };

            var class2 = new ClassInfo()
            {
                Directory = "Assets\\ExtendsImplements\\Class2.cs",
                Namespace = "Awiz.Core.Test.Assets.ExtendsImplements",
                Name = "Class2",
            };

            var class3 = new ClassInfo()
            {
                Directory = "Assets\\ExtendsImplements\\Subdir\\Class3.cs",
                Namespace = "Awiz.Core.Test.Assets.ExtendsImplements.Subdir",
                Name = "Class3",
            };

            var interface1 = new ClassInfo()
            {
                Directory = "Assets\\ExtendsImplements\\Interface1.cs",
                Namespace = "Awiz.Core.Test.Assets.ExtendsImplements",
                Name = "Interface1",
            };

            var interface2 = new ClassInfo()
            {
                Directory = "Assets\\ExtendsImplements\\Interface2.cs",
                Namespace = "Awiz.Core.Test.Assets.ExtendsImplements",
                Name = "Interface2",
            };

            var classProviderMock = new Mock<IClassProvider>();
            classProviderMock.Setup(p => p.Classes).Returns([
                class1,
                class2,
                class3,
                interface1,
                interface2,
                ]);

            // Act
            var sut = new ClassFilter("Assets\\ExtendsImplements\\", "include_by_name");
            var result = sut.Filter(classProviderMock.Object);

            // Assert
            Assert.That(result.Classes.Count, Is.EqualTo(3));
            Assert.That(result.Classes[0].Name, Is.EqualTo("Class1"));
            Assert.That(result.Classes[1].Name, Is.EqualTo("Class2"));
            Assert.That(result.Classes[2].Name, Is.EqualTo("Class3"));
        }
    }
}
