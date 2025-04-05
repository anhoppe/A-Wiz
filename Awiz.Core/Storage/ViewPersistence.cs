using Awiz.Core.CodeInfo;
using Gwiz.Core.Contract;
using Wiz.Infrastructure.IO;

namespace Awiz.Core.Storage
{
    internal class ViewPersistence : IViewPersistence
    {
        private Dictionary<INode, (string, View)> _nodeToPaths = new();

        internal IFileSystem FileSystem { private get; set; } = new FileSystem();

        internal string PathToRepo { private get; set; } = string.Empty;

        internal IStorageAccess StorageAccess { private get; set; } = new StorageAccess();

        internal string ViewName { private get; set; } = string.Empty;

        public void AddNode(INode node, ClassInfo classInfo)
        {
            var storagePath = ConstructStoragePath(classInfo.Directory);

            var view = new View();

            if (FileSystem.Exists(storagePath))
            {
                using (var stream = FileSystem.OpenRead(storagePath))
                {
                    view = StorageAccess.LoadNode(node, ViewName, stream);
                }
            }

            _nodeToPaths[node] = (storagePath, view);
        }

        /// <summary>
        /// Stores all nodes under perstistence
        /// </summary>
        public void Save()
        {
            foreach (var node in _nodeToPaths.Keys)
            {
                var stream = FileSystem.Create(_nodeToPaths[node].Item1);
                StorageAccess.SaveNode(node, _nodeToPaths[node].Item2, ViewName, stream);
            }
        }

        private string ConstructStoragePath(string pathToFile)
        {
            var directory = Path.GetDirectoryName(pathToFile) ?? string.Empty;
            var fileName = Path.GetFileNameWithoutExtension(pathToFile);

            var path = Path.Combine(PathToRepo, ".wiz\\storage");
            var relativePath = Path.GetRelativePath(PathToRepo, directory);

            path = Path.Combine(path, relativePath);
            return Path.Combine(path, fileName);
        }
    }
}
