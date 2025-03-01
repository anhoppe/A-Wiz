using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core.CodeInfo
{
    public class MethodInfo
    {
        public string AccessModifier { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        
        public string ReturnType { get; set; } = string.Empty;
                
        public override string ToString() => $"{AccessModifier} {ReturnType} {Name}({string.Join(", ", Parameters.Select(p => p.ToString()))})";
    }
}
