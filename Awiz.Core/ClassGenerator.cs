using Awiz.Core;
using Gwiz.Core.Contract;

namespace Awiz.Core
{
    public class ClassGenerator
    {
        public Config Config { get; set; } = new Config();
        
        public void Generate(IClassProvider classProvider, IGraph graph)
        {
            foreach (var classInfo in classProvider.Classes)
            {
                if (IsClassAdded(classInfo.Namespace))
                { 
                    ClassNodeGenerator.Create(graph, classInfo);
                }
            }
        }

        internal IClassNodeGenerator ClassNodeGenerator { get; set; } = new ClassNodeGenerator();

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
