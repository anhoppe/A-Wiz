using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpClassGenerator;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    internal class ArchitectureClassView : ArchitectureView
    {
        private IList<ClassInfo> _classInfos;

        private Dictionary<INode, ClassInfo> _nodeToClassInfo = new();
        
        public ArchitectureClassView(IList<ClassInfo> classInfos)
        {
            _classInfos = classInfos;
        }

        public override ArchitectureViewType Type => ArchitectureViewType.Class;

        internal IClassNodeGenerator? ClassNodeGenerator { get; set; }

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        internal IRelationBuilder? RelationBuilder { private get; set; }
        
        internal ISerializer Serializer { get; set; } = new YamlSerializer();

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

            RelationBuilder.Build(Graph, classInfo, _nodeToClassInfo.Select(p => p.Value).ToList());
            RegisterNodeForSelectionEvent(classInfo, node);

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
        }
    }
}
