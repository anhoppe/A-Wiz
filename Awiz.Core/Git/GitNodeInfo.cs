using Awiz.Core.Contract.Git;

namespace Awiz.Core.Git
{
    internal class GitNodeInfo : IGitNodeInfo
    {
        public GitNodeInfo()
        {
            AssociatedCommits = new();
        }

        public GitNodeInfo(List<Commit> commits)
        {
            AssociatedCommits = commits;
        }

        public List<Commit> AssociatedCommits { get; set; }
    }
}
