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
        /// Returns a class by its ID, used to find the class info object after reloading
        /// </summary>
        /// <param name="classId">ID of the class instance to find</param>
        /// <returns>Instance of the class info object with the given ID</returns>
        ClassInfo GetClassInfoById(string classId);

        /// <summary>
        /// For a class info that describes an interface all implementing classes are returned
        /// </summary>
        /// <param name="interfaceInfo"></param>
        /// <returns></returns>
        IList<ClassInfo> GetImplementations(ClassInfo interfaceInfo);

        /// <summary>
        /// Returns a method by its ID, used to find the method info object after reloading
        /// </summary>
        /// <param name="methodId">ID of the method instance to find</param>
        /// <returns>Instance of the method info object with the given ID</returns>
        MethodInfo GetMethodInfoById(string methodId);

    }
}
