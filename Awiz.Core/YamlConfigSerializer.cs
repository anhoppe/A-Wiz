using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Awiz.Core
{
    public class YamlConfigSerializer
    {
        public ClassFilter Deserialize(Stream stream)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using (var reader = new StreamReader(stream))
            {
                var yaml = reader.ReadToEnd();
                return deserializer.Deserialize<ClassFilter>(yaml);
            }
        }
    }
}
