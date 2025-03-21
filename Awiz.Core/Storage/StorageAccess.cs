using Gwiz.Core;
using Gwiz.Core.Contract;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Awiz.Core.Storage
{
    internal class StorageAccess : IStorageAccess
    {
        public View LoadNode(INode targetNode, string viewName, Stream stream)
        {
            var view = new View();
            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                view = deserializer.Deserialize<View>(yaml) ?? view;

                if (view.Views.ContainsKey(viewName))
                {
                    var nodeInfo = view.Views[viewName];

                    targetNode.X = nodeInfo.X;
                    targetNode.Y = nodeInfo.Y;
                }
            }

            return view;
        }

        public void SaveNode(INode targetNode, View targetView, string viewName, Stream stream)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            var nodeInfo = new Node(targetNode);
            targetView.Views[viewName] = nodeInfo;

            var yaml = serializer.Serialize(targetView);
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(yaml);
            }
        }
    }
}
