using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public class MethodInfo
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public string AccessModifier { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        public override string ToString() => $"{AccessModifier} {ReturnType} {Name}({string.Join(", ", Parameters.Select(p => p.ToString()))})";
    }
}
