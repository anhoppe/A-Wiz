using System;
using System.Collections.Generic;
using Awiz.Core;
using Awiz.Core.Contract;
using Gwiz.Core;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Mvvm;

namespace Awiz.ViewModel
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly ViewReader _viewReader = new();

        private IArchitectureView? _architectureView;

        private IGraph _graph = new Graph();

        public MainWindowViewModel()
        {
            //_viewReader.ReadProject("C:\\repo\\A-Wiz\\Awiz.Core.Test\\Assets\\ExtendsImplements\\");
            //_viewReader.ReadProject("C:\\repo\\G-Wiz\\");
            _viewReader.ReadProject("C:\\repo\\A-Wiz\\");

            CommitInfoPanelViewModel = new CommitInfoPanelViewModel(_viewReader.GitAccess);
            UseCasePanelViewModel = new UseCasePanelViewModel(CommitInfoPanelViewModel);

            UseCasePanelViewModel.Visibility = Visibility.Collapsed;
            ClassPanelViewModel.Visibility = Visibility.Collapsed;

            CreateMenu();
        }

        public ClassPanelViewModel ClassPanelViewModel { get; } = new();

        public CommitInfoPanelViewModel CommitInfoPanelViewModel { get; }

        public IGraph Graph
        {
            get => _graph;

            set
            {
                SetProperty(ref _graph, value);
            }
        }

        public List<MenuBarItem> MenuItems { get; } = new();

        public UseCasePanelViewModel UseCasePanelViewModel { get; }

        private void CreateMenu()
        {
            var fileMenuItem = new MenuBarItem
            {
                Title = "File",
            };
            MenuItems.Add(fileMenuItem);

            var saveItem = new MenuFlyoutItem()
            {
                Text = "Save",
            };

            saveItem.Click += (s, e) =>
            {
                if (_architectureView == null)
                {
                    throw new NullReferenceException("Saving failed, no architectur view is open");
                }

                _architectureView.Save();
            };
            fileMenuItem.Items.Add(saveItem);

            // Use Cases
            var useCasesMenuItem = new MenuBarItem
            {
                Title = "Use-Cases",
            };
            MenuItems.Add(useCasesMenuItem);

            foreach (var useCase in _viewReader.UseCases)
            {
                var item = new MenuFlyoutItem()
                {
                    Text = useCase,
                };

                item.Click += (s, e) =>
                {
                    _architectureView = _viewReader.LoadUseCase(useCase);
                    Graph = _architectureView.Graph ?? new Graph();
                    UseCasePanelViewModel.Graph = Graph;

                    UseCasePanelViewModel.Visibility = Visibility.Visible;
                    ClassPanelViewModel.Visibility = Visibility.Collapsed;

                    CommitInfoPanelViewModel.SetupGitInfo(Graph, _architectureView);
                };

                useCasesMenuItem.Items.Add(item);
            }

            // Views
            var viewsMenuItem = new MenuBarItem
            {
                Title = "Views",
            };
            MenuItems.Add(viewsMenuItem);

            foreach (var viewName in _viewReader.ClassDiagrams)
            {
                var item = new MenuFlyoutItem()
                {
                    Text = viewName,
                };

                item.Click += (s, e) =>
                {
                    _architectureView = _viewReader.LoadClassDiagram(viewName);
                    Graph = _architectureView.Graph ?? new Graph();

                    UseCasePanelViewModel.Visibility = Visibility.Collapsed;
                    ClassPanelViewModel.Visibility = Visibility.Visible;
                };

                viewsMenuItem.Items.Add(item);
            }
        }
    }
}
