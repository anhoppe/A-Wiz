namespace Awiz.Core.Contract.CodeInfo
{
    /// <summary>
    /// Info about methods in classes
    /// </summary>
    public class MethodInfo
    {
        private string _id = string.Empty;

        public string AccessModifier { get; set; } = string.Empty;

        public string Id 
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = $"{ParentClass.Id}.{Name}.{string.Join(", ", Parameters.Select(p => p.ToString()))}";
                }

                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string Name { get; set; } = string.Empty;
        
        public ClassInfo ParentClass { get; set; } = new ClassInfo();

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
