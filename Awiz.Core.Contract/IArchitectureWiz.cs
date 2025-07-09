using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;
using Awiz.Core.Contract.Git;

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
        /// List of all sequence diagrams available in a project
        /// </summary>
        List<string> SequenceDiagrams { get; }

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
        /// Returns the class namespace nodes as tree.
        /// </summary>
        /// <param name="includeInterfaces">If set to true, the returned tree contains the interfaces</param>
        /// <returns>Class tree sorted by namespace</returns>
        IDictionary<string, ClassNamespaceNode> GetClassNamespaceNodes(bool includeInterfaces);

        /// <summary>
        /// Loads a class diagram by name (all names from ClassDiagrams property are valid)
        /// </summary>
        /// <param name="classDiagramName">Name of the class diagram to load</param>
        /// <returns>The loaded architectural view</returns>
        IArchitectureView LoadClassDiagram(string classDiagramName);

        /// <summary>
        /// Loads a sequence diagram by name (all names from SequenceDiagrams property are valid)
        /// </summary>
        /// <param name="sequenceDiagramName">Name of the sequence diagram to load</param>
        /// <returns>The loaded architectural view containing the sequence diagram</returns>
        IArchitectureView LoadSequenceDiagram(string sequenceDiagramName);

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

        /// <summary>
        /// Sets a method to compute text sizes for nodes. Needed when nodes are supposed to fit
        /// to their text
        /// </summary>
        /// <param name="textSizeCalculator">Method that receives a string and returns the required space as a width/height tupe</param>
        void SetTextSizeCalculator(Func<string, (int, int)> textSizeCalculator);
    }
}
