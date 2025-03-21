using Gwiz.Core.Contract;

namespace Awiz.Core.Storage
{
    /// <summary>
    /// Node for serialization of persisted information
    /// </summary>
    internal class Node
    {
        public Node() { }

        public Node(INode node) 
        {
            X = node.X;
            Y = node.Y;
        }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
