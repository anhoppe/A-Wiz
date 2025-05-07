using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CodeTree;

namespace Awiz.Core.CodeTree
{
    /// <summary>
    /// Creates a tree hierarchy of classes according to the namespaces
    /// </summary>
    internal class NamespaceBuilder : INamespaceBuilder
    {
        public NamespaceBuilder() { }

        public List<ClassNamespaceNode> Build(IList<ClassInfo> classInfos)
        {
            List<ClassNamespaceNode> roots = new();

            foreach (var classInfo in classInfos)
            {
                var namespaces = classInfo.Namespace.Split('.');

                var currentNode = roots.FirstOrDefault(n => n.Name == namespaces[0]);
                if (currentNode == null)
                {
                    currentNode = new ClassNamespaceNode { Name = namespaces[0] };
                    roots.Add(currentNode);
                }
                
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
