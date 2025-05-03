namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Information about a parameter of a method
    /// </summary>
    public class ParameterInfo
    {
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        
        public override string ToString() => $"{Type} {Name}";
    }
}
