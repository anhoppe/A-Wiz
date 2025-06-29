using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.SequenceDiagram
{
    [TestFixture]
    public class InteractionBehaviorTest
    {
        private Mock<IGraph> _graphMock = new();

        private Mock<IMethodSelector> _methodSelectorMock = new();

        private InteractionBehavior _sut = new();

        [SetUp]
        public void SetUp()
        {
            _graphMock = new();
            _methodSelectorMock = new();

            _sut = new ()
            {
                Graph = _graphMock.Object,
                MethodSelector = _methodSelectorMock.Object,
            };
        }

        [Test]
        public void UpdateUserInitiaiteCallSequence_WhenCalled_ThenUserLieflineAddMethodButtonSetVisible()
        {
            // Arrange
            var userLifelineNodeMock = new Mock<INode>();
            var buttonMock = new Mock<IButton>();

            userLifelineNodeMock.Setup(m =>m.GetButtonById("AddMethodCall")).Returns(buttonMock.Object);

            // Act
            _sut.UpdateUserInitiaiteCallSequence(userLifelineNodeMock.Object, [], (classInfo, methodInfo) => { });

            // Assert
            buttonMock.VerifySet(m => m.Visible = true, Times.Once, "Expected that the AddMethodCall button is set to visible when the user initiates a call sequence");
        }
    }
}
