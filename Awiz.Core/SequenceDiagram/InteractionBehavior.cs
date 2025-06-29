using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class InteractionBehavior : IInteractionBehavior
    {
        private EventHandler? _addMethodEventHandler = null;

        private EventHandler? _returnFromCallEventHandler = null;

        internal IGraph? Graph { private get; set; }

        internal IMethodSelector? MethodSelector { private get; set; }

        public void RemoveButtons(CallInfo callInfo)
        {
            DisableClassButtons(callInfo.TargetNode);

            _addMethodEventHandler = null;
            _returnFromCallEventHandler = null;
        }

        public void UpdateButtons(CallInfo callInfo, Action<ClassInfo, MethodInfo> addMethodCall, Action returnFromMethodCall)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (MethodSelector == null)
            {
                throw new InvalidOperationException("MethodSelector is not set");
            }

            DisableClassButtons(callInfo.SourceNode);

            _addMethodEventHandler = (sender, args) =>
            {
                var methodSelection = MethodSelector.CreateAddMethodCallSelection(callInfo.CalledMethod, addMethodCall);
                Graph.ShowContextMenu(methodSelection);
            };
            _returnFromCallEventHandler = (sender, args) =>
            {
                returnFromMethodCall();
            };

            EnableClassButtons(callInfo.TargetNode);
        }

        public void UpdateUserInitiaiteCallSequence(INode userLifelineNode, IList<ClassInfo> classesInDiagram, Action<ClassInfo, MethodInfo> createInitialCall)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (MethodSelector == null)
            {
                throw new InvalidOperationException("MethodSelector is not set");
            }

            var addMethodCallButton = userLifelineNode.GetButtonById("AddMethodCall");
            addMethodCallButton.Visible = true;

            addMethodCallButton.Clicked += (sender, args) =>
            {
                var methodSelection = MethodSelector.CreateStartSequenceSelection(classesInDiagram, createInitialCall);
                Graph.ShowContextMenu(methodSelection);
            };
        }

        private void EnableClassButtons(INode? node)
        {
            if (node == null)
            {
                throw new InvalidOperationException($"No lifeline node for target or source class set");
            }

            var addMethodCallButton = node.GetButtonById("AddMethodCall");
            addMethodCallButton.Visible = true;
            addMethodCallButton.Clicked += _addMethodEventHandler;

            var returnCallButton = node.GetButtonById("ReturnCall");
            returnCallButton.Visible = true;
            returnCallButton.Clicked += _returnFromCallEventHandler;
        }

        private void DisableClassButtons(INode? node)
        {
            if (node == null)
            {
                throw new InvalidOperationException($"No lifeline node for target or source class set");
            }

            var addMethodCallButton = node.GetButtonById("AddMethodCall");
            addMethodCallButton.Visible = false;

            if (_addMethodEventHandler != null)
            {
                addMethodCallButton.Clicked -= _addMethodEventHandler;
            }

            var returnCallButton = node.GetButtonById("ReturnCall");
            returnCallButton.Visible = false;

            if (_returnFromCallEventHandler != null)
            {
                returnCallButton.Clicked -= _returnFromCallEventHandler;
            }
        }

    }
}
