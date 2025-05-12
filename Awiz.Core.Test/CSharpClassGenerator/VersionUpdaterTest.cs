using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using NUnit.Framework;

namespace Awiz.Core.Test.CSharpClassGenerator
{
    [TestFixture]
    public class VersionUpdaterTest
    {
        [Test]
        public void DeleteAddedSuggestion_WhenAddedMethodIsDeleted_ThenAddedMethodIsRemovedFromClassInfo()
        {
            // Arrange
            var methodInfo = new MethodInfo()
            {
                Name = "bar",
            };
        
            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedMethods = [methodInfo],
                Methods = [],
            };

            var sut = new VersionUpdater();
            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.AddedMethods.Count, Is.EqualTo(0));
                Assert.That(classInfo.Methods.Count, Is.EqualTo(0));
            };
            
            // Act
            sut.DeleteAddedSuggestion(updatedClassInfo, methodInfo);
            
            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void DeleteAddedSuggestion_WhenAddedPropertyIsDeleted_ThenAddedPropertyIsRemovedFromClassInfo()
        {
            // Arrange
            var proeprtyInfo = new PropertyInfo()
            {
                Name = "bar",
            };
        
            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedProperties = [proeprtyInfo],
                Properties = [],
            };

            var sut = new VersionUpdater();
            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.AddedProperties.Count, Is.EqualTo(0));
                Assert.That(classInfo.Properties.Count, Is.EqualTo(0));
            };
            
            // Act
            sut.DeleteAddedSuggestion(updatedClassInfo, proeprtyInfo);
            
            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void DeleteDeletedSuggestion_WhenDeletedMethodSuggestinIsDeleted_ThenDeletedMethodIsRemovedFromClassInfo()
        {
            // Arrange
            var methodInfo = new MethodInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                DeletedMethods = [methodInfo],
                Methods = [methodInfo],
            };

            var sut = new VersionUpdater();
            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.DeletedMethods.Count, Is.EqualTo(0));
                Assert.That(classInfo.Methods.Count, Is.EqualTo(1));
            };

            // Act
            sut.DeleteDeletedSuggestion(updatedClassInfo, methodInfo);

            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void DeleteDeletedSuggestion_WhenDeletedPropertySuggestionIsDeleted_ThenDeletedPropertyIsRemovedFromClassInfo()
        {
            // Arrange
            var proeprtyInfo = new PropertyInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                DeletedProperties = [proeprtyInfo],
                Properties = [proeprtyInfo],
            };

            var sut = new VersionUpdater();
            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.DeletedProperties.Count, Is.EqualTo(0));
                Assert.That(classInfo.Properties.Count, Is.EqualTo(1));
            };

            // Act
            sut.DeleteDeletedSuggestion(updatedClassInfo, proeprtyInfo);

            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void PropertyAdded_WhenAPropertyWasAdded_ThenAddedPropertiesIsSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };

            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Properties =
                [
                    new PropertyInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.AddedProperties.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.AddedProperties[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void PropertyAdded_WhenAPropertyWasAddedButNewPropertyAlreadyInAddedProperties_ThenAddedPropertiesIsNotSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedProperties =
                [
                    new PropertyInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };

            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Properties =
                [
                    new PropertyInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.AddedProperties.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.AddedProperties[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void PropertyDeleted_WhenPropertyWasDeleted_ThenDeletedPropertiesIsSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Properties =
                [
                     new PropertyInfo()
                     {
                         Name = "foobar",
                     }
                ],
            };

            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.DeletedProperties.Count, Is.EqualTo(1));
        }

        [Test]
        public void PropertyDeleted_WhenPropertyWasDeletedButDeletedPropertyAlreadyContained_ThenDeletedPropertiesIsNotSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Properties =
                [
                     new PropertyInfo()
                     {
                         Name = "foobar",
                     }
                ],
                DeletedProperties =
                [
                    new PropertyInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };

            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.DeletedProperties.Count, Is.EqualTo(1));
        }

        [Test]
        public void MethodAdded_WhenMethodWasAdded_ThenAddedMethodsIsSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };
            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Methods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };
            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.AddedMethods.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.AddedMethods[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void MethodAdded_WhenMethodWasAddedButAddedMethodAlreadContaind_ThenAddedMethodsIsNotSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedMethods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };
            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Methods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };
            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.AddedMethods.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.AddedMethods[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void MethodDeleted_WhenMethodWasDeleted_ThenDeletedMethodsIsSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Methods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };
            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.DeletedMethods.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.DeletedMethods[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void MethodDeleted_WhenMethodWasDeletedButDeletedMethodAlreadyContained_ThenDeletedMethodsIsNotSet()
        {
            // Arrange
            var classInfoOld = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Methods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
                DeletedMethods =
                [
                    new MethodInfo()
                    {
                        Name = "foobar",
                    }
                ],
            };
            var classInfoNew = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
            };

            var sut = new VersionUpdater();

            // Act
            sut.CheckVersionUpdates(classInfoOld, [classInfoNew]);

            // Assert
            Assert.That(classInfoOld.DeletedMethods.Count, Is.EqualTo(1));
            Assert.That(classInfoOld.DeletedMethods[0].Name, Is.EqualTo("foobar"));
        }

        [Test]
        public void UpdateAdd_WhenMethodAdded_ThenMethodsIsAddedToClassInfoAndClassInfoChangedIdRaised()
        {
            // Arrange
            var methodInfo = new MethodInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedMethods = [methodInfo],
            };

            var sut = new VersionUpdater();

            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.AddedMethods.Count, Is.EqualTo(0), "Expected method to be removed from suggestions");
                Assert.That(classInfo.Methods.Count, Is.EqualTo(1));
                Assert.That(classInfo.Methods[0].Name, Is.EqualTo("bar"));
            };

            // Act
            sut.UpdateAdd(updatedClassInfo, methodInfo);

            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void UpdateAdd_WhenPropertyAdded_ThenPropertyIsAddedToClassInfoAndClassInfoChangedIdRaised()
        {
            // Arrange
            var propertyInfo = new PropertyInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                AddedProperties = [propertyInfo]
            };

            var sut = new VersionUpdater();

            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.AddedProperties.Count, Is.EqualTo(0));
                Assert.That(classInfo.Properties.Count, Is.EqualTo(1));
                Assert.That(classInfo.Properties[0].Name, Is.EqualTo("bar"));
            };

            // Act
            sut.UpdateAdd(updatedClassInfo, propertyInfo);

            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void UpdateDeleted_WhenPropertyDeleted_ThenPropertyIsDeletedFromClassInfoAndClassInfoChangedIdRaised()
        {
            // Arrange
            var propertyInfo = new PropertyInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Properties = [propertyInfo],
                DeletedProperties = [propertyInfo]
            };

            var sut = new VersionUpdater();

            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.DeletedProperties.Count, Is.EqualTo(0));
                Assert.That(classInfo.Properties.Count, Is.EqualTo(0));
            };

            // Act
            sut.UpdateDelete(updatedClassInfo, propertyInfo);

            // Assert
            Assert.That(updateCalled);
        }

        [Test]
        public void UpdateDeleted_WhenMethodDeleted_ThenMethodIsDeletedFromClassInfoAndClassInfoChangedIdRaised()
        {
            // Arrange
            var methodInfo = new MethodInfo()
            {
                Name = "bar",
            };

            var updatedClassInfo = new ClassInfo()
            {
                Name = "foo",
                Namespace = "My.Namespace",
                Methods = [methodInfo],
                DeletedMethods = [methodInfo]
            };

            var sut = new VersionUpdater();

            bool updateCalled = false;
            sut.ClassUpdated += (sender, classInfo) =>
            {
                updateCalled = true;
                Assert.That(classInfo.DeletedMethods.Count, Is.EqualTo(0));
                Assert.That(classInfo.Methods.Count, Is.EqualTo(0));
            };

            // Act
            sut.UpdateDelete(updatedClassInfo, methodInfo);

            // Assert
            Assert.That(updateCalled);
        }
    }
}
