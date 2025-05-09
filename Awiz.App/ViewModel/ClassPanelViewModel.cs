using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CodeTree;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Awiz.ViewModel
{
    public class ClassPanelViewModel : BindableBase
    {
        private IArchitectureView? _architectureView;

        private string _baseClassName = string.Empty;

        private ClassInfo? _selectedClassInfo;

        private Visibility _visbility = Visibility.Collapsed;

        public ClassPanelViewModel()
        {
            AddBaseClassCommand = new DelegateCommand(() => 
            {
                if (ArchitectureView != null && _selectedClassInfo != null)
                {
                    ArchitectureView.AddBaseClassNode(_selectedClassInfo);
                }
            });

            AddInterfaceCommand = new DelegateCommand(() =>
            {
                if (ArchitectureView != null &&
                    ArchitectureWiz != null &&
                    SelectedInterfaceIndex != -1)
                {
                    var interfaceInfo = ArchitectureWiz.GetClassInfoById(ImplementedIntefaces[SelectedInterfaceIndex].Id);

                    if (interfaceInfo != null)
                    {
                        ArchitectureView.AddClassNode(interfaceInfo);
                    }
                }
            });
        }

        public ICommand AddBaseClassCommand { get; }
        
        public ICommand AddInterfaceCommand { get; }
        
        public string BaseClassName 
        {
            get => _baseClassName;
            set
            {
                SetProperty(ref _baseClassName, value);
            }
        }

        public IGraph? Graph { get; set; }

        public ObservableCollection<ClassInfo> ImplementedIntefaces { get; } = new();

        public Visibility Visibility
        {
            get => _visbility;
            set
            {
                SetProperty(ref _visbility, value);
            }
        }

        internal IDictionary<string, ClassNamespaceNode> ClassNamespaceNodes { get; set; } = new Dictionary<string, ClassNamespaceNode>();
        
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
                        _selectedClassInfo = classInfo;

                        if (!string.IsNullOrEmpty(_selectedClassInfo.BaseClass))
                        {                        
                            var baseClass = ArchitectureWiz.GetClassInfoById(_selectedClassInfo.BaseClass);

                            if (baseClass != null)
                            {
                                BaseClassName = baseClass.Name;
                            }
                        }

                        ImplementedIntefaces.Clear();
                        foreach (var implementedInterface in _selectedClassInfo.ImplementedInterfaces)
                        {
                            var interfaceInfo = ArchitectureWiz.GetClassInfoById(implementedInterface);

                            if (interfaceInfo != null)
                            {
                                ImplementedIntefaces.Add(interfaceInfo);
                            }
                        }
                    };
                }
            }
        }

        public int SelectedInterfaceIndex { get; set; } = -1;

        internal IArchitectureWiz? ArchitectureWiz { private get; set; }

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
