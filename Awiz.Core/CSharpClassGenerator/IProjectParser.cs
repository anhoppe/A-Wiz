namespace Awiz.Core.CSharpClassGenerator
{
    /// <summary>
    /// Used to determine which cs file belongs to which assembly
    /// </summary>
    internal interface IProjectParser
    {
        /// <summary>
        /// Returns the name of the project the file belongs to.
        /// This method can be used after calling ParsseProject.
        /// </summary>
        /// <param name="fileName">Name of the file to get the project name for</param>
        /// <returns></returns>
        string GetProject(string fileName);

        /// <summary>
        /// Parses the project files in the given directory and its subdirectories.
        /// Current implementation is just a simple heuristic:
        /// Each folder in the root directory is considered a project.
        /// .cs files in the folders are assigned to the project.
        /// </summary>
        /// <param name="pathToRepo">Directpory that contains the repository with different projects</param>
        void ParseProject(string pathToRepo);
    }
}
