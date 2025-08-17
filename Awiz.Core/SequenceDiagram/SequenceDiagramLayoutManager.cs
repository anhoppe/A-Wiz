
namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Layout manager for sequence diagrams. Operates on a referenced lifeline list.
    /// </summary>
    public class SequenceDiagramLayoutManager : ISequenceDiagramLayoutManager
    {
        private IReadOnlyList<SequenceLifelineInfo>? _lifelines;

        public SequenceDiagramLayoutManager(IReadOnlyList<SequenceLifelineInfo> lifelines)
        {
            _lifelines = lifelines;
        }

        public int GetLifelineXPosition(int lifelineIndex)
        {
            if (_lifelines == null) throw new System.InvalidOperationException("Layout manager not initialized.");
            int x = 0;
            for (int i = 0; i < lifelineIndex; i++)
            {
                x += _lifelines[i].DistanceToNextLifeline;
            }
            return x;
        }

        public int CalculateAvailableSpace(int minIndex, int maxIndex)
        {
            if (_lifelines == null) throw new System.InvalidOperationException("Layout manager not initialized.");
            int availableSpace = 0;
            for (int i = minIndex; i < maxIndex; i++)
            {
                availableSpace += _lifelines[i].DistanceToNextLifeline;
            }
            return availableSpace;
        }

        public void EnsureDistance(int minIndex, int maxIndex, int requiredDistance)
        {
            if (_lifelines == null) throw new System.InvalidOperationException("Layout manager not initialized.");
            int availableSpace = CalculateAvailableSpace(minIndex, maxIndex);
            if (requiredDistance > availableSpace)
            {
                int extra = requiredDistance - availableSpace;
                _lifelines[minIndex].DistanceToNextLifeline += extra;
            }
        }
    }
} 