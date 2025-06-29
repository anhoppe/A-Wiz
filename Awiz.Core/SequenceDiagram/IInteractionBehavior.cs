using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Configures the interaction behavior of the buttons used in the sequence diagram
    /// </summary>
    internal interface IInteractionBehavior
    {
        /// <summary>
        /// Removes the buttons from the most recent method call on the call stack
        /// </summary>
        void RemoveButtons(CallInfo callInfo);

        /// <summary>
        /// Updates the buttons on the calstack to enable/disable
        /// </summary>
        /// <param name="addMethodCall"></param>
        /// <param name="returnFromMethodCall"></param>
        void UpdateButtons(CallInfo callInfo, Action<ClassInfo, MethodInfo> addMethodCall, Action returnFromMethodCall);

        /// <summary>
        /// Updates the buttons for the initial call sequence from the user lifeline
        /// </summary>
        /// <param name="userLifelineNode"></param>
        /// <param name="classesInDiagram"></param>
        /// <param name="createInitialCall"></param>
        void UpdateUserInitiaiteCallSequence(INode userLifelineNode, IList<ClassInfo> classesInDiagram, Action<ClassInfo, MethodInfo> createInitialCall);
    }
}
