namespace Awiz.Core.Contract.Git
{
    /// <summary>
    /// Access to repository information for the loaded view / node
    /// </summary>
    public interface IGitRepo
    {
        /// <summary>
        /// Gets the entire history of the loaded project
        /// </summary>
        /// <returns></returns>
        List<Commit> GetHistory();
    }
}
