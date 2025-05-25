using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.Contract.CSharpParsing
{
    /// <summary>
    /// Nodes to represent classes in a menu tree
    /// </summary>
    public class ClassNamespaceNode
    {
        public List<ClassNamespaceNode> Children { get; } = new List<ClassNamespaceNode>();
     
        public List<ClassInfo> Classes { get; } = new List<ClassInfo>();

        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
