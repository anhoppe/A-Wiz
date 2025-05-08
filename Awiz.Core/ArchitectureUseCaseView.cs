using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Git;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    internal class ArchitectureUseCaseView : ArchitectureView
    {
        public override ArchitectureViewType Type => ArchitectureViewType.UseCase;

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        internal string UseCasePath { private get; set; } = string.Empty;

        internal ISerializer Serializer { get; set; } = new YamlSerializer();

        public override void AddBaseClassNode(ClassInfo derivedClassInfo)
        {
            throw new NotSupportedException("Use case diagram cannot add base class");
        }

        public override void AddClassNode(ClassInfo classInfo)
        {
            throw new NotSupportedException("Cannot add a class node to a Use Case diagram");
        }

        public override void AddUseCaseNode(INode node)
        {
            _gitNodeInfo.Add(node.Id, new GitNodeInfo());

            RaiseNodeAdded(node);
        }

        public override void Load()
        {
            var storagePath = Path.Combine(RepoPath, $".wiz\\req_storage\\{Name}");

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    _gitNodeInfo = StorageAccess.LoadGitInfo(stream);
                }
            }
        }

        public override void Save()
        {
            // Save the graph
            if (Graph == null)
            {
                throw new InvalidOperationException("No use case loaded");
            }

            var graphStoragePath = Path.Combine(RepoPath, $".wiz\\reqs\\{Name}.yaml");
            using (var fileStream = FileSystem.Create(graphStoragePath))
            {
                Serializer.Serialize(fileStream, Graph);
            }

            // Save meta-information
            var storagePath = Path.Combine(RepoPath, $".wiz\\req_storage\\{Name}");

            using (var stream = FileSystem.Create(storagePath))
            {
                StorageAccess.SaveGitInfo(_gitNodeInfo, stream);
            }
        }

        protected override void OnNodeRemoved(INode node)
        {
            
        }
    }
}
