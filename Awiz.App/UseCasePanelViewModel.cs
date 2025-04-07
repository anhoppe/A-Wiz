using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Awiz
{
    public class UseCasePanelViewModel : BindableBase
    {
        private Visibility _visbility = Visibility.Collapsed;

        public UseCasePanelViewModel()
        {
            AddActorCommand = new DelegateCommand(AddActor);
            AddUseCaseCommand = new DelegateCommand(AddUseCase);
        }

        public ICommand AddActorCommand { get; }
        
        public ICommand AddUseCaseCommand { get; }

        public IGraph? Graph { private get; set; }

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

        private void AddUseCase()
        {
            if (Graph == null)
            {
                return;
            }

            var node = Graph.AddNode("UseCase");
            node.Width = 100;
            node.Height = 80;

            node.X = 100;
            node.Y = 100;
        }
    }
}
