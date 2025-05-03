using Awiz.Core.Contract.Git;

namespace Awiz.Core
{
    internal interface ILoadableGitRepo : IGitRepo
    {
        void LoadRepo(string pathToRepo);
    }
}
