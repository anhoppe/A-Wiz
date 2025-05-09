using System.Xml.Linq;

namespace Awiz.Core.Contract.CodeInfo
{
    public enum ClassType
    {
        Class,
        Interface
    }

    public class ClassInfo
    {
        /// <summary>
        /// Name of the assembly the class belongs to
        /// </summary>
        public string Assembly { get; set; } = string.Empty;

        /// <summary>
        /// Name of the base class, only set for Type == Class
        /// May be empty when not derived from any class
        /// </summary>
        public string BaseClass { get; set; } = string.Empty;

        /// <summary>
        /// The directory the class is extracted from
        /// </summary>
        public string Directory { get; set; } = string.Empty;

        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();

        public string Id
        {
            get
            {
                var id = $"{Namespace}.{Name}";
                if (!string.IsNullOrEmpty(id) && id[^1] == '?')
                {
                    id = id[..^1];
                }

                return id;
            }
        }

        public List<string> ImplementedInterfaces { get; } = new List<string>();
        
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();

        public string Name { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;

        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();

        public ClassType Type { get; set; } = ClassType.Class;

        public override string ToString() => Name;      
    }
}
