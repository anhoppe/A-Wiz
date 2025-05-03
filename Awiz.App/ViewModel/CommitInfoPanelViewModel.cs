using Awiz.Core.Contract;
using Awiz.Core.Contract.Git;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.WebUI;

namespace Awiz.ViewModel
{
    public class CommitInfoPanelViewModel : BindableBase
    {
        private IArchitectureView? _architectureView;

        private IGitNodeInfo? _selectedNodeGitInfo;

        private Visibility _visbility = Visibility.Collapsed;

        public CommitInfoPanelViewModel(IGitRepo gitRepo)
        {
            RepositoryCommits.Clear();
            
            foreach (var commit in gitRepo.GetHistory())
            {
                RepositoryCommits.Add(new CommitViewModel(commit));
            }

            AddCommitCommand = new DelegateCommand(() =>
            {
                if (_selectedNodeGitInfo != null && SelectedRepositoryCommitIndex != -1)
                {
                    if (!_selectedNodeGitInfo.AssociatedCommits.Any(c => c.Sha == RepositoryCommits[SelectedRepositoryCommitIndex].Commit.Sha))
                    {
                        _selectedNodeGitInfo.AssociatedCommits.Add(RepositoryCommits[SelectedRepositoryCommitIndex].Commit);
                        AssociatedCommits.Add(RepositoryCommits[SelectedRepositoryCommitIndex]);
                    }
                }
            });

            RemoveCommitCommand = new DelegateCommand(() =>
            {
                if (_selectedNodeGitInfo != null && SelectedAssociatedCommitIndex != -1)
                {
                    _selectedNodeGitInfo.AssociatedCommits.RemoveAt(SelectedAssociatedCommitIndex);
                    AssociatedCommits.RemoveAt(SelectedAssociatedCommitIndex);
                }
            });
        }

        public ICommand AddCommitCommand { get; }

        public ObservableCollection<CommitViewModel> AssociatedCommits { get; set; } = new();

        public ICommand RemoveCommitCommand { get; }

        public ObservableCollection<CommitViewModel> RepositoryCommits { get; set; } = new();

        public int SelectedAssociatedCommitIndex { get; set; }
        
        public int SelectedRepositoryCommitIndex { get; set; }

        public Visibility Visibility
        {
            get => _visbility;
            set
            {
                SetProperty(ref _visbility, value);
            }
        }


        internal void SetupGitInfo(IGraph graph, IArchitectureView architectureView)
        {
            _architectureView = architectureView;

            _architectureView.NodeAdded += (sender, node) =>
            {
                node.SelectedChanged += (_, isSelected) =>
                {
                    SetNode(isSelected, node);
                };
            };

            foreach(var node in graph.Nodes)
            {
                node.SelectedChanged += (_, isSelected) =>
                {
                    SetNode(isSelected, node);
                };
            }
        }

        private void SetNode(bool isSelected, INode node)
        {
            AssociatedCommits.Clear();

            _selectedNodeGitInfo = null;

            if (isSelected && _architectureView != null)
            {
                _selectedNodeGitInfo = _architectureView.GetAssociatedCommits(node);

                foreach (var commit in _selectedNodeGitInfo.AssociatedCommits)
                {
                    AssociatedCommits.Add(new CommitViewModel(commit));
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
