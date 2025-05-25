namespace Awiz.Core.CSharpParsing
{
    internal class ProjectParser : IProjectParser
    {
        private string _pathToProject = string.Empty;

        /// <summary>
        /// In this implementation we just return the first sub-folder from the file
        /// The assumption is that the root folder is the repo and the first folder the project
        /// This is just a rough heuritic to have some convenience when browsing the classes
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Name of the project the file belongs to</returns>
        public string GetProject(string fileName)
        {
            var relativePath = Path.GetRelativePath(_pathToProject, fileName);
            string directoryPath = Path.GetDirectoryName(relativePath) ?? string.Empty;

            // Split into folders
            string[] folders = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (folders.Length >= 1)
            {
                return folders[0];
            }

            return string.Empty;
        }

        public void ParseProject(string pathToRepo)
        {
            _pathToProject = pathToRepo;
        }
    }
}
