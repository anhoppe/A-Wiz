using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.Git;
using Awiz.Core.Git;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;

namespace Awiz.Core
{
    internal abstract class ArchitectureView : IArchitectureView
    {
        protected Dictionary<string, IGitNodeInfo> _gitNodeInfo = new Dictionary<string, IGitNodeInfo>();

        public IGraph? Graph { get; set; }

        public string Name { get; set; } = string.Empty;

        public string RepoPath { get; set; } = string.Empty;

        public abstract ArchitectureViewType Type { get; }

        internal IStorageAccess StorageAccess { get; set; } = new YamlStorageAccess();

        public event EventHandler<INode>? NodeAdded;

        public abstract void AddClassNode(INode node, ClassInfo classInfo);

        public abstract void AddUseCaseNode(INode node);

        public IGitNodeInfo GetAssociatedCommits(INode node)
        {
            if (!_gitNodeInfo.ContainsKey(node.Id))
            {
                _gitNodeInfo[node.Id] = new GitNodeInfo();
            }

            return _gitNodeInfo[node.Id];
        }

        public abstract void Load();
        
        public abstract void Save();

        internal void RaiseNodeAdded(INode node)
        {
            NodeAdded?.Invoke(this, node);
        }
    }
}
