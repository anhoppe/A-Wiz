using Moq;
using NUnit.Framework;
using Wiz.Infrastructure.IO;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ViewReaderTest
    {
        private Mock<ILoadableGitRepo> _loadableGitAccessMock = new();

        private ViewReader _sut = new();

        [SetUp]
        public void SetUp()
        {
            _loadableGitAccessMock = new();

            _sut = new ViewReader()
            {
                LoadableGitAccess = _loadableGitAccessMock.Object,
            };
        }

        [Test]
        public void AnnotationOptions_WhenReadingClassDiagramWithAnnotationOptions_ThenOptionsAreDetected()
        {
            // Arrange
            _sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var view = _sut.LoadClassDiagram("annotation_options");

            // Assert
            Assert.That(0, Is.EqualTo(view.Graph?.Edges.Count), "Expected all edges defined for the view in the graph");
        }

        [Test]
        public void LoadUseCase_WhenReadingExtendsImplementsProject_ThenTheUseCaseGrapgIsAvailable()
        {
            _sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var view = _sut.LoadUseCase("base");

            // Assert
            Assert.That(view.Graph?.Nodes.Count, Is.EqualTo(4), "Expected all nodes defined for the use case in the graph");
            Assert.That(view.Graph?.Edges.Count, Is.EqualTo(2), "Expected all edges defined for the use case in the graph");
        }

        [Test]
        public void LoadClassDiagram_WhenReadingExtendsImplementsProject_ThenTheGraphsForTheViewsAreAvailable()
        {
            // Arrange
            _sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var view = _sut.LoadClassDiagram("include_by_name");

            // Assert
            Assert.That(3, Is.EqualTo(view.Graph?.Nodes.Count), "Expected all nodes defined for the view in the graph");
            Assert.That(2, Is.EqualTo(view.Graph?.Edges.Count), "Expected all edges defined for the view in the graph");
        }

        [Test]
        public void Save_WhenReadingUseCaseAndSaving_ThenTheOriginalFileIsOverwritten()
        {
            // Arrange
            FileSystem fs = new();
            var tempPath = fs.CopyToTempPath("Assets\\ExtendsImplements\\");
            
            _sut.ReadProject(tempPath);

            var view = _sut.LoadUseCase("base");


            if (view.Graph == null)
            {
                throw new NullReferenceException("Graph is null");
            }
            view.Graph.Nodes[0].X = 23;

            // Act
            view.Save();

            // Assert

            // Create a new view reader to read the project again
            var viewReader = new ViewReader()
            {
                LoadableGitAccess = _loadableGitAccessMock.Object,
            };

            viewReader.ReadProject(tempPath);
            var adaptedView = _sut.LoadUseCase("base");
            Assert.That(adaptedView.Graph?.Nodes[0].X, Is.EqualTo(23), "Expected that the X value which was set in the test above is written to the files");

            Directory.Delete(tempPath, true);
        }

        [Test]
        public void UseCases_WhenReadingExtendsImplementsProject_ThenUseCasesAreDetected()
        {
            // Arrange
            _sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var useCases = _sut.UseCases;

            // Assert
            Assert.That(useCases.Count, Is.EqualTo(1));
            Assert.That("base", Is.EqualTo(useCases[0]));
        }

        [Test]
        public void ClassDiagrams_WhenReadingExtendsImplementsProject_ThenProjectsInViewsAreDetected()
        {
            // Arrange
            _sut.ReadProject("Assets\\ExtendsImplements\\");

            // Act
            var views = _sut.ClassDiagrams;

            // Assert
            Assert.That(views.Count, Is.EqualTo(2));

            Assert.That(views.Contains("annotation_options"));
            Assert.That(views.Contains("include_by_name"));
        }
    }
}
