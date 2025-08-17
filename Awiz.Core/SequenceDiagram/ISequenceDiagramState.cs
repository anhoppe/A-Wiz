using System.Collections.Generic;
using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Provides state management for a sequence diagram, such as the call stack and lifelines.
    /// </summary>
    internal interface ISequenceDiagramState
    {
        /// <summary>
        /// Represents the current call stack in the sequence diagram.
        /// </summary>
        Stack<CallInfo> CallStack { get; }

        /// <summary>
        /// Gets whether a method call is currently in progress (i.e., the call stack is not empty).
        /// </summary>
        bool IsMethodCallInProgress { get; }

        /// <summary>
        /// Gets the list of lifeline information objects in the diagram.
        /// </summary>
        IReadOnlyList<SequenceLifelineInfo> Lifelines { get; }

        /// <summary>
        /// Gets the number of lifelines currently in the diagram.
        /// </summary>
        int LifelineCount { get; }

        /// <summary>
        /// Adds a new lifeline for the specified class to the diagram.
        /// </summary>
        /// <param name="classInfo">The class to add as a lifeline.</param>
        /// <param name="headerNodeId">The node ID of the header node.</param>
        /// <param name="lifelineNodeId">The node ID of the lifeline node.</param>
        void AddLifeline(ClassInfo classInfo, string headerNodeId, string lifelineNodeId);

        /// <summary>
        /// Gets the index of the lifeline for the specified class.
        /// </summary>
        /// <param name="classInfo">The class to look up.</param>
        /// <returns>The index of the lifeline, or -1 if not found.</returns>
        int GetLifelineIndexByClass(ClassInfo classInfo);

        /// <summary>
        /// Gets the lifeline information object for the specified class.
        /// </summary>
        /// <param name="classInfo">The class to look up.</param>
        /// <returns>The lifeline info, or null if not found.</returns>
        SequenceLifelineInfo? GetLifelineInfoByClass(ClassInfo classInfo);

        /// <summary>
        /// Pushes a new call onto the call stack.
        /// </summary>
        /// <param name="callInfo">The call information to push.</param>
        void PushCall(CallInfo callInfo);

        /// <summary>
        /// Returns the most recent call without removing it from the call stack.
        /// </summary>
        /// <returns>Most recent call info from the call stack</returns>
        CallInfo? PeekCall();

        /// <summary>
        /// Pops the most recent call from the call stack.
        /// </summary>
        /// <returns>The most recent CallInfo, or null if the stack is empty.</returns>
        CallInfo? PopCall();

        /// <summary>
        /// Clears the call stack and resets the state.
        /// </summary>
        void Clear();
    }
} 