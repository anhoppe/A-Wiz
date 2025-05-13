using Awiz.Core.ClassDiagram;
using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    internal class ArchitectureClassView : ArchitectureView
    {
        private IClassNodeGenerator? _classNodeGenerator;
        
        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();
        
        private IList<ClassInfo> _repoClassInfos;

        public ArchitectureClassView(IList<ClassInfo> classInfos)
        {
            _repoClassInfos = classInfos;
        }
        
        public override ArchitectureViewType Type => ArchitectureViewType.Class;

        internal IClassNodeGenerator? ClassNodeGenerator 
        {
            get => _classNodeGenerator;
            set
            {
                _classNodeGenerator = value;
                if (_classNodeGenerator != null)
                {
                    _classNodeGenerator.NodeToClassInfoMapping = _nodeToClassInfo;
                }
            }
        }

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        internal IRelationBuilder? RelationBuilder { private get; set; }
        
        internal ISerializer Serializer { get; set; } = new YamlSerializer();

        public override void AddBaseClassNode(ClassInfo derivedClassInfo)
        {
            var baseClassInfo = _repoClassInfos.First(p => p.Id() == derivedClassInfo.BaseClass);

            AddClassNode(baseClassInfo);
        }

        public override void AddClassNode(ClassInfo classInfo)
        {
            if (_nodeToClassInfo.ContainsValue(classInfo))
            {
                return;
            }

            if (ClassNodeGenerator == null)
            {
                throw new NullReferenceException("ClassNodeGenerator is not set");
            }

            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }

            if (RelationBuilder == null)
            {
                throw new NullReferenceException("RelationuBuilder is not set");
            }

            var node = ClassNodeGenerator.CreateClassNode(Graph, classInfo, RaiseShowVersionDiff);
            _nodeToClassInfo[node] = classInfo;

            RelationBuilder.Build(Graph, classInfo, _nodeToClassInfo.Select(p => p.Value).ToList());
            RegisterNodeForSelectionEvent(classInfo, node);
        }

        public override void AddUseCaseNode(INode node)
        {
            throw new NotSupportedException("Can't add a use case node to a class diagram");
        }

        public override void Load(IVersionUpdater versionUpdater)
        {
            if (Graph == null)
            {
                throw new NullReferenceException("Graph is not set");
            }
            if (ClassNodeGenerator == null)
            {
                throw new NullReferenceException("ClassNodeGenerator is not set");
            }

            var storagePath = Path.Combine(RepoPath, $".wiz\\storage\\{Name}.yaml");

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    var mapping = StorageAccess.LoadNodeIdToClassInfoMapping(stream);

                    foreach (var (nodeId, classInfo) in mapping)
                    {
                        var node = Graph.Nodes.FirstOrDefault(p => p.Id == nodeId);
                        if (classInfo != null && node != null)
                        {
                            _nodeToClassInfo[node] = classInfo;
                            RegisterNodeForSelectionEvent(classInfo, node);

                            versionUpdater.CheckVersionUpdates(classInfo, _repoClassInfos);
                            ClassNodeGenerator.UpdateClassNode(node, classInfo, RaiseShowVersionDiff);
                        }
                    }
                }
            }

            versionUpdater.ClassUpdated += (sender, classInfo) =>
            {
                if (ClassNodeGenerator == null)
                {
                    throw new NullReferenceException("ClassNodeGenerator is not set");
                }

                if (_nodeToClassInfo.ContainsValue(classInfo))
                {
                    var node = _nodeToClassInfo.First(p => p.Value == classInfo).Key;
                    ClassNodeGenerator.UpdateClassNode(node, classInfo, RaiseShowVersionDiff);
                    Graph.Update();
                }
            };
        }

        public override void Save()
        {
            // Save the graph
            if (Graph == null)
            {
                throw new InvalidOperationException("No class diagram loaded");
            }

            var graphStoragePath = Path.Combine(RepoPath, $".wiz\\views\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(graphStoragePath))
            {
                Serializer.Serialize(fileStream, Graph);
            }

            // Save the mapping to the classes
            var storagePath = Path.Combine(RepoPath, $".wiz\\storage\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(storagePath))
            {
                StorageAccess.SaveNodeIdToClassInfoMapping(_nodeToClassInfo.Select(p => (p.Key.Id, p.Value)).ToDictionary(), fileStream);
            }
        }

        protected override void OnNodeRemoved(INode node)
        {
            if (_nodeToClassInfo.ContainsKey(node))
            {
                _nodeToClassInfo.Remove(node);
            }
        }

        private void RegisterNodeForSelectionEvent(ClassInfo classInfo, INode node)
        {
            node.SelectedChanged += (sender, isSelected) =>
            {
                if (isSelected)
                {
                    RaiseClassSelected(classInfo);
                }
            };
        }
    }
}
