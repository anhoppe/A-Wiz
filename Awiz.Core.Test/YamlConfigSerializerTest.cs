using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class YamlConfigSerializerTest
    {
        [Test]
        public void Deserialize()
        {
            // Arrange
            var yaml = "EnableAssociations: true\n" +
                       "Namespaces:\n" +
                       "  Blacklist:\n" +
                       "    - System\n" +
                       "    - System.Collections\n" +
                       "    - System.Collections.Generic\n" +
                       "  Whitelist:\n" +
                       "    - System.Text\n" +
                       "    - System.Text.RegularExpressions\n" +
                       "    - System.Text.RegularExpressions.Regex\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            var sut = new YamlConfigSerializer();

            // Act
            var config = sut.Deserialize(stream);

            // Assert
            Assert.That(config.EnableAssociations, Is.True);
            Assert.That(config.Namespaces.Blacklist, Is.EquivalentTo(new[] { "System", "System.Collections", "System.Collections.Generic" }));
            Assert.That(config.Namespaces.Whitelist, Is.EquivalentTo(new[] { "System.Text", "System.Text.RegularExpressions", "System.Text.RegularExpressions.Regex" }));
        }
    }
}
