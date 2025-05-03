using LibGit2Sharp;

namespace Awiz.Core.Git
{
    public class GitRepo : ILoadableGitRepo
    {
        Repository? _repo;

        public GitRepo()
        {
        }

        ~GitRepo()
        {
            if (_repo != null)
            {
                _repo.Dispose();
            }        
        }

        public List<Contract.Git.Commit> GetHistory()
        {
            var history = new List<Contract.Git.Commit>();

            if (_repo != null)
            {            
                foreach (var commit in _repo.Commits)
                {
                    var c = new Contract.Git.Commit()
                    {
                        Author = commit.Author.Name,
                        Sha = commit.Sha,
                        Message = commit.MessageShort
                    };

                    history.Add(c);
                }
            }

            return history;
        }

        public void LoadRepo(string pathToRepo)
        {
            _repo = new Repository(pathToRepo);
        }
    }
}
