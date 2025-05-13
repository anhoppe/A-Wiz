using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Dynamic;

namespace Awiz.Core
{
    internal class DynamicDeserializerYaml
    {
        public dynamic Deserialize(Stream stream)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using (var reader = new StreamReader(stream))
            {
                var yaml = reader.ReadToEnd();
                return deserializer.Deserialize<ExpandoObject>(yaml);
            }
        }
    }
}
