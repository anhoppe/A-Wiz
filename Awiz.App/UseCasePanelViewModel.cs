﻿using Gwiz.Core.Contract;
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
            AddBoundaryCommand = new DelegateCommand(AddBoundary);
            AddUseCaseCommand = new DelegateCommand(AddUseCase);
        }

        public ICommand AddActorCommand { get; }
        
        public ICommand AddBoundaryCommand { get; }
        
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
