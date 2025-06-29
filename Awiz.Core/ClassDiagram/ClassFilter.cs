using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.CSharpParsing;

namespace Awiz.Core.ClassDiagram
{
    public class ClassFilter : IClassFilter
    {
        private class ClassProvider : ISourceCode
        {
            public List<ClassInfo> Classes { get; } = new();

            public IList<CallSite> GetCallSites(MethodInfo method)
            {
                throw new NotImplementedException();
            }

            public ClassInfo GetClassInfoById(string classId)
            {
                throw new NotImplementedException();
            }

            public IList<ClassInfo> GetImplementations(ClassInfo interfaceInfo)
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetMethodInfoById(string methodId)
            {
                throw new NotImplementedException();
            }
        }

        private readonly List<string> _allowedFiles = new();

        private readonly DynamicDeserializerYaml _dynamicDeserializer = new();
        public ClassFilter() { }

        public ClassFilter(string pathToRepo, string viewName)
        {
            IEnumerable<string> wizFiles = GetAllWizYamlFiles(pathToRepo);

            foreach (var file in wizFiles)
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var result = _dynamicDeserializer.Deserialize(stream);

                    foreach (var project in result.Views)
                    {
                        if (project["Name"] == viewName)
                        {
                            foreach (var included in project["Include"])
                            {
                                _allowedFiles.Add(Path.Combine(Path.GetDirectoryName(file), included));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When set to true, the associations between classes are shown
        /// </summary>
        public bool EnableAssociations { get; set; }

        public ISourceCode Filter(ISourceCode classProvider)
        {
            var filteredClasses = new ClassProvider();

            foreach (var classInfo in classProvider.Classes)
            {
                if (IsClassAdded(classInfo))
                {
                    filteredClasses.Classes.Add(classInfo);
                }
            }

            return filteredClasses;
        }

        static IEnumerable<string> GetAllWizYamlFiles(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException($"The directory '{rootDirectory}' does not exist.");
            }

            return Directory.EnumerateFiles(rootDirectory, "wiz.yaml", SearchOption.AllDirectories);
        }

        private bool IsClassAdded(ClassInfo classInfo)
        {
            var fileNameWithoutExt = Path.Combine(
                Path.GetDirectoryName(classInfo.Directory) ?? string.Empty,
                Path.GetFileNameWithoutExtension(classInfo.Directory));

            return _allowedFiles.Contains(fileNameWithoutExt);
        }
    }
}
