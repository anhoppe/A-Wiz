using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Awiz.Core.Contract.CodeTree;
using Awiz.Core.Contract.CodeInfo;
using Awiz.ViewModel.ClassDiagram;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Awiz.View
{
    public sealed partial class ClassPanelView : UserControl
    {
        public ClassPanelView()
        {
            this.InitializeComponent();
        }

        private void ClassPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ClassPanelViewModel;
            if (viewModel != null)
            {
                PopulateTree(viewModel.ClassNamespaceNodes);
            }
        }

        public void PopulateTree(IDictionary<string, ClassNamespaceNode> classNamespaceNodes)
        {
            foreach(var assemblyName in classNamespaceNodes.Keys)
            {
                var assemblyNode = new TreeViewNode
                {
                    Content = assemblyName,
                    IsExpanded = true
                };
                _classNodeTree.RootNodes.Add(assemblyNode);
                var node = CreateTreeViewNode(classNamespaceNodes[assemblyName]);
                assemblyNode.Children.Add(node);
            }

            _classNodeTree.SelectionChanged += (sender, args) =>
            {
                if (args.AddedItems.Count > 0)
                {
                    var item = args.AddedItems.First();
                    var treeNodeItem = item as TreeViewNode;

                    if (treeNodeItem != null)
                    {
                        var classInfo = treeNodeItem.Content as ClassInfo;
                        if (classInfo == null)
                        {
                            return;
                        }

                        var dataContext = DataContext as ClassPanelViewModel;
                        if (dataContext == null)
                        {
                            return;
                        }

                        dataContext.AddClassNode(classInfo);
                    }
                }
            };

        }

        private TreeViewNode CreateTreeViewNode(ClassNamespaceNode node)
        {
            var treeNode = new TreeViewNode 
            { 
                Content = node, 
                IsExpanded = true 
            };

            foreach (var child in node.Children)
            {
                treeNode.Children.Add(CreateTreeViewNode(child));
            }

            foreach (var childClass in node.Classes)
            {
                var treeViewNode = new TreeViewNode()
                {
                    Content = childClass,
                };
                treeNode.Children.Add(treeViewNode);
            }

            return treeNode;
        }
    }
}
