using System.Collections.Generic;
using System.Linq;
using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Holds the state of the sequence diagram, such as the call stack and lifelines.
    /// Does not handle layout or node creation.
    /// </summary>
    internal class SequenceDiagramState : ISequenceDiagramState
    {
        private readonly List<SequenceLifelineInfo> _lifelines = new();

        public IReadOnlyList<SequenceLifelineInfo> Lifelines => _lifelines;

        public int LifelineCount => _lifelines.Count;

        /// <summary>
        /// Stack of call information (CallInfo).
        /// </summary>
        public Stack<CallInfo> CallStack { get; } = new();

        public bool IsMethodCallInProgress => CallStack.Count > 0;

        public void AddLifeline(ClassInfo classInfo, string headerNodeId, string lifelineNodeId)
        {
            if (_lifelines.Count > 0)
            {
                _lifelines[_lifelines.Count - 1].DistanceToNextLifeline = Design.SequenceClassesDistance;
            }
            _lifelines.Add(new SequenceLifelineInfo
            {
                ClassInfo = classInfo,
                HeaderNodeId = headerNodeId,
                LifelineNodeId = lifelineNodeId
            });
        }

        public int GetLifelineIndexByClass(ClassInfo classInfo)
        {
            return _lifelines.FindIndex(l => l.ClassInfo.Id == classInfo.Id);
        }

        public SequenceLifelineInfo? GetLifelineInfoByClass(ClassInfo classInfo)
        {
            return _lifelines.FirstOrDefault(l => l.ClassInfo.Id == classInfo.Id);
        }

        public void PushCall(CallInfo callInfo)
        {
            CallStack.Push(callInfo);
        }

        public CallInfo? PeekCall()
        {
            if (CallStack.Count == 0)
            {
                return null;
            }
            return CallStack.Peek();
        }

        public CallInfo? PopCall()
        {
            if (CallStack.Count == 0)
            {
                return null;
            }
            return CallStack.Pop();
        }

        public void Clear()
        {
            CallStack.Clear();
            _lifelines.Clear();
        }
    }
} 