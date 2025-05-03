using Awiz.Core.Storage;
using Awiz.Core.Contract;
using Awiz.Core.CSharpClassGenerator;
using Awiz.Core.Contract.Git;
using Awiz.Core.Git;

namespace Awiz.Core
{
    public class ViewReader : IArchitectureWiz
    {
        private ClassGenerator _classGenerator = new();

        private ClassNodeGenerator _classNodeGenerator = new();

        private ClassParser _classParser = new();

        private string _pathToRepo = string.Empty;

        private string _pathToWiz = string.Empty;

        private Dictionary<string, string> _useCaseNameToViewPath = new Dictionary<string, string>();

        private Dictionary<string, string> _viewNameToViewPath = new Dictionary<string, string>();

        public List<string> ClassDiagrams { get; } = new List<string>();

        public IGitRepo GitAccess => LoadableGitAccess;

        public ArchitectureViewType LoadedView { get; private set; } = ArchitectureViewType.None;

        public List<string> UseCases { get; } = new List<string>();
        
        internal ILoadableGitRepo LoadableGitAccess { get; set; } = new GitRepo();

        internal IStorageAccess StorageAccess { get; set; } = new YamlStorageAccess();

        public IArchitectureView LoadUseCase(string useCaseName)
        {
            var graph = StorageAccess.LoadUseCaseGraph(useCaseName, _useCaseNameToViewPath[useCaseName]);

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

        public IArchitectureView LoadClassDiagram(string viewName)
        {
            var graph = StorageAccess.LoadClassGraph();

            var annotationOptions = new AnnotationOptions();

            using (var fileStream = new FileStream(_viewNameToViewPath[viewName], FileMode.Open))
            {
                annotationOptions = AnnotationOptions.Deserialize(fileStream) ?? annotationOptions;
            }

            var architectureView = new ArchitectureClassView()
            {
                Graph = graph,
                Name = viewName,
                RepoPath = _pathToRepo,
                StorageAccess = StorageAccess,
            };

            _classNodeGenerator.ArchitectureView = architectureView;
                
            _classGenerator.AnnotationOptions = annotationOptions;
            _classGenerator.ClassFilter = new ClassFilter(_pathToRepo, viewName);
            _classGenerator.ClassNodeGenerator = _classNodeGenerator;

            _classGenerator.Generate(_classParser, graph);

            return architectureView;
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

            // Parse the wiz information from the repo
            ReadUseCases();
            ReadClassDiagrams();

            return LoadableGitAccess;
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
    }
}
