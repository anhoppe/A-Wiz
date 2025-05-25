using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.SequenceDiagram
{
    [TestFixture]
    public class InteractionBehaviorTest
    {
        private Mock<IButton> _addMethodCallButtonMock = new();

        private Mock<IArchitectureView> _architectureViewMock = new();

        private Mock<IGraph> _graphMock = new();

        private Mock<INode> _lifelineNodeMock = new();

        private Mock<IMethodSelector> _methodSelectorMock = new();

        private Mock<IButton> _startCallSequenceButtonMock = new();

        private InteractionBehavior _sut = new();

        [SetUp]
        public void SetUp()
        {
            _methodSelectorMock = new();

            _sut = new()
            {
                MethodSelector = _methodSelectorMock.Object,
            };
            
            _architectureViewMock = new();
            _architectureViewMock.Setup(m => m.Graph).Returns(_graphMock.Object);
            
            _addMethodCallButtonMock = new();
            _lifelineNodeMock = new();
            _startCallSequenceButtonMock = new();

            _lifelineNodeMock.Setup(m => m.GetButtonById("AddMethodCall")).Returns(_addMethodCallButtonMock.Object);
            _lifelineNodeMock.Setup(m => m.GetButtonById("StartCallSequence")).Returns(_startCallSequenceButtonMock.Object);
        }

        [Test]
        public void ButtonEnabledState_WhenCalled_ThenButtonsCorrectlyInitialized()
        {
            // Arrange / Act
            _sut.AttachButtonBehavior(_architectureViewMock.Object, new ClassInfo(), _lifelineNodeMock.Object);

            // Assert
            _startCallSequenceButtonMock.VerifySet(p => p.Visible = true);
            _addMethodCallButtonMock.VerifySet(p => p.Visible = false);
        }

        [Test]
        public void AddMethodCall_WhenAddMethodCallIsClicked_ThenCreateMethodSelectionForPossibleCallSitesIsCalled()
        {
            // Arrange
            var methodInClass1 = new MethodInfo()
            {
                Name = "Method1",
            };
            var classInfo1 = new ClassInfo()
            {
                Methods = new List<MethodInfo>()
                {
                    methodInClass1,
                },
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            var methodInClass2 = new MethodInfo()
            {
                Name = "Method2",
            };
            var classInfo2 = new ClassInfo()
            {
                Methods = new List<MethodInfo>()
                {
                    methodInClass2,
                },
                Name = "Class2",
                Namespace = "MyNamespace",
            };

            // We need to setup the method selector to execute the callback that we pass
            _methodSelectorMock.Setup(m => m.CreateStartSequenceSelection(classInfo1.Methods, It.IsAny<Action<MethodInfo>>())).Callback<IList<MethodInfo>, Action<MethodInfo>>((methodInfoList, callback) =>
            {
                callback.Invoke(methodInClass1);
            });

            _sut.AttachButtonBehavior(_architectureViewMock.Object, classInfo1, _lifelineNodeMock.Object);

            // By calling the first button we selected a method to start a call squence with
            // Since we setup the MethodSelector to call the callback with the method of classInfo2
            // The event for the addMethodCallButton should be set
            _startCallSequenceButtonMock.Raise(m => m.Clicked += null, new EventArgs());

            // Act
            _addMethodCallButtonMock.Raise(m => m.Clicked += null, new EventArgs());

            // Assert
            _methodSelectorMock.Verify(m => m.CreateAddMethodCallSelection(methodInClass1, It.IsAny<Action<ClassInfo, MethodInfo>>()), Times.Once, "Expected that the method selector is called with the methods of the class that was selected in the context menu");
        }


        [Test]
        public void StartCallSequence_WhenClassNodeIsAddedAndStartCallSequenceIsRaised_ThenContextMenuIsShown()
        {
            // Arrange
            var classInfo = new ClassInfo()
            {
                Methods = new List<MethodInfo>()
                {
                    new MethodInfo()
                    {
                        Name = "Method1",
                    },
                },
                Name = "Class1",
                Namespace = "MyNamespace",
            };

            _sut.AttachButtonBehavior(_architectureViewMock.Object, classInfo, _lifelineNodeMock.Object);

            // Act
            _startCallSequenceButtonMock.Raise(m => m.Clicked += null, new EventArgs());

            // Assert
            _methodSelectorMock.Verify(m => m.CreateStartSequenceSelection(classInfo.Methods, It.IsAny<Action<MethodInfo>>()));
        }
    }
}
