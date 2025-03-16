using Gwiz.Core.Serializer;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Awiz.Core;
using System.Reflection;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace Awiz
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        private readonly ViewReader _viewReader = new();

        private List<IEdge> _edges = new();

        private List<INode> _nodes = new();

        public MainWindowViewModel()
        {
            _viewReader.Read("C:\\repo\\A-Wiz\\Awiz.Core.Test\\Assets\\ExtendsImplements\\");
            //_viewReader.Read("C:\\repo\\G-Wiz\\");

            var menuItem = new MenuBarItem
            {
                Title = "Views",
            };
            MenuItems.Add(menuItem);

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
                };

                menuItem.Items.Add(item);
            }
        }

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
    }
}
