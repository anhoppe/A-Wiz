namespace Awiz.Core.Contract.Git
{
    /// <summary>
    /// Poco for commit information
    /// </summary>
    public class Commit
    {
        public string Author { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;

        public string Sha { get; set; } = string.Empty;
    }

    public static class CommitExtensions
    {
        // Implemented as an extension method to avoid serialization problems
        public static string ShortSha(this Commit commit)
        {
            return commit.Sha.Length > 7 ? commit.Sha.Substring(0, 7) : commit.Sha;
        }
    }
}
