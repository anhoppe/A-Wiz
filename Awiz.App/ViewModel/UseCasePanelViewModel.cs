using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace Awiz.ViewModel
{
    public class UseCasePanelViewModel : BindableBase
    {
        private CommitInfoPanelViewModel _commitInfoViewModel;

        private IGraph? _graph;
        
        private Visibility _visbility = Visibility.Collapsed;

        public UseCasePanelViewModel(CommitInfoPanelViewModel commitInfoPabelViewModel)
        {
            _commitInfoViewModel = commitInfoPabelViewModel;

            AddActorCommand = new DelegateCommand(AddActor);
            AddBoundaryCommand = new DelegateCommand(AddBoundary);
            AddUseCaseCommand = new DelegateCommand(AddUseCase);
        }

        public ICommand AddActorCommand { get; }
        
        public ICommand AddBoundaryCommand { get; }
        
        public ICommand AddUseCaseCommand { get; }

        public IGraph? Graph
        { 
            private get => _graph;
            set
            {
                _graph = value;

                if (_graph != null)
                {
                    foreach (var node in _graph.Nodes)
                    {
                        node.SelectedChanged += (sender, isSelected) =>
                        {
                            if (isSelected)
                            {
                                _commitInfoViewModel.Visibility = Visibility.Visible;
                            }
                        };
                    }
                }
            }
        }

        public Visibility Visibility 
        {
            get => _visbility;
            set
            {
                SetProperty(ref _visbility, value);
            }
        }

        private void AddActor()
        {
            if (Graph == null)
            {
                return;
            }

            var node = Graph.AddNode("Actor");
            node.Width = 80;
            node.Height = 80;

            node.X = 100;
            node.Y = 100;
        }

        private void AddBoundary()
        {
            if (Graph == null)
            {
                return;
            }

            var node = Graph.AddNode("Boundary");
            node.Width = 300;
            node.Height = 300;

            node.X = 100;
            node.Y = 100;
        }

        private void AddUseCase()
        {
            if (Graph == null)
            {
                return;
            }

            var node = Graph.AddNode("UseCase");
            node.Width = 180;
            node.Height = 120;

            node.X = 120;
            node.Y = 120;
        }
    }
}
