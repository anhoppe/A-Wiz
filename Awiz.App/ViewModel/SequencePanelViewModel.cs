using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace Awiz.ViewModel
{
    public class SequencePanelViewModel : BindableBase
    {
        private IArchitectureView? _architectureView;

        private Visibility _visibility = Visibility.Collapsed;

        public IArchitectureView? ArchitectureView
        {
            get => _architectureView;
            internal set
            {
                _architectureView = value;

                if (_architectureView != null && ArchitectureWiz != null)
                {
                    _architectureView.ClassSelected += (sender, classInfo) =>
                    {
                    };
                }
            }
        }

        public IGraph? Graph { get; set; }

        public Visibility Visibility 
        { 
            get => _visibility;
            set
            {
                SetProperty(ref _visibility, value);
            }
        }
        
        internal IArchitectureWiz? ArchitectureWiz { private get; set; }

        internal IDictionary<string, ClassNamespaceNode> ClassNamespaceNodes { get; set; } = new Dictionary<string, ClassNamespaceNode>();

        internal void AddClassNode(ClassInfo classInfo)
        {
            if (Graph != null && ArchitectureView != null)
            {
                ArchitectureView.AddClassNode(classInfo);
            }
        }

        internal void SetClassTree(IDictionary<string, ClassNamespaceNode> classNamespaceNodes)
        {
            ClassNamespaceNodes = classNamespaceNodes;
        }
    }
}
