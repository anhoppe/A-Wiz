using Awiz.Core.Storage;
using Awiz.Core.Contract;
using Awiz.Core.CSharpClassGenerator;
using Awiz.Core.Contract.Git;
using Awiz.Core.Git;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CodeTree;
using Awiz.Core.Contract.CodeTree;

namespace Awiz.Core
{
    public class ViewReader : IArchitectureWiz
    {
        private ClassGenerator _classGenerator = new();

        private ClassNodeGenerator _classNodeGenerator = new();

        private ClassParser _classParser = new();

        private string _pathToRepo = string.Empty;

        private string _pathToWiz = string.Empty;

        private RelationBuilder _relationBuilder = new();

        private Dictionary<string, string> _useCaseNameToViewPath = new Dictionary<string, string>();

        private Dictionary<string, string> _viewNameToViewPath = new Dictionary<string, string>();

        public List<string> ClassDiagrams { get; } = new();

        public List<ClassInfo> ClassInfos { get; private set; } = new();

        public List<ClassNamespaceNode> ClassNamespaceNodes { get; } = new();

        public IGitRepo GitAccess => LoadableGitAccess;

        public ArchitectureViewType LoadedView { get; private set; } = ArchitectureViewType.None;

        public List<string> UseCases { get; } = new List<string>();
        
        internal ILoadableGitRepo LoadableGitAccess { get; set; } = new GitRepo();

        internal INamespaceBuilder NamespaceBuilder { get; set; } = new NamespaceBuilder();

        internal IStorageAccess StorageAccess { get; set; } = new YamlStorageAccess();

        public ViewReader()
        {
            _relationBuilder.ClassNodeGenerator = _classNodeGenerator;
        }

        public ClassInfo? GetClassInfoById(string id)
        {
            return ClassInfos.FirstOrDefault(p => p.Id == id);
        }

        public IArchitectureView LoadClassDiagram(string viewName)
        {
            var graph = StorageAccess.LoadDiagramGraph(viewName, _viewNameToViewPath[viewName]);
            var architectureView = new ArchitectureClassView(_classParser.Classes)
            {
                ClassNodeGenerator = _classNodeGenerator,
                Graph = graph,
                Name = viewName,
                RelationBuilder = _relationBuilder,
                RepoPath = _pathToRepo,
                StorageAccess = StorageAccess,
            };

            return architectureView;
        }

        public IArchitectureView LoadUseCase(string useCaseName)
        {
            var graph = StorageAccess.LoadDiagramGraph(useCaseName, _useCaseNameToViewPath[useCaseName]);

            var useCase = new ArchitectureUseCaseView()
            {
                Graph = graph,
                Name = useCaseName,
                RepoPath = _pathToRepo,
                StorageAccess = StorageAccess,
                UseCasePath = _useCaseNameToViewPath[useCaseName],
            };

            useCase.Load();

            return useCase;
        }

        public IGitRepo ReadProject(string pathToRepo)
        {
            _pathToRepo = pathToRepo;
            LoadableGitAccess.LoadRepo(pathToRepo);
            _pathToWiz = Path.Combine(_pathToRepo, ".wiz");

            if (!Directory.Exists(_pathToWiz))
            {
                throw new DirectoryNotFoundException("The repo in not initialized for A-Wiz");
            }

            // Parse the source code information from the repo
            _classParser.ParseClasses(_pathToRepo);
            ClassInfos = _classParser.Classes;
            ClassNamespaceNodes.AddRange(NamespaceBuilder.Build(_classParser.Classes));
            // Parse the wiz information from the repo
            ReadUseCases();
            ReadClassDiagrams();

            return LoadableGitAccess;
        }

        private void ReadClassDiagrams()
        {
            var path = Path.Combine(_pathToWiz, "views");

            var files = Directory.GetFiles(path, "*.yaml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var viewName = Path.GetFileNameWithoutExtension(file);
                _viewNameToViewPath[viewName] = file;
                ClassDiagrams.Add(viewName);
            }
        }

        private void ReadUseCases()
        {
            var path = Path.Combine(_pathToWiz, "reqs");

            var files = Directory.GetFiles(path, "*.yaml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var useCaseName = Path.GetFileNameWithoutExtension(file);
                UseCases.Add(useCaseName);
                _useCaseNameToViewPath[useCaseName] = file;
            }
        }
    }
}
