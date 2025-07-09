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

            Graph.AddNode("Actor")
                .WithSize(80, 80)
                .WithPos(100, 100)
                .Build();
        }

        private void AddBoundary()
        {
            if (Graph == null)
            {
                return;
            }

            Graph.AddNode("Boundary")
                .WithSize(300, 300)
                .WithPos(100, 100)
                .Build();
        }

        private void AddUseCase()
        {
            if (Graph == null)
            {
                return;
            }

            Graph.AddNode("UseCase")
                .WithSize(180, 120)
                .WithPos(120, 120)
                .Build();
        }
    }
}
