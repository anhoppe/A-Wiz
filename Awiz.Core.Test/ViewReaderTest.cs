using NUnit.Framework;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class ViewReaderTest
    {
        [Test]
        public void Views_WhenReadingExtendsImplementsProject_ThenProjectsInViewsAreDetected()
        {
            // Arrange
            var sut = new ViewReader();
            sut.Read("Assets\\ExtendsImplements\\");

            // Act
            var views = sut.Views;

            // Assert
            Assert.That(2, Is.EqualTo(views.Count));

            Assert.That(views.Contains("annotation_options"));
            Assert.That(views.Contains("include_by_name"));
        }

        [Test]
        public void GetViewByName_WhenReadingExtendsImplementsProject_ThenTheGraphsForTheViewsAreAvailable()
        {
            // Arrange
            var sut = new ViewReader();
            sut.Read("Assets\\ExtendsImplements\\");

            // Act
            var graph = sut.GetViewByName("include_by_name");

            // Assert
            Assert.That(3, Is.EqualTo(graph.Nodes.Count), "Expected all nodes defined for the view in the graph");
            Assert.That(2, Is.EqualTo(graph.Edges.Count), "Expected all edges defined for the view in the graph");
        }

        [Test]
        public void AnnotationOptions_WhenReadingViewWithAnnotationOptions_ThenOptionsAreDetected()
        {
            // Arrange
            var sut = new ViewReader();
            sut.Read("Assets\\ExtendsImplements\\");

            // Act
            var graph = sut.GetViewByName("annotation_options");

            // Assert
            Assert.That(0, Is.EqualTo(graph.Edges.Count), "Expected all edges defined for the view in the graph");
        }
    }
}
