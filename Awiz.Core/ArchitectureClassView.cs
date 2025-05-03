using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Wiz.Infrastructure.IO;

namespace Awiz.Core
{
    internal class ArchitectureClassView : ArchitectureView
    {
        private Dictionary<INode, (string, View)> _nodeToPaths = new();

        public override ArchitectureViewType Type => ArchitectureViewType.Class;

        internal IFileSystem FileSystem { get; set; } = new FileSystem();

        public override void AddClassNode(INode node, ClassInfo classInfo)
        {
            var storagePath = ConstructClassStoragePath(classInfo.Directory);

            var view = new View();

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    view = StorageAccess.LoadNode(node, Name, stream);
                }
            }

            _nodeToPaths[node] = (storagePath, view);
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
