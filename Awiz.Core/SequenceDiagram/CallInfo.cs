using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    internal class CallInfo
    {
        public CallInfo() { }


        public CallInfo(INode sourceNode, INode targetNode, ClassInfo sourceClass, ClassInfo targetClass, MethodInfo calledMethod)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            SourceClass = sourceClass;
            TargetClass = targetClass;

            CalledMethod = calledMethod;
        }

        public MethodInfo CalledMethod { get; set; } = new MethodInfo();

        public ClassInfo SourceClass { get; }
        
        public INode? SourceNode { get; set; }

        public INode? TargetNode { get; set; }

        public ClassInfo TargetClass { get; }
    }
}
