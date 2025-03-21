using Gwiz.Core.Contract;

namespace Awiz.Core
{
    /// <summary>
    /// Generates the classes given the view configuration, 
    /// the class provider and the graph to be filled.
    /// </summary>
    public class ClassGenerator
    {
        public AnnotationOptions AnnotationOptions { get; set; } = new();

        public IClassFilter ClassFilter { get; set; } = new ClassFilter();

        internal IClassNodeGenerator ClassNodeGenerator { get; set; } = new ClassNodeGenerator();

        public void Generate(IClassProvider classProvider, IGraph graph)
        {
            var filteredClassProvider = ClassFilter.Filter(classProvider);

            AddClassesToGraph(filteredClassProvider, graph);

            if (AnnotationOptions.EnableAssociations)
            {
                AddAssociationsToGraph(filteredClassProvider, graph);
            }

            AddExtensionsAndImplementationsToGraph(filteredClassProvider, graph);
        }

        private void AddAssociationsToGraph(IClassProvider classProvider, IGraph graph)
        {
            foreach (var classInfo in classProvider.Classes)
            {
                foreach (var classInfo2 in classProvider.Classes)
                {
                    if (!object.ReferenceEquals(classInfo, classInfo2))
                    {
                        foreach (var prop in classInfo.Properties)
                        {
                            if (prop.Type == classInfo2.Name)
                            {
                                ClassNodeGenerator.CreateAssociation(graph, classInfo, classInfo2);
                            }
                        }
                    }
                }
            }
        }

        private void AddClassesToGraph(IClassProvider classProvider, IGraph graph)
        {
            foreach (var classInfo in classProvider.Classes)
            {
                ClassNodeGenerator.CreateClassNode(graph, classInfo);
            }
        }

        private void AddExtension(IClassProvider classProvider, IGraph graph, CodeInfo.ClassInfo classInfo)
        {
            if (!string.IsNullOrEmpty(classInfo.BaseClass))
            {
                var baseClass = classProvider.Classes.FirstOrDefault(p => p.Namespace + "." + p.Name == classInfo.BaseClass);
                if (baseClass != null)
                {
                    ClassNodeGenerator.CreateExtension(graph, baseClass, classInfo);
                }
            }
        }

        private void AddExtensionsAndImplementationsToGraph(IClassProvider classProvider, IGraph graph)
        {
            foreach (var classInfo in classProvider.Classes)
            {
                // Add edge for base class
                AddExtension(classProvider, graph, classInfo);

                // Add edges for interfaces implemented by the class
                AddImplementations(classProvider, graph, classInfo);
            }
        }

        private void AddImplementations(IClassProvider classProvider, IGraph graph, CodeInfo.ClassInfo classInfo)
        {
            foreach (var implInterface in classInfo.ImplementedInterfaces)
            {
                var implementedInterface = classProvider.Classes.FirstOrDefault(p => implInterface == p.Namespace + "." + p.Name);

                if (implementedInterface != null)
                {
                    ClassNodeGenerator.CreateImplementation(graph, implementedInterface, classInfo);
                }
            }
        }
    }
}
