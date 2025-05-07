using Gwiz.Core.Contract;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Gwiz.Core.Serializer;
using System.Text;
using System.Reflection;
using Awiz.Core.Git;
using Awiz.Core.Contract.Git;

namespace Awiz.Core.Storage
{
    internal class YamlStorageAccess : IStorageAccess
    {
        public IGraph LoadDiagramGraph(string name, string path)
        {
            using (var templateDefinitions = GetEmbeddedUmlYaml())
            {
                using (var nodeDefinitinos = File.Open(path, FileMode.Open))
                {
                    var templatesAsText = new StreamReader(templateDefinitions, Encoding.UTF8).ReadToEnd();
                    var graphAsText = new StreamReader(nodeDefinitinos, Encoding.UTF8).ReadToEnd();

                    var combined = templatesAsText.TrimEnd() + "\n" + graphAsText.TrimStart();

                    using (MemoryStream combinedStream = new MemoryStream(Encoding.UTF8.GetBytes(combined)))
                    {
                        using (var reader = new StreamReader(combinedStream))
                        {
                            var gwizDeserializer = new YamlSerializer();
                            return gwizDeserializer.Deserialize(combinedStream);
                        }
                    }
                }
            }
        }

        public Dictionary<string, IGitNodeInfo> LoadGitInfo(Stream stream)
        {
            Dictionary<string, IGitNodeInfo> gitInfo = new();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();
                var serializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

                var gitInfoInternal = serializer.Deserialize<Dictionary<string, GitNodeInfo>>(yaml) ?? new ();

                gitInfo = gitInfoInternal.ToDictionary(kvp => kvp.Key, kvp => (IGitNodeInfo)kvp.Value);
            }

            return gitInfo;
        }

        public void SaveGitInfo(Dictionary<string, IGitNodeInfo> gitInfo, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(gitInfo);

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
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
    }
}
