using Awiz.Core.Storage;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System.Reflection;
using Wiz.Infrastructure.IO;
using System.Text;

namespace Awiz.Core
{
    public class ViewReader : IViewProvider
    {
        private enum LoadedView
        {
            None,
            UseCase,
            ClassDiagram
        }

        private ClassGenerator _classGenerator = new();

        private ClassNodeGenerator _classNodeGenerator = new();

        private LoadedView _loadedView = LoadedView.None;

        private ViewPersistence _nodePersistence = new ViewPersistence();
        
        private string _pathToRepo = string.Empty;

        private string _pathToWiz = string.Empty;

        private ClassParser _classParser = new();

        private IGraph? _useCaseDiagram = null;

        private string _useCaseName = string.Empty;

        private Dictionary<string, string> _useCaseNameToViewPath = new Dictionary<string, string>();
        
        private Dictionary<string, string> _viewNameToViewPath = new Dictionary<string, string>();

        public List<string> UseCases { get; } = new List<string>();
        
        public List<string> Views { get; } = new List<string>();

        public IGraph GetUseCaseByName(string useCaseName)
        {
            using (var templateDefinitions = GetEmbeddedUmlYaml())
            {
                using (var nodeDefinitinos = File.Open(_useCaseNameToViewPath[useCaseName], FileMode.Open))
                {
                    var templatesAsText = new StreamReader(templateDefinitions, Encoding.UTF8).ReadToEnd();
                    var graphAsText = new StreamReader(nodeDefinitinos, Encoding.UTF8).ReadToEnd();

                    var combined = templatesAsText.TrimEnd() + "\n" + graphAsText.TrimStart();

                    using (MemoryStream combinedStream = new MemoryStream(Encoding.UTF8.GetBytes(combined)))
                    {
                        using (var reader = new StreamReader(combinedStream))
                        {
                            var gwizDeserializer = new YamlSerializer();
                            var graph = gwizDeserializer.Deserialize(combinedStream);

                            _loadedView = LoadedView.UseCase;
                            _useCaseName = useCaseName;
                            _useCaseDiagram = graph;
                            return graph;
                        }
                    }
                }
            }
        }

        public IGraph GetViewByName(string viewName)
        {
            using (var stream = GetEmbeddedUmlYaml())
            {
                var gwizDeserializer = new YamlSerializer();

                var graph = gwizDeserializer.Deserialize(stream);

                var annotationOptions = new AnnotationOptions();

                using (var fileStream = new FileStream(_viewNameToViewPath[viewName], FileMode.Open))
                {
                    annotationOptions = AnnotationOptions.Deserialize(fileStream) ?? annotationOptions;
                }

                _nodePersistence = new ViewPersistence()
                {
                    FileSystem = new FileSystem(),
                    PathToRepo = _pathToRepo,
                    StorageAccess = new StorageAccess(),
                    ViewName = viewName,
                };

                _classNodeGenerator.NodePersistence = _nodePersistence;
                
                _classGenerator.AnnotationOptions = annotationOptions;
                _classGenerator.ClassFilter = new ClassFilter(_pathToRepo, viewName);
                _classGenerator.ClassNodeGenerator = _classNodeGenerator;

                _classGenerator.Generate(_classParser, graph);

                _loadedView = LoadedView.ClassDiagram;

                return graph;
            }
        }

        public void ReadProject(string pathToRepo)
        {
            _pathToRepo = pathToRepo;
            _pathToWiz = Path.Combine(_pathToRepo, ".wiz");

            if (!Directory.Exists(_pathToWiz))
            {
                throw new DirectoryNotFoundException("The repo in not initialized for A-Wiz");
            }

            // Parse the source code information from the repo
            _classParser.ParseClasses(_pathToRepo);

            // Parse the wiz information from the repo
            ReadUseCases();
            ReadViews();
        }

        public void SaveView()
        {
            switch (_loadedView)
            {
                case LoadedView.UseCase:
                    YamlSerializer serializer = new YamlSerializer();

                    if (_useCaseDiagram == null)
                    {
                        throw new InvalidOperationException("No use case loaded");
                    }

                    using (var fileStream = new FileStream(_useCaseNameToViewPath[_useCaseName], FileMode.Create))
                    {
                        serializer.Serialize(fileStream, _useCaseDiagram);
                    }
                    break;
                case LoadedView.ClassDiagram:
                    _nodePersistence.Save();
                    break;
                default:
                    throw new InvalidOperationException("No view loaded");
            }
        }

        private static Stream GetEmbeddedUmlYaml()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = "Awiz.Core.Assets.Uml.yaml";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }

            return stream;
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

        private void ReadViews()
        {
            var path = Path.Combine(_pathToWiz, "views");

            var files = Directory.GetFiles(path, "*.yaml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var viewName = Path.GetFileNameWithoutExtension(file);
                Views.Add(viewName);
                _viewNameToViewPath[viewName] = file;
            }
        }
    }
}
