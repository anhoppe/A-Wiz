using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CodeTree;

namespace Awiz.Core.CSharpClassGenerator
{
    /// <summary>
    /// Creates a tree hierarchy of classes according to the namespaces
    /// </summary>
    internal class NamespaceBuilder : INamespaceBuilder
    {
        public NamespaceBuilder() { }

        public IDictionary<string, ClassNamespaceNode> Build(IList<ClassInfo> classInfos)
        {
            var roots = new Dictionary<string, ClassNamespaceNode>();

            foreach (var classInfo in classInfos)
            {
                if (!roots.ContainsKey(classInfo.Assembly))
                {
                    roots[classInfo.Assembly] = new ClassNamespaceNode { Name = classInfo.Namespace };
                }
                var currentNode = roots[classInfo.Assembly];

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

                currentNode.Classes.Add(classInfo);
            }

            return roots;
        }
    }
}
