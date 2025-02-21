using Gwiz.Core.Serializer;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Awiz.Core;
using System.Reflection;
using Gwiz.Core.Contract;
using System;
using System.Linq;

namespace Awiz
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public MainWindowViewModel(string path)
        {
            using (var stream = GetEmbeddedUmlYaml())
            {
                var serializer = new YamlSerializer((text) => new Size(80,30));

                var graph = serializer.Deserialize(stream);

                var classParser = new ClassParser();

                var classInfos = classParser.ParseClasses(path);

                foreach (var classInfo in classInfos)
                {
                    var node = graph.AddNode("Class");
                    node.Grid.FieldText[0][0] = classInfo.ClassName;
                    node.X = 10;
                    node.Y = 10;

                    node.Width = 180;
                    node.Height = 120;

                    node.Grid.TextSizeFactory = (text) => new Size(80, 30);

                    node.Grid.FieldText[0][1] = classInfo.Properties.Aggregate("", (current, prop) => current += prop.Name + "\n");
                    node.Grid.FieldText[0][2] = classInfo.Methods.Aggregate("", (current, method) => current += method.Name + "\n");
                }

                Nodes = graph.Nodes;
            }
        }

        public List<Node> Nodes { get; set; } = new();

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
