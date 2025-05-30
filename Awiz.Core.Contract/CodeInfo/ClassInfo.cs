﻿namespace Awiz.Core.Contract.CodeInfo
{
    public enum ClassType
    {
        Class,
        Interface
    }

    public class ClassInfo
    {
        /// <summary>
        /// Version diff information
        /// List of methods that were added to the class in newer versions
        /// </summary>
        public IList<MethodInfo> AddedMethods { get; set; } = new List<MethodInfo>();

        /// <summary>
        /// Version diff information
        /// List of properties that were added to the class in newer versions
        /// </summary>
        public IList<PropertyInfo> AddedProperties { get; set; } = new List<PropertyInfo>();

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
        /// Version diff information
        /// List of methods that were deleted from the class in newer versions
        /// </summary>
        public IList<MethodInfo> DeletedMethods { get; set; } = new List<MethodInfo>();

        /// <summary>
        /// Version diff information
        /// List of methods that were deleted from the class in newer versions
        /// </summary>
        public IList<PropertyInfo> DeletedProperties { get; set; } = new List<PropertyInfo>();

        /// <summary>
        /// The directory the class is extracted from
        /// </summary>
        public string Directory { get; set; } = string.Empty;

        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();

        public List<string> ImplementedInterfaces { get; set; } = new List<string>();

        public List<MethodInfo> Methods { get; set; } = new List<MethodInfo>();

        /// <summary>
        /// Name of the class / interface
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;

        public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();

        public ClassType Type { get; set; } = ClassType.Class;

        public string Id()
        {
            var id = $"{Namespace}.{Name}";
            if (!string.IsNullOrEmpty(id) && id[^1] == '?')
            {
                id = id[..^1];
            }

            return id;
        }

        public override string ToString() => Name;
    }
}
