using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ClassParserTest
    {
        [Test]
        public void ExtendsImplements_WhenClassExtendsClassAndClassImplementsInterface_ThenImplementedInterfacesAndBaseClassPropertiesSet()
        {
            // Arrange
            var parser = new ClassParser();
            parser.ParseClasses("Assets/ExtendsImplements/");

            // Act
            var classInfos = parser.Classes;

            // Assert
            var class1 = classInfos.First(c => c.Name == "Class1");
            Assert.That(class1.ImplementedInterfaces.Count, Is.EqualTo(1));
            Assert.That(class1.ImplementedInterfaces[0], Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Interface1"));

            var class2 = classInfos.First(c => c.Name == "Class2");
            Assert.That(class2.BaseClass, Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Class1"));

            var interface2 = classInfos.First(c => c.Name == "Interface2");
            Assert.That(interface2.ImplementedInterfaces.Count, Is.EqualTo(1));
            Assert.That(interface2.ImplementedInterfaces[0], Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Interface1"));
        }

        [Test]
        public void PathInClassInfo_WhenClassesAreParsed_ThenClassInfoContainsCorrectDirectory()
        {
            // Arrange
            var parser = new ClassParser();
            parser.ParseClasses("Assets\\ExtendsImplements\\");

            // Act
            var classInfos = parser.Classes;

            // Assert
            var class1 = classInfos.First(c => c.Name == "Class1");
            Assert.That(class1.Directory, Is.EqualTo("Assets\\ExtendsImplements\\Class1.cs"));

            var class3 = classInfos.First(c => c.Name == "Class3");
            Assert.That(class3.Directory, Is.EqualTo("Assets\\ExtendsImplements\\Subdir\\Class3.cs"));
        }
    }
}
