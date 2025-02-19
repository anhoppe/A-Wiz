using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz.Core
{
    public class ClassInfo
    {
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();
        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();


        public override string ToString()
        {
            return $"{Namespace}.{ClassName}\n" +
                   $"  Methods: {string.Join(", ", Methods.Select(m => m.ToString()))}\n" +
                   $"  Properties: {string.Join(", ", Properties.Select(p => p.ToString()))}\n" +
                   $"  Fields: {string.Join(", ", Fields.Select(f => f.ToString()))}";
        }
    }

}
