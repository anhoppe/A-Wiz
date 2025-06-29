using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class CallInfo
    {
        public CallInfo() { }

        public CallInfo(INode sourceClass, INode targetClass)
        {
            SourceNode = sourceClass;
            TargetNode = targetClass;
        }

        public CallInfo(INode sourceClass, INode targetClass, MethodInfo calledMethod)
        {
            SourceNode = sourceClass;
            TargetNode = targetClass;
            CalledMethod = calledMethod;
        }

        public MethodInfo CalledMethod { get; set; } = new MethodInfo();

        public INode? SourceNode { get; set; }

        public INode? TargetNode { get; set; }
    }
}
