namespace Awiz.Core.Contract.Git
{
    /// <summary>
    /// Represens the git information for a single node
    /// </summary>
    public interface IGitNodeInfo
    {
        /// <summary>
        /// Commits that are associated with a node
        /// </summary>
        List<Commit> AssociatedCommits { get; }
    }
}

