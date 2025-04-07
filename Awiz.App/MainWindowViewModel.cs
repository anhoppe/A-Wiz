using System.Collections.Generic;
using Awiz.Core;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Mvvm;

namespace Awiz
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly ViewReader _viewReader = new();

        private List<IEdge> _edges = new();

        private LoadedView _loadedView = LoadedView.None;

        private List<INode> _nodes = new();

        public MainWindowViewModel()
        {
            _viewReader.ReadProject("C:\\repo\\A-Wiz\\Awiz.Core.Test\\Assets\\ExtendsImplements\\");
            //_viewReader.Read("C:\\repo\\G-Wiz\\");

            var fileMenuItem = new MenuBarItem
            {
                Title = "File",
            };
            MenuItems.Add(fileMenuItem);
            var saveItem = new MenuFlyoutItem()
            {
                Text = "Save",
            };

            saveItem.Click += (s, e) => {
                _viewReader.SaveView();
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

                item.Click += (s, e) => {
                    var useCaseGraph = _viewReader.GetUseCaseByName(useCase);
                    if (useCaseGraph != null)
                    {
                        Nodes = useCaseGraph.Nodes;
                        Edges = useCaseGraph.Edges;
                    }
                    LoadedView = LoadedView.UseCase;
                    UseCasePanelViewModel.Graph = useCaseGraph;
                };

                useCasesMenuItem.Items.Add(item);
            }

            // Views
            var viewsMenuItem = new MenuBarItem
            {
                Title = "Views",
            };
            MenuItems.Add(viewsMenuItem);

            foreach (var viewName in _viewReader.Views) 
            {
                var item = new MenuFlyoutItem()
                {
                    Text = viewName,
                };

                item.Click += (s, e) => {
                    var viewGraph = _viewReader.GetViewByName(viewName);
                    if (viewGraph != null)
                    {
                        Nodes = viewGraph.Nodes;
                        Edges = viewGraph.Edges;
                    }
                    LoadedView = LoadedView.Class;
                };

                viewsMenuItem.Items.Add(item);
            }
        }

        public ClassPanelViewModel ClassPanelViewModel { get; } = new();

        public List<IEdge> Edges 
        { 
            get 
            {
                return _edges;
            } 
            
            set 
            { 
                SetProperty(ref _edges, value);
            } 
        }

        public LoadedView LoadedView
        {
            get => _loadedView;
            set
            {
                _loadedView = value;

                switch (_loadedView)
                {
                    case LoadedView.None:
                        UseCasePanelViewModel.Visibility = Visibility.Collapsed;
                        ClassPanelViewModel.Visibility = Visibility.Collapsed;
                        break;
                    case LoadedView.UseCase:
                        UseCasePanelViewModel.Visibility = Visibility.Visible;
                        ClassPanelViewModel.Visibility = Visibility.Collapsed;
                        break;
                    case LoadedView.Class:
                        UseCasePanelViewModel.Visibility = Visibility.Collapsed;
                        ClassPanelViewModel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public List<MenuBarItem> MenuItems { get; } = new();

        public List<INode> Nodes 
        { 
            get
            {
                return _nodes;
            }

            set
            {
                SetProperty(ref _nodes, value);
            }
        }

        public UseCasePanelViewModel UseCasePanelViewModel { get; } = new();
    }
}
