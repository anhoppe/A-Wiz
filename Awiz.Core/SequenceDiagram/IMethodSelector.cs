using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Prepares the selection of methods for the context menu in sequence diagrams
    /// </summary>
    internal interface IMethodSelector
    {
        /// <summary>
        /// Creates a selection of methods in the list for direct selection
        /// </summary>
        /// <param name="methods"></param>
        IList<ContextMenuItem> CreateStartSequenceSelection(IList<MethodInfo> methods, Action<MethodInfo> startCallSequence);

        /// <summary>
        /// Createas a selection of methods from the call site of a method
        /// </summary>
        /// <param name="callSites"></param>
        IList<ContextMenuItem> CreateAddMethodCallSelection(MethodInfo calledMethod, Action<ClassInfo, MethodInfo> addMethodCall);
    }
}
