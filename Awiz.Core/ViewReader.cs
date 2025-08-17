using Awiz.Core.Storage;
using Awiz.Core.Contract;
using Awiz.Core.CSharpParsing;
using Awiz.Core.Contract.Git;
using Awiz.Core.Git;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.ClassDiagram;
using Awiz.Core.Contract.CSharpParsing;
using Awiz.Core.SequenceDiagram;

namespace Awiz.Core
{
    public class ViewReader : IArchitectureWiz
    {
        private ClassNodeGenerator _classNodeGenerator = new();

        private ClassParser _classParser = new()
        {
            ProjectParser = new ProjectParser(),
        };

        private InteractionBehavior _interactionBehavior;

        private IMethodSelector _methodSelector;

        private string _pathToRepo = string.Empty;

        private string _pathToWiz = string.Empty;

        private RelationBuilder _relationBuilder = new();

        private IStorageAccess _storageAccess;

        private Dictionary<string, string> _sequenceDiagramNameToViewPath = new Dictionary<string, string>();

        private SequenceNodeGenerator _sequenceNodeGenerator;

        private Func<string, (int, int)> _textSizeCalculator = (text) => throw new NotImplementedException("Text size calculator is not set in ViewReader.");

        private Dictionary<string, string> _useCaseNameToViewPath = new Dictionary<string, string>();

        private Dictionary<string, string> _viewNameToViewPath = new Dictionary<string, string>();

        public ViewReader()
        {
            _methodSelector = new MethodSelector()
            {
                SourceCode = _classParser,
            };

            _interactionBehavior = new InteractionBehavior()
            {
                MethodSelector = _methodSelector,
            };

            _relationBuilder.ClassNodeGenerator = _classNodeGenerator;

            var state = new SequenceDiagramState();
            var layoutManager = new SequenceDiagramLayoutManager(state.Lifelines);

            _sequenceNodeGenerator = new SequenceNodeGenerator(layoutManager, state);

            _storageAccess = new YamlStorageAccess()
            {
                SourceCode = _classParser,
            };
        }

        public List<string> ClassDiagrams { get; } = new();

        public List<ClassInfo> ClassInfos { get; private set; } = new();

        public IGitRepo GitAccess => LoadableGitAccess;

        public ArchitectureViewType LoadedView { get; private set; } = ArchitectureViewType.None;

        public List<string> SequenceDiagrams { get; } = new();

        public List<string> UseCases { get; } = new List<string>();

        public IVersionUpdater VersionUpdater { get; } = new VersionUpdater();

        internal ILoadableGitRepo LoadableGitAccess { get; set; } = new GitRepo();

        internal INamespaceBuilder NamespaceBuilder { get; set; } = new NamespaceBuilder();

        public ClassInfo? GetClassInfoById(string id)
        {
            return ClassInfos.FirstOrDefault(p => p.Id() == id);
        }
        
        public IDictionary<string, ClassNamespaceNode> GetClassNamespaceNodes(bool includeInterfaces) => NamespaceBuilder.GetClassTree(includeInterfaces);

        public IArchitectureView LoadClassDiagram(string viewName)
        {
            var graph = _storageAccess.LoadDiagramGraph(viewName, _viewNameToViewPath[viewName]);
            var architectureView = new ArchitectureClassView(_classParser.Classes)
            {
                ClassNodeGenerator = _classNodeGenerator,
                Graph = graph,
                Name = viewName,
                RelationBuilder = _relationBuilder,
                RepoPath = _pathToRepo,
                StorageAccess = _storageAccess,
            };

            architectureView.Load(VersionUpdater);

            return architectureView;
        }

        public IArchitectureView LoadSequenceDiagram(string sequenceDiagramName)
        {
            var graph = _storageAccess.LoadDiagramGraph(sequenceDiagramName, _sequenceDiagramNameToViewPath[sequenceDiagramName]);
            graph.SetTextSizeCalculator(_textSizeCalculator);

            var architectureView = new ArchitectureSequenceView()
            {
                Graph = graph,
                InteractionBehavior = _interactionBehavior,
                Name = sequenceDiagramName,
                RepoPath = _pathToRepo,
                SequenceNodeGenerator = _sequenceNodeGenerator,
                StorageAccess = _storageAccess,
            };

            _interactionBehavior.Graph = graph;

            architectureView.Load(VersionUpdater);
            architectureView.Initialize();

            return architectureView;
        }

        public IArchitectureView LoadUseCase(string useCaseName)
        {
            var graph = _storageAccess.LoadDiagramGraph(useCaseName, _useCaseNameToViewPath[useCaseName]);
            var useCase = new ArchitectureUseCaseView()
            {
                Graph = graph,
                Name = useCaseName,
                RepoPath = _pathToRepo,
                StorageAccess = _storageAccess,
                UseCasePath = _useCaseNameToViewPath[useCaseName],
            };

            useCase.Load(VersionUpdater);

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
            NamespaceBuilder.Build(_classParser.Classes);
            
            // Parse the wiz information from the repo
            ReadUseCases();
            ReadClassDiagrams();
            ReadSequenceDiagrams();

            return LoadableGitAccess;
        }

        public void SetTextSizeCalculator(Func<string, (int, int)> textSizeCalculator)
        {
            _textSizeCalculator = textSizeCalculator;
            _sequenceNodeGenerator.SetTextSizeCalculator(textSizeCalculator);
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

        private void ReadSequenceDiagrams()
        {
            var path = Path.Combine(_pathToWiz, "sequence");

            var files = Directory.GetFiles(path, "*.yaml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var sequenceDiagramName = Path.GetFileNameWithoutExtension(file);
                SequenceDiagrams.Add(sequenceDiagramName);
                _sequenceDiagramNameToViewPath[sequenceDiagramName] = file;
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
