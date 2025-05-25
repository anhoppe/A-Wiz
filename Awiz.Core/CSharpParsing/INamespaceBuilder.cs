using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;

namespace Awiz.Core.CSharpParsing
{
    /// <summary>
    /// Generates the namespace tree that can be used to group the source files based on the parsed classes
    /// </summary>
    internal interface INamespaceBuilder
    {
        /// <summary>
        /// Builds the namespace tree from the given class information
        /// </summary>
        /// <param name="classInfos"></param>
        /// <returns>Returns a dictinary with a namespace tree in each</returns>
        void Build(IList<ClassInfo> classInfos);

        IDictionary<string, ClassNamespaceNode> GetClassTree(bool includeInterfaces);
    }
}
