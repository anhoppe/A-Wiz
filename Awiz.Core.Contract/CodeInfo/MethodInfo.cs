namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Info about methods in classes
    /// </summary>
    public class MethodInfo
    {
        public string AccessModifier { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        
        public string ReturnType { get; set; } = string.Empty;

        public string AccessModifierShort() => AccessModifier.ToLower() switch
        {
            "public" => "+",
            "private" => "-",
            "protected" => "#",
            "internal" => "~",
            _ => "",
        };

        public override string ToString() => $"{AccessModifierShort()} {Name}({string.Join(", ", Parameters.Select(p => p.ToString()))}): {ReturnType}";
    }
}
