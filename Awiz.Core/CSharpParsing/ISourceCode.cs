using Awiz.Core.Contract.CodeInfo;

namespace Awiz.Core.CSharpParsing
{
    /// <summary>
    /// Gives access to the source code information
    /// </summary>
    public interface ISourceCode
    {
        /// <summary>
        /// List of classes in the source code
        /// </summary>
        List<ClassInfo> Classes { get; }

        /// <summary>
        /// Returns the call sites of a single method
        /// </summary>
        /// <param name="method">Method to get the call sites for</param>
        /// <returns>List of call sites, empty list if the method has no call sites</returns>
        IList<CallSite> GetCallSites(MethodInfo method);

        /// <summary>
        /// For a class info that describes an interface all implementing classes are returned
        /// </summary>
        /// <param name="interfaceInfo"></param>
        /// <returns></returns>
        IList<ClassInfo> GetImplementations(ClassInfo interfaceInfo);

    }
}
