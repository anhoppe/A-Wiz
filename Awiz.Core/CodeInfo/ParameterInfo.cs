namespace Awiz.Core.CodeInfo
{
    public class ParameterInfo
    {
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        
        public override string ToString() => $"{Type} {Name}";
    }
}
