using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CodeTree;
using Awiz.Core.Contract.Git;
using Gwiz.Core.Contract;

namespace Awiz.Core.Contract
{
    /// <summary>
    /// A-Wiz interface that is used by a client to access the views on the architecture of a project
    /// </summary>
    public interface IArchitectureWiz
    {
        /// <summary>
        /// List of all class diagrams available in the project
        /// </summary>
        List<string> ClassDiagrams { get; }

        /// <summary>
        /// Contains class infos of all classes / interfaces after repo was loaded
        /// </summary>
        List<ClassInfo> ClassInfos { get; }

        /// <summary>
        /// List of all classes in the repo
        /// </summary>
        List<ClassNamespaceNode> ClassNamespaceNodes { get; }

        /// <summary>
        /// List of all use cases available in a project
        /// </summary>
        List<string> UseCases { get; }

        /// <summary>
        /// Retrieves the class info of a class by its id
        /// </summary>
        /// <param name="id">ID of the class to be found</param>
        /// <returns>Reference to the class info</returns>
        ClassInfo? GetClassInfoById(string id);

        /// <summary>
        /// Loads a class diagram by name (all names from ClassDiagrams property are valid)
        /// </summary>
        /// <param name="classDiagramName">Name of the class diagram to load</param>
        /// <returns>The loaded architectural view</returns>
        IArchitectureView LoadClassDiagram(string classDiagramName);

        /// <summary>
        /// Loadas a use case diagram by name (all anems from UseCases property are valid)
        /// </summary>
        /// <param name="useCaseName">Name of the use case to load</param>
        /// <returns>The loaded architectural view</returns>
        IArchitectureView LoadUseCase(string useCaseName);

        /// <summary>
        /// Reads a project. After reading a project the information on all diagrams is available
        /// </summary>
        /// <param name="pathToRepo"></param>
        IGitRepo ReadProject(string pathToRepo);
    }
}
