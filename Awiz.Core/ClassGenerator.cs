using Awiz.Core;
using Gwiz.Core.Contract;
using YamlDotNet.Serialization;

namespace Awiz.Core
{
    /// <summary>
    /// Generates the classes given the view configuration, 
    /// the class provider and the graph to be filled.
    /// </summary>
    public class ClassGenerator
    {
        public Config Config { get; set; } = new Config();

        internal IClassNodeGenerator ClassNodeGenerator { get; set; } = new ClassNodeGenerator();

        public void Generate(IClassProvider classProvider, IGraph graph)
        {
            AddClassesToGraph(classProvider, graph);

            if (Config.EnableAssociations)
            {
                AddAssociationsToGraph(classProvider, graph);
            }
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
                if (IsClassAdded(classInfo.Namespace))
                {
                    ClassNodeGenerator.CreateClassNode(graph, classInfo);
                }
            }
        }


        private bool IsClassAdded(string namesp)
        {
            bool addClass = false;

            if (Config.Namespaces.Whitelist.Any())
            {
                addClass = Config.Namespaces.Whitelist.Any(p => p == namesp);
            }
            else
            {
                addClass = true;
            }

            addClass &= !Config.Namespaces.Blacklist.Any(p => p == namesp);

            return addClass;
        }
    }
}
