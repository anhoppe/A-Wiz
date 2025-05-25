using Awiz.Core.Contract;
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
        /// Attaches the button behavior to the start call sequence button
        /// </summary>
        /// <param name="architectureView"></param>
        /// <param name="classInfo"></param>
        /// <param name="lifelineNode"></param>
        void AttachButtonBehavior(IArchitectureView architectureView, ClassInfo classInfo, INode lifelineNode);
        
        void UpdateAddMethodButtonBehavior(IArchitectureView architectureView, ClassInfo classInfo, MethodInfo methodInfo, INode lifelineNode);
    }
}
