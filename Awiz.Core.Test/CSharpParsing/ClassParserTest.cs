using NUnit.Framework;
using Moq;
using Awiz.Core.CSharpParsing;

namespace Awiz.Core.Test.CSharpParsing
{
    [TestFixture]
    public class ClassParserTest
    {
        Mock<IProjectParser> _projectParserMock = new();

        ClassParser _sut = new();

        [SetUp]
        public void SetUp()
        {
            _projectParserMock = new();

            _sut = new ClassParser()
            {
                ProjectParser = _projectParserMock.Object,
            };
        }

        [Test]
        public void AssemblyName_WhenProjectIsRead_ThenTheCorrectAssemblyNameIsAssigned()
        {
            // Arrange
            _projectParserMock.Setup(m => m.GetProject("Assets\\ExtendsImplements\\Class1.cs")).Returns("foo");

            // Act
            _sut.ParseClasses("Assets\\ExtendsImplements\\");

            // Assert
            var class1 = _sut.Classes.First(p => p.Name == "Class1");
            Assert.That(class1.Assembly, Is.EqualTo("foo"));
        }


        [Test]
        public void CallSite_WhenMethodIsPassed_ThenAllCallsToOtherObjectsAreReturned()
        {
            // Arrange
            _sut.ParseClasses("Assets\\ExtendsImplements\\");

            var class1 = _sut.Classes.First(p => p.Name == "Class1");
            var method = class1.Methods.First(m => m.Name == "MyFunc");

            // Act
            var callSites = _sut.GetCallSites(method);

            // Assert
            Assert.That(callSites.Count, Is.EqualTo(3));

            var class2 = _sut.Classes.First(p => p.Name == "Class2");
            Assert.That(callSites[0].Class, Is.EqualTo(class2));
            Assert.That(callSites[0].Method, Is.EqualTo(class2.Methods.First(p => p.Name == "MyOtherFunc")));
            
            var class3 = _sut.Classes.First(p => p.Name == "Class3");
            Assert.That(callSites[1].Class, Is.EqualTo(class3));
            Assert.That(callSites[1].Method, Is.EqualTo(class3.Methods.First(p => p.Name == "MyEvenMoreOtherFunc")));
            
            var interface2 = _sut.Classes.First(p => p.Name == "Interface2");
            Assert.That(callSites[2].Class, Is.EqualTo(interface2));
            Assert.That(callSites[2].Method, Is.EqualTo(interface2.Methods.First(p => p.Name == "ThisIsAnInterfaceMethod")));
        }
        
        [Test]
        public void CallSite_WhenMethodIsCalledTwice_ThenOnlyOneCallSiteGenerated()
        {
            // Arrange
            _sut.ParseClasses("Assets\\ExtendsImplements\\");

            var class1 = _sut.Classes.First(p => p.Name == "Class1");
            var method = class1.Methods.First(m => m.Name == "MyBetterFunc");

            // Act
            var callSites = _sut.GetCallSites(method);

            // Assert
            Assert.That(callSites.Count, Is.EqualTo(1));

            var class2 = _sut.Classes.First(p => p.Name == "Class2");
            Assert.That(callSites[0].Class, Is.EqualTo(class2));
            Assert.That(callSites[0].Method, Is.EqualTo(class2.Methods.First(p => p.Name == "MyOtherFunc")));
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

        [Test]
        public void Integration_ClassParser_ProjectParser()
        {
            // Arrange
            _sut = new ClassParser()
            {
                ProjectParser = new ProjectParser(),
            };

            // Act
            _sut.ParseClasses("Assets");

            // All classes should be in the same project
            var class1 = _sut.Classes.First(c => c.Directory == "Assets\\ExtendsImplements\\Class1.cs");
            var class2 = _sut.Classes.First(c => c.Directory == "Assets\\GenericSupport\\Class2.cs");

            Assert.That(class1.Assembly, Is.EqualTo("ExtendsImplements"));
            Assert.That(class2.Assembly, Is.EqualTo("GenericSupport"));
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

            var class3 = classInfos.First(c => c.Name == "SubClass");
            Assert.That(class3.Directory, Is.EqualTo("Assets\\ExtendsImplements\\Subdir\\SubClass.cs"));
        }
    }
}
