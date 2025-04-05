using Gwiz.Core.Serializer;
using Newtonsoft.Json.Bson;
using NUnit.Framework;
using System.Linq;
using Wiz.Infrastructure.IO;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ViewReaderTest
    {
        private ViewReader sut = new();

        [SetUp]
        public void SetUp()
        {
            sut = new ViewReader();
        }

        [Test]
        public void AnnotationOptions_WhenReadingViewWithAnnotationOptions_ThenOptionsAreDetected()
        {
            // Arrange
            sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var graph = sut.GetViewByName("annotation_options");

            // Assert
            Assert.That(0, Is.EqualTo(graph.Edges.Count), "Expected all edges defined for the view in the graph");
        }

        [Test]
        public void GetUseCaseByName_WhenReadingExtendsImplementsProject_ThenTheUseCaseGrapgIsAvailable()
        {
            sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var graph = sut.GetUseCaseByName("base");

            // Assert
            Assert.That(graph.Nodes.Count, Is.EqualTo(4), "Expected all nodes defined for the use case in the graph");
            Assert.That(graph.Edges.Count, Is.EqualTo(2), "Expected all edges defined for the use case in the graph");
        }

        [Test]
        public void GetViewByName_WhenReadingExtendsImplementsProject_ThenTheGraphsForTheViewsAreAvailable()
        {
            // Arrange
            sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var graph = sut.GetViewByName("include_by_name");

            // Assert
            Assert.That(3, Is.EqualTo(graph.Nodes.Count), "Expected all nodes defined for the view in the graph");
            Assert.That(2, Is.EqualTo(graph.Edges.Count), "Expected all edges defined for the view in the graph");
        }

        [Test]
        public void Save_WhenReadingUseCaseAndSaving_ThenTheOriginalFileIsOverwritten()
        {
            // Arrange
            FileSystem fs = new();
            var tempPath = fs.CopyToTempPath("Assets\\ExtendsImplements\\");
            
            sut.ReadProject(tempPath);

            var graph = sut.GetUseCaseByName("base");

            graph.Nodes[0].X = 23;

            // Act
            sut.SaveView();

            // Assert
            var pathToUseCase = Path.Combine(tempPath, ".wiz");
            pathToUseCase = Path.Combine(pathToUseCase, "reqs");
            pathToUseCase = Path.Combine(pathToUseCase, "base.yaml");

            // Open the serialized file and check the content
            using (var stream = File.Open(pathToUseCase, FileMode.Open))
            {
                YamlSerializer serializer = new YamlSerializer();

                var adaptedGraph = serializer.Deserialize(stream);

                Assert.That(23, Is.EqualTo(adaptedGraph.Nodes[0].X), "Expected that the X value which was set in the test above is written to the files");
            }

            Directory.Delete(tempPath, true);
        }

        [Test]
        public void UseCases_WhenReadingExtendsImplementsProject_ThenUseCasesAreDetected()
        {
            // Arrange
            sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var useCases = sut.UseCases;

            // Assert
            Assert.That(useCases.Count, Is.EqualTo(1));
            Assert.That("base", Is.EqualTo(useCases[0]));
        }

        [Test]
        public void Views_WhenReadingExtendsImplementsProject_ThenProjectsInViewsAreDetected()
        {
            // Arrange
            sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var views = sut.Views;

            // Assert
            Assert.That(views.Count, Is.EqualTo(2));

            Assert.That(views.Contains("annotation_options"));
            Assert.That(views.Contains("include_by_name"));
        }
    }
}
