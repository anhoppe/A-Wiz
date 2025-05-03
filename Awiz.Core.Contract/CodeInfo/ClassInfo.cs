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
        /// Name of the base class, only set for Type == Class
        /// May be empty when not derived from any class
        /// </summary>
        public string BaseClass { get; set; } = string.Empty;

        /// <summary>
        /// The directory the class is extracted from
        /// </summary>
        public string Directory { get; set; } = string.Empty;

        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();

        public string Id => $"{Namespace}.{Name}";

        public List<string> ImplementedInterfaces { get; } = new List<string>();
        
        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();

        public string Name { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;


        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();

        public ClassType Type { get; set; } = ClassType.Class;


        public override string ToString()
        {
            return $"{Namespace}.{Name}\n" +
                   $"  Methods: {string.Join(", ", Methods.Select(m => m.ToString()))}\n" +
                   $"  Properties: {string.Join(", ", Properties.Select(p => p.ToString()))}\n" +
                   $"  Fields: {string.Join(", ", Fields.Select(f => f.ToString()))}";
        }

    }

}
