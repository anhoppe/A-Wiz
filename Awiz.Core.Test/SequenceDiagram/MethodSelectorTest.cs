using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;
using Awiz.Core.SequenceDiagram;
using Gwiz.Core.Contract;
using Moq;
using NUnit.Framework;

namespace Awiz.Core.Test.SequenceDiagram
{
    [TestFixture]
    public class MethodSelectorTest
    {
        private Mock<ISourceCode> _sourceCodeMock = new();

        private MethodSelector _sut = new();

        [SetUp]
        public void SetUp()
        {
            _sourceCodeMock = new();

            _sut = new MethodSelector()
            {
                SourceCode = _sourceCodeMock.Object,
            };
        }

        [Test]
        public void CreateStartSequenceSelection_WhenMethodListIsPassed_ThenMethodsInTheSelection()
        {
            // Arrange
            var methods = new List<MethodInfo>
            {
                new MethodInfo { Name = "Method1" },
                new MethodInfo { Name = "Method2" }
            };

            // Act
            var contextMenuItems =_sut.CreateStartSequenceSelection(methods, method => { });

            // Assert
            Assert.That(contextMenuItems.Count, Is.EqualTo(2));
            Assert.That(contextMenuItems[0].Name, Is.EqualTo("Method1"));
            Assert.That(contextMenuItems[1].Name, Is.EqualTo("Method2"));
        }

        [Test]
        public void CreateStartSequenceSelection_WhenMethodIsPassed_ThenCallbackIsAssigned()
        {
            // Arrange
            var methods = new List<MethodInfo>
            {
                new MethodInfo { Name = "Method1" },
            };

            bool callbackInvoked = false;

            // Act
            var contextMenuItems = _sut.CreateStartSequenceSelection(methods, method =>
            {
                if (method.Name == "Method1")
                {
                    callbackInvoked = true;
                }
            });
            contextMenuItems[0].Callback.Invoke();

            // Assert
            Assert.That(callbackInvoked);
        }

        [Test]
        public void CreateAddMethodCallSelection_WhenMethodIsPassed_ThenCallSitesOfMethodAreShown()
        {
            // Arrange
            var calledMethod = new MethodInfo { Name = "CalledMethod" };
            
            var callSites = new List<CallSite>
            {
                new CallSite(new ClassInfo { Name = "Class1" }, new MethodInfo() { Name = "Method1" }),
                new CallSite(new ClassInfo { Name = "Class2" }, new MethodInfo() { Name = "Method2" })
            };

            _sourceCodeMock.Setup(p => p.GetCallSites(calledMethod)).Returns(callSites);

            // Act
            var contextMenuItems = _sut.CreateAddMethodCallSelection(calledMethod, (targetClass, calledMethod) => { });

            // Assert
            Assert.That(contextMenuItems.Count, Is.EqualTo(2));
        }
    }
}
