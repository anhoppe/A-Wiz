using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Awiz.Core
{
    public class AnnotationOptions
    {
        public bool EnableAssociations { get; set; } = true;

        public static AnnotationOptions Deserialize(Stream stream)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using (var reader = new StreamReader(stream))
            {
                var yaml = reader.ReadToEnd();
                return deserializer.Deserialize<AnnotationOptions>(yaml);
            }
        }
    }
}
