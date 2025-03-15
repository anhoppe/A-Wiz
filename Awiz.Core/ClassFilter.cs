using Awiz.Core.CodeInfo;

namespace Awiz.Core
{
    public class ClassFilter : IClassFilter
    {
        private class ClassProvider : IClassProvider
        {
            public List<ClassInfo> Classes { get; } = new();
        }

        /// <summary>
        /// When set to true, the associations between classes are shown
        /// </summary>
        public bool EnableAssociations { get; set; }

        public AllowedLists Namespaces { get; set; } = new AllowedLists();

        public IClassProvider Filter(IClassProvider classProvider)
        {
            var filteredClasses = new ClassProvider();

            foreach (var classInfo in classProvider.Classes)
            {
                if (IsClassAdded(classInfo.Namespace))
                {
                    filteredClasses.Classes.Add(classInfo);
                }
            }

            return filteredClasses;
        }

        private bool IsClassAdded(string namesp)
        {
            bool addClass = true;

            if (Namespaces.Whitelist.Any())
            {
                addClass = Namespaces.Whitelist.Any(p => p == namesp);
            }

            addClass &= !Namespaces.Blacklist.Any(p => p == namesp);

            return addClass;
        }
    }

    public class AllowedLists
    {
        public List<string> Blacklist { get; set; } = new List<string>();

        public List<string> Whitelist { get; set; } = new List<string>();
    }
}
