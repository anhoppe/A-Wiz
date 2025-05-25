using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    public class InteractionBehavior : IInteractionBehavior
    {
        internal IMethodSelector? MethodSelector { private get; set; }

        public void AttachButtonBehavior(IArchitectureView architectureView, ClassInfo classInfo, INode lifelineNode)
        {
            var graph = architectureView.Graph;
            if (graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (MethodSelector == null)
            {
                throw new InvalidOperationException("MethodSelector is not set");
            }

            var startCallSequenceButton = lifelineNode.GetButtonById("StartCallSequence");
            startCallSequenceButton.Visible = true;

            var addMethodCallButton = lifelineNode.GetButtonById("AddMethodCall");
            addMethodCallButton.Visible = false;

            startCallSequenceButton.Clicked += (sender, args) =>
            {
                var methodSelection = MethodSelector.CreateStartSequenceSelection(classInfo.Methods, (methodInfo) =>
                {
                    addMethodCallButton.Visible = true;
                    addMethodCallButton.Clicked += (sender, args) =>
                    {
                        var methodSelection = MethodSelector.CreateAddMethodCallSelection(methodInfo, (targetClass, calledMethod) => architectureView.AddMethodCall(classInfo, targetClass, calledMethod));
                        graph.ShowContextMenu(methodSelection);
                    };
                });

                graph.ShowContextMenu(methodSelection);
            };
        }

        public void UpdateAddMethodButtonBehavior(IArchitectureView architectureView, ClassInfo classInfo, MethodInfo methodInfo, INode lifelineNode)
        {
            var graph = architectureView.Graph;
            if (graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (MethodSelector == null)
            {
                throw new InvalidOperationException("MethodSelector is not set");
            }

            var addMethodCallButton = lifelineNode.GetButtonById("AddMethodCall");
            addMethodCallButton.Visible = true;
            addMethodCallButton.Clicked += (sender, args) =>
            {
                var methodSelection = MethodSelector.CreateAddMethodCallSelection(methodInfo, (targetClass, calledMethod) => architectureView.AddMethodCall(classInfo, targetClass, calledMethod));
                graph.ShowContextMenu(methodSelection);
            };
        }
    }
}
