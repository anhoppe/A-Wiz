using Gwiz.Core.Serializer;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Awiz.Core;
using System.Reflection;
using Gwiz.Core.Contract;

namespace Awiz
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public MainWindowViewModel(string path)
        {
            using (var stream = GetEmbeddedUmlYaml())
            {
                var gwizDeserializer = new YamlSerializer();

                var graph = gwizDeserializer.Deserialize(stream);

                // Load config
                using (var configStream = File.OpenRead("C:\\repo\\A-Wiz\\Awiz.App\\Assets\\ClassConfig.yaml"))
                {
                    var awizDeserializer = new YamlConfigSerializer();
                    var config = awizDeserializer.Deserialize(configStream);

                    var classGenerator = new ClassGenerator()
                    {
                        ClassFilter = config,
                    };

                    var classParser = new ClassParser("C:\\repo\\G-Wiz\\");
                    //var classParser = new ClassParser("C:\\repo\\A-Wiz\\Awiz.Core.Test\\Assets\\ExtendsImplements\\");

                    classGenerator.Generate(classParser, graph);
                }

                Nodes = graph.Nodes;
                Edges = graph.Edges;

            }
        }

        public List<IEdge> Edges { get; set; } = new();

        public List<INode> Nodes { get; set; } = new();

        private static Stream GetEmbeddedUmlYaml()
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = "Awiz.Assets.Uml.yaml";

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }

            return stream;
        }
    }
}
