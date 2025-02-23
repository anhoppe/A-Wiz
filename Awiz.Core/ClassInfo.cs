using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public class ClassInfo
    {
        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();
        
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();

        public string Name { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;


        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();

        public override string ToString()
        {
            return $"{Namespace}.{Name}\n" +
                   $"  Methods: {string.Join(", ", Methods.Select(m => m.ToString()))}\n" +
                   $"  Properties: {string.Join(", ", Properties.Select(p => p.ToString()))}\n" +
                   $"  Fields: {string.Join(", ", Fields.Select(f => f.ToString()))}";
        }
    }

}
