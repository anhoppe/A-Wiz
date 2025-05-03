using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.CSharpClassGenerator
{
    /// <summary>
    /// Factory class to generate A-Wiz nodes from class info
    /// </summary>
    internal interface IClassNodeGenerator
    {
        /// <summary>
        /// Generates an association between two classes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from">The class that is owner of the instance to the 'to' class</param>
        /// <param name="to">The 'to' class, the instance that is hold be 'from'</param>
        void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to);

        /// <summary>
        /// Generates an association between two classes with multiplicities
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from">The class that is owner of the instance to the 'to' class</param>
        /// <param name="to">The 'to' class, the instance that is hold be 'from'</param>
        /// <param name="fromMultiplicity">Multiplicity label on 'from' side</param>
        /// <param name="toMultiplicity">Multiplicity label on 'to' side</param>
        void CreateAssociation(IGraph graph, ClassInfo from, ClassInfo to, string fromMultiplicity, string toMultiplicity);

        /// <summary>
        /// Creates a node to represent a class
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="classInfo"></param>
        void CreateClassNode(IGraph graph, ClassInfo classInfo);

        /// <summary>
        /// Generates an extension between baseClass and derivedClass
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="baseClass"></param>
        /// <param name="derivedClass"></param>
        void CreateExtension(IGraph graph, ClassInfo baseClass, ClassInfo derivedClass);

        /// <summary>
        /// Generates an edge indicating and implementation of an interface by a class
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="implementedInterface">The interface that is implemented by the class</param>
        /// <param name="implementingClass">The class that is implementing the interface</param>
        void CreateImplementation(IGraph graph, ClassInfo implementedInterface, ClassInfo implementingClass);
    }
}
