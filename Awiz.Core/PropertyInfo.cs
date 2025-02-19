using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public class PropertyInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string AccessModifier { get; set; }
        public override string ToString() => $"{AccessModifier} {Type} {Name}";
    }
}
