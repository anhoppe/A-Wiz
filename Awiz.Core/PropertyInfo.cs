using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public class PropertyInfo
    {
        public string AccessModifier { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        
        public override string ToString() => $"{AccessModifier} {Type} {Name}";
    }
}
