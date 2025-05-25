using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;

namespace Awiz.Core.CSharpParsing
{
    /// <summary>
    /// Creates a tree hierarchy of classes according to the namespaces
    /// </summary>
    internal class NamespaceBuilder : INamespaceBuilder
    {
        private IDictionary<string, ClassNamespaceNode> _classTreeRootWithInterfaces = new Dictionary<string, ClassNamespaceNode>();
        
        private IDictionary<string, ClassNamespaceNode> _classTreeRootWithoutInterfaces = new Dictionary<string, ClassNamespaceNode>();
        
        public NamespaceBuilder() { }

        public void Build(IList<ClassInfo> classInfos)
        {
            BuildTree(_classTreeRootWithInterfaces, classInfos, true);
            BuildTree(_classTreeRootWithoutInterfaces, classInfos, false);
        }

        public IDictionary<string, ClassNamespaceNode> GetClassTree(bool includeInterfaces) => includeInterfaces ? _classTreeRootWithInterfaces : _classTreeRootWithoutInterfaces;

        private static void BuildTree(IDictionary<string, ClassNamespaceNode> classTreeRoot, IList<ClassInfo> classInfos, bool includeInterfaces)
        {
            foreach (var classInfo in classInfos)
            {
                if (!classTreeRoot.ContainsKey(classInfo.Assembly))
                {
                    classTreeRoot[classInfo.Assembly] = new ClassNamespaceNode { Name = classInfo.Namespace };
                }
                var currentNode = classTreeRoot[classInfo.Assembly];

                var namespaces = classInfo.Namespace.Split('.');
                foreach (var namespaceName in namespaces.Skip(1))
                {
                    var childNode = currentNode.Children.FirstOrDefault(n => n.Name == namespaceName);
                    if (childNode == null)
                    {
                        childNode = new ClassNamespaceNode { Name = namespaceName };
                        currentNode.Children.Add(childNode);
                    }
                    currentNode = childNode;
                }

                if (classInfo.Type != ClassType.Interface || includeInterfaces)
                {
                    currentNode.Classes.Add(classInfo);
                }
            }
        }
    }
}
