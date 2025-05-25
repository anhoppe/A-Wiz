using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Awiz.ViewModel.ClassDiagram;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;
using Awiz.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Awiz.View
{
    public sealed partial class SequencePanelView : UserControl
    {
        public SequencePanelView()
        {
            this.InitializeComponent();
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

        private void SequencePanelView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as SequencePanelViewModel;
            if (viewModel != null)
            {
                PopulateTree(viewModel.ClassNamespaceNodes);
            }
        }


        private void PopulateTree(IDictionary<string, ClassNamespaceNode> classNamespaceNodes)
        {
            foreach (var assemblyName in classNamespaceNodes.Keys)
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

                        var dataContext = DataContext as SequencePanelViewModel;
                        if (dataContext == null)
                        {
                            return;
                        }

                        dataContext.AddClassNode(classInfo);
                    }
                }
            };
        }
    }
}
