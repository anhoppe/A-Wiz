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

        private IGraph? _graph;

        public IGraph? Graph 
        {
            get => _graph;
            set
            {
                _graph = value;

                if (_graph != null)
                {
                    _graph.NodeRemoved += (sender, node) => OnNodeRemoved(node);
                }
            }
        }

        public string Name { get; set; } = string.Empty;

        public string RepoPath { get; set; } = string.Empty;

        public abstract ArchitectureViewType Type { get; }

        internal IStorageAccess StorageAccess { get; set; } = new YamlStorageAccess();

        public event EventHandler<ClassInfo>? ClassSelected;

        public event EventHandler<INode>? NodeAdded;

        public abstract void AddBaseClassNode(ClassInfo derivedClassInfo);

        public abstract void AddClassNode(ClassInfo classInfo);

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

        protected abstract void OnNodeRemoved(INode node);

        protected void RaiseClassSelected(ClassInfo selectedClass)
        {
            ClassSelected?.Invoke(this, selectedClass);
        }

        protected void RaiseNodeAdded(INode node)
        {
            NodeAdded?.Invoke(this, node);
        }
    }
}
