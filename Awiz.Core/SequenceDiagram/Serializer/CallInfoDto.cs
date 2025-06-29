
namespace Awiz.Core.SequenceDiagram.Serializer
{
    internal class CallInfoDto
    {
        public CallInfoDto() { }

        public CallInfoDto(CallInfo callStackInfo)
        {
            CalledMethodId = callStackInfo.CalledMethod.Id;
            SourceNodeId = callStackInfo.SourceNode?.Id ?? throw new InvalidOperationException("Source node is not set");
            TargetNodeId = callStackInfo.TargetNode?.Id ?? throw new InvalidOperationException("Target node is not set");
        }

        public string CalledMethodId { get; set; } = string.Empty;

        public string SourceNodeId { get; set; } = string.Empty;

        public string TargetNodeId { get; set; } = string.Empty;
    }
}
