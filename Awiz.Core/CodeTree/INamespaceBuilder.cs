using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CodeTree;

namespace Awiz.Core.CodeTree
{
    /// <summary>
    /// Generates the namespace tree that can be used to group the source files based on the parsed classes
    /// </summary>
    internal interface INamespaceBuilder
    {
        List<ClassNamespaceNode> Build(IList<ClassInfo> classInfos);
    }
}
