using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using NUnit.Framework;

namespace Awiz.Core.Test.CodeTree
{
    [TestFixture]
    public class NamespaceBuilderTest
    {
        private NamespaceBuilder _sut = new();

        [SetUp]
        public void SetUp()
        {
            _sut = new();
        }

        [Test]
        public void Build_When2ClassesInRootNamespace_ThenTheClassesAreInTheRootNode()
        {
            // Arrange
            var classInfos = new List<ClassInfo>
            {
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class1",
                    Namespace = "Namespace1"
                },
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class2",
                    Namespace = "Namespace1"
                },
            };

            // Act
            var roots = _sut.Build(classInfos);

            // Assert
            Assert.That(roots.Count, Is.EqualTo(1));
            Assert.That(roots["foo"].Name, Is.EqualTo("Namespace1"));
            Assert.That(roots["foo"].Classes.Count, Is.EqualTo(2));
            Assert.That(roots["foo"].Classes[0].Name, Is.EqualTo("Class1"));
            Assert.That(roots["foo"].Classes[1].Name, Is.EqualTo("Class2"));
            Assert.That(roots["foo"].Children.Count, Is.EqualTo(0));
        }

        [Test]
        public void Build_When2ClassesAreInDifferentRootNamespaces_ThenRootsHasTwoNodes()
        {
            // Arrange
            var classInfos = new List<ClassInfo>
            {
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class1",
                    Namespace = "Namespace1"
                },
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class2",
                    Namespace = "Namespace2"
                },
            };

            // Act
            var roots = _sut.Build(classInfos);

            // Assert
            Assert.That(roots.Count, Is.EqualTo(1));
            Assert.That(roots["foo"].Name, Is.EqualTo("Namespace1"));
            Assert.That(roots["foo"].Classes.Count, Is.EqualTo(2));
            Assert.That(roots["foo"].Classes[0].Name, Is.EqualTo("Class1"));
            Assert.That(roots["foo"].Classes[1].Name, Is.EqualTo("Class2"));
            Assert.That(roots["foo"].Children.Count, Is.EqualTo(0));
        }

        [Test]
        public void Build_WhenClassesAreInNamespaceHierarchy_ThenNamespaceNodesAreCorrectlyCreated()
        {
            // Arrange
            var classInfos = new List<ClassInfo>
            {
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class1",
                    Namespace = "Namespace1"
                },
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class2",
                    Namespace = "Namespace1.Sub2"
                },
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class3",
                    Namespace = "Namespace1.Sub2"
                },
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class4",
                    Namespace = "Namespace1.Sub2.Sub3"
                },
            };

            // Act
            var roots = _sut.Build(classInfos);

            // Assert
            Assert.That(roots.Count, Is.EqualTo(1));

            Assert.That(roots["foo"].Children.Count, Is.EqualTo(1));
            Assert.That(roots["foo"].Name, Is.EqualTo("Namespace1"));
            Assert.That(roots["foo"].Classes.Count, Is.EqualTo(1));
            Assert.That(roots["foo"].Classes[0].Name, Is.EqualTo("Class1"));
            Assert.That(roots["foo"].Children.Count, Is.EqualTo(1));

            Assert.That(roots["foo"].Children[0].Name, Is.EqualTo("Sub2"));
            Assert.That(roots["foo"].Children[0].Classes.Count, Is.EqualTo(2));
            Assert.That(roots["foo"].Children[0].Classes[0].Name, Is.EqualTo("Class2"));
            Assert.That(roots["foo"].Children[0].Classes[1].Name, Is.EqualTo("Class3"));
            Assert.That(roots["foo"].Children[0].Children.Count, Is.EqualTo(1));

            Assert.That(roots["foo"].Children[0].Children[0].Name, Is.EqualTo("Sub3"));
        }

        [Test]
        public void MultipleAssemblies_WhenClassesAreInDifferentAssemblies_ThenMultipleRootNodesForTheAssembliesAreCreated()
        {
            // Arrange
            var classInfos = new List<ClassInfo>
            {
                new ClassInfo()
                {
                    Assembly = "foo",
                    Name = "Class1",
                    Namespace = "Namespace1"
                },
                new ClassInfo()
                {
                    Assembly = "bar",
                    Name = "Class2",
                    Namespace = "Namespace1"
                },
            };

            // Act
            var roots = _sut.Build(classInfos);

            // Assert
            Assert.That(roots.Count, Is.EqualTo(2));
            Assert.That(roots["foo"].Name, Is.EqualTo("Namespace1"));
            Assert.That(roots["bar"].Name, Is.EqualTo("Namespace1"));
        }
    }
}
