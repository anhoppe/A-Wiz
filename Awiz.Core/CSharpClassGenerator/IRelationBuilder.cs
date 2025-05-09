using Awiz.Core.Contract.CodeInfo;
using Gwiz.Core.Contract;

namespace Awiz.Core.CSharpClassGenerator
{
    /// <summary>
    /// Responsible for creation of relations in a class diagram (extends, implements, associations)
    /// </summary>
    internal interface IRelationBuilder
    {
        /// <summary>
        /// Builds the relations (extends, implements, associations) to existing classes in the diagram
        /// </summary>
        /// <param name="graph">Reference to the graph the relation is added to</param>
        /// <param name="addedClassInfo">The class or interface that was added to the diagram</param>
        /// <param name="classesInDiagram">List of classes / interfaces that are already in the diagram</param>
        void Build(IGraph graph, ClassInfo addedClassInfo, IList<ClassInfo> classesInDiagram);
    }
}
