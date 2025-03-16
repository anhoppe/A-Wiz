using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.Test
{
    [TestFixture]
    public class AnnotationsOptionsTest
    {
        [Test]
        public void Deserialize()
        {
            // Arrange
            var yaml = "EnableAssociations: true\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));

            // Act
            var config = AnnotationOptions.Deserialize(stream);

            // Assert
            Assert.That(config.EnableAssociations, Is.True);
        }

    }
}
