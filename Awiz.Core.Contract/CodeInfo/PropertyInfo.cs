namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Represents a property of a class
    /// </summary>
    public class PropertyInfo
    {
        public string AccessModifier { get; set; } = string.Empty;
        
        public ClassInfo GenericType { get; set; } = new();

        public bool IsEnumerable { get; set; } = false;

        public string Name { get; set; } = string.Empty;

        public string TypeNamespace { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public string AccessModifierShort() => AccessModifier.ToLower() switch
        {
            "public" => "+",
            "private" => "-",
            "protected" => "#",
            "internal" => "~",
            _ => "",
        };

        public string TypeId()
        {
            var id = $"{TypeNamespace}.{TypeName}";
            if (!string.IsNullOrEmpty(id) && id[^1] == '?')
            {
                id = id[..^1];
            }
            return id;
        }

        public override string ToString() => $"{AccessModifierShort()} {TypeName} {Name}";
    }
}
