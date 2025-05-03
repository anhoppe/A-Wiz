using Awiz.Core.Contract.Git;
using Microsoft.UI.Xaml.Media.Animation;

namespace Awiz.ViewModel
{
    public class CommitViewModel
    {
        public CommitViewModel(Commit commit)
        {
            Commit = commit;
            Message = commit.Message;
            Sha = commit.ShortSha();
        }

        public Commit Commit { get; }

        public string Message { get; }

        public string Sha { get; }
    }
}
