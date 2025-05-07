using NUnit.Framework;
using Awiz.Core.CSharpClassGenerator;

namespace Awiz.Core.Test.CSharpClassGenerator
{
    [TestFixture]
    public class ClassParserTest
    {
        ClassParser _sut = new();

        [SetUp]
        public void SetUp()
        {
            _sut = new ClassParser();
        }

        [Test]
        public void ExtendsImplements_WhenClassExtendsClassAndClassImplementsInterface_ThenImplementedInterfacesAndBaseClassPropertiesSet()
        {
            // Arrange
            _sut.ParseClasses("Assets/ExtendsImplements/");

            // Act
            var classInfos = _sut.Classes;

            // Assert
            var class1 = classInfos.First(c => c.Name == "Class1");
            Assert.That(class1.ImplementedInterfaces.Count, Is.EqualTo(1));
            Assert.That(class1.ImplementedInterfaces[0], Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Interface1"));
            Assert.That(class1.Id, Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Class1"));

            var class2 = classInfos.First(c => c.Name == "Class2");
            Assert.That(class2.BaseClass, Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Class1"));

            var interface2 = classInfos.First(c => c.Name == "Interface2");
            Assert.That(interface2.ImplementedInterfaces.Count, Is.EqualTo(1));
            Assert.That(interface2.ImplementedInterfaces[0], Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Interface1"));
            Assert.That(interface2.Id, Is.EqualTo("Awiz.Core.Test.Assets.ExtendsImplements.Interface2"));
        }

        [Test]
        public void PathInClassInfo_WhenClassesAreParsed_ThenClassInfoContainsCorrectDirectory()
        {
            // Arrange
            _sut.ParseClasses("Assets\\ExtendsImplements\\");

            // Act
            var classInfos = _sut.Classes;

            // Assert
            var class1 = classInfos.First(c => c.Name == "Class1");
            Assert.That(class1.Directory, Is.EqualTo("Assets\\ExtendsImplements\\Class1.cs"));

            var class3 = classInfos.First(c => c.Name == "Class3");
            Assert.That(class3.Directory, Is.EqualTo("Assets\\ExtendsImplements\\Subdir\\Class3.cs"));
        }

        [Test]
        public void GenericProperties_WhenClassContainsEnumerableType_ThenPropertyContainsEnumerableTypeInformation()
        {
            // Arrange
            _sut.ParseClasses("Assets\\GenericSupport\\");

            // Act
            var classInfos = _sut.Classes;

            // Assert
            var class1 = classInfos.First(c => c.Name == "Class1");
            var properties = class1.Properties;

            Assert.That(properties.Count, Is.EqualTo(3));
            Assert.That(properties[0].GenericType.Name, Is.EqualTo("String"));
            Assert.That(properties[0].IsEnumerable);
            Assert.That(properties[1].GenericType.Name, Is.EqualTo("Int32"));
            Assert.That(properties[1].IsEnumerable);
            Assert.That(properties[2].GenericType.Name, Is.EqualTo("Class2"));
            Assert.That(properties[2].GenericType.Namespace, Is.EqualTo("Awiz.Core.Test.Assets.GenericSupport"));
            Assert.That(properties[2].IsEnumerable);
        }
    }
}
