using Awiz.Core.Contract.CodeInfo;
using System.Collections.Generic;

namespace Awiz.Core.SequenceDiagram
{
    /// <summary>
    /// Provides layout management for sequence diagrams, including position and distance calculations.
    /// </summary>
    public interface ISequenceDiagramLayoutManager
    {
        /// <summary>
        /// Calculates the X position for the lifeline at the given index.
        /// </summary>
        /// <param name="lifelineIndex">The index of the lifeline.</param>
        /// <returns>The X position in the diagram.</returns>
        int GetLifelineXPosition(int lifelineIndex);

        /// <summary>
        /// Calculates the available horizontal space between two lifelines.
        /// </summary>
        /// <param name="minIndex">The starting lifeline index (inclusive).</param>
        /// <param name="maxIndex">The ending lifeline index (exclusive).</param>
        /// <returns>The total available space in pixels.</returns>
        int CalculateAvailableSpace(int minIndex, int maxIndex);

        /// <summary>
        /// Ensures that the space between two lifelines is at least the required distance.
        /// </summary>
        /// <param name="minIndex">The starting lifeline index (inclusive).</param>
        /// <param name="maxIndex">The ending lifeline index (exclusive).</param>
        /// <param name="requiredDistance">The minimum required distance in pixels.</param>
        void EnsureDistance(int minIndex, int maxIndex, int requiredDistance);
    }
} 