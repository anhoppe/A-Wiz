using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Holds information about a sequence diagram lifeline and its header
    /// </summary>
    public class SequenceLifelineInfo
    {
        public string HeaderNodeId { get; set; } = string.Empty;
        public string LifelineNodeId { get; set; } = string.Empty;
        public ClassInfo ClassInfo { get; set; } = null!;
        public int DistanceToNextLifeline { get; set; } = Design.SequenceClassesDistance;
    }
} 