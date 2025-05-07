using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.Contract.CodeTree
{
    public class ClassNamespaceNode
    {
        public List<ClassNamespaceNode> Children { get; } = new List<ClassNamespaceNode>();
     
        public List<ClassInfo> Classes { get; } = new List<ClassInfo>();

        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
