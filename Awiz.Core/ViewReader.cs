using Gwiz.Core;
using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using System.Reflection;

namespace Awiz.Core
{
    public class ViewReader : IViewProvider
    {
        private ClassGenerator _classGenerator = new ClassGenerator();
        
        private string _pathToRepo = string.Empty;

        private ClassParser _classParser = new();

        private Dictionary<string, string> _viewNameToViewPath = new Dictionary<string, string>();

        public List<string> Views { get; } = new List<string>();

        public IGraph GetViewByName(string viewName)
        {
            using (var stream = GetEmbeddedUmlYaml())
            {
                var gwizDeserializer = new YamlSerializer();

                var graph = gwizDeserializer.Deserialize(stream);

                var annotationOptions = new AnnotationOptions();

                using (var fileStream = new FileStream(_viewNameToViewPath[viewName], FileMode.Open))
                {
                    annotationOptions ??= AnnotationOptions.Deserialize(fileStream);
                }                

                _classGenerator.AnnotationOptions = annotationOptions;
                _classGenerator.ClassFilter = new ClassFilter(_pathToRepo, viewName);
                _classGenerator.Generate(_classParser, graph);

                return graph;
            }
        }

        public void Read(string pathToRepo)
        {
            if (Directory.Exists(pathToRepo + ".wiz"))
            {
                _pathToRepo = pathToRepo;
        
                _classParser.ParseClasses(pathToRepo);

                ReadViews(pathToRepo);
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

        private void ReadViews(string pathToRepo)
        {
            var path = Path.Combine(pathToRepo, ".wiz");

            path = Path.Combine(path, "views");

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
