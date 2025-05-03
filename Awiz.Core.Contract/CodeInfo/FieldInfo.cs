namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Info about fields in classes
    /// </summary>
    public class FieldInfo
    {
        public string AccessModifier { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string Type { get; set; } = string.Empty;
        
        public override string ToString() => $"{AccessModifier} {Type} {Name}";
    }
}
