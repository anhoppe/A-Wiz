using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    internal class ArchitectureClassView : ArchitectureView
    {
        private IList<ClassInfo> _classInfos;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();
        
        private Dictionary<INode, (string, View)> _nodeToPaths = new();

        public ArchitectureClassView(IList<ClassInfo> classInfos)
        {
            _classInfos = classInfos;
        }

        public override ArchitectureViewType Type => ArchitectureViewType.Class;

        internal IClassNodeGenerator? ClassNodeGenerator { get; set; }

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        internal IRelationBuilder? RelationBuilder { private get; set; }

        public override void AddBaseClassNode(ClassInfo derivedClassInfo)
        {
            var baseClassInfo = _classInfos.First(p => p.Id == derivedClassInfo.BaseClass);

            AddClassNode(baseClassInfo);
        }

        public override void AddClassNode(ClassInfo classInfo)
        {
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

            var node = ClassNodeGenerator.CreateClassNode(Graph, classInfo);

            var storagePath = ConstructClassStoragePath(classInfo.Directory);

            var view = new View();

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    view = StorageAccess.LoadNode(node, Name, stream);
                }
            }

            RelationBuilder.Build(Graph, classInfo, _nodeToClassInfo.Select(p => p.Value).ToList());
            RegisterNodeForSelectionEvent(classInfo, node);

            _nodeToPaths[node] = (storagePath, view);
            _nodeToClassInfo[node] = classInfo;
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

        public override void AddUseCaseNode(INode node)
        {
            throw new NotSupportedException("Can't add a use case node to a class diagram");
        }

        public override void Load()
        {
        }

        public override void Save()
        {
            foreach (var node in _nodeToPaths.Keys)
            {
                var stream = FileSystem.Create(_nodeToPaths[node].Item1);
                StorageAccess.SaveNode(node, _nodeToPaths[node].Item2, Name, stream);
            }
        }

        private string ConstructClassStoragePath(string pathToFile)
        {
            var directory = Path.GetDirectoryName(pathToFile) ?? string.Empty;
            var fileName = Path.GetFileNameWithoutExtension(pathToFile);

            var path = Path.Combine(RepoPath, ".wiz\\storage");
            var relativePath = Path.GetRelativePath(RepoPath, directory);

            path = Path.Combine(path, relativePath);
            return Path.Combine(path, fileName);
        }
    }
}
