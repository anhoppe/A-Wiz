using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Awiz.Core.Contract.CSharpParsing;
using Gwiz.Core.Contract;
using Microsoft.UI.Xaml;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Awiz.ViewModel.ClassDiagram
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
                    var interfaceInfo = ArchitectureWiz.GetClassInfoById(ImplementedIntefaces[SelectedInterfaceIndex].Id());

                    if (interfaceInfo != null)
                    {
                        ArchitectureView.AddClassNode(interfaceInfo);
                    }
                }
            });
        }

        public ICommand AddBaseClassCommand { get; }

        public ObservableCollection<MethodInfoViewModel> AddedMethods { get; } = new();

        public int AddedMethodsSelectedIndex { get; set; } = -1;

        public ObservableCollection<PropertyInfoViewModel> AddedProperties { get; } = new();

        public int AddedPropertiesSelectedIndex { get; set; } = -1;

        public ICommand AddInterfaceCommand { get; }

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
                        ClearVersionUpdateInfo();

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

                    _architectureView.ShowVersionDiff += (sender, classInfo) =>
                    {
                        ClearVersionUpdateInfo();
                        FillVersionUpdateInfo(classInfo);
                        _selectedClassInfo = classInfo;
                    };
                }
            }
        }

        public string BaseClassName
        {
            get => _baseClassName;
            set
            {
                SetProperty(ref _baseClassName, value);
            }
        }

        public ObservableCollection<MethodInfoViewModel> DeletedMethods { get; } = new();

        public int DeletedMethodsSelectedIndex { get; set; } = -1;

        public ObservableCollection<PropertyInfoViewModel> DeletedProperties { get; } = new();

        public int DeletedPropertiesSelectedIndex { get; set; } = -1;

        public IGraph? Graph { get; set; }

        public ObservableCollection<ClassInfo> ImplementedIntefaces { get; } = new();

        public int SelectedInterfaceIndex { get; set; } = -1;

        public Visibility Visibility
        {
            get => _visbility;
            set
            {
                SetProperty(ref _visbility, value);
            }
        }

        public IVersionUpdater? VersionUpdater { get; internal set; }

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

        private void Update(ClassInfo classInfo)
        {
            ClearVersionUpdateInfo();
            FillVersionUpdateInfo(classInfo);
        }

        private void ClearVersionUpdateInfo()
        {
            AddedPropertiesSelectedIndex = -1;
            DeletedPropertiesSelectedIndex = -1;
            AddedMethodsSelectedIndex = -1;
            DeletedMethodsSelectedIndex = -1;

            AddedProperties.Clear();
            DeletedProperties.Clear();
            AddedMethods.Clear();
            DeletedMethods.Clear();
        }

        private void FillVersionUpdateInfo(ClassInfo classInfo)
        {
            if (VersionUpdater == null)
            {
                throw new NullReferenceException("VersionUpdated is not set");
            }    

            foreach(var item in classInfo.AddedProperties)
            {
                AddedProperties.Add(new PropertyInfoViewModel(classInfo, item, Update, VersionUpdater));
            }

            foreach(var item in classInfo.DeletedProperties)
            {
                DeletedProperties.Add(new PropertyInfoViewModel(classInfo, item, Update, VersionUpdater));
            }

            foreach (var item in classInfo.AddedMethods)
            {
                AddedMethods.Add(new MethodInfoViewModel(classInfo, item, Update, VersionUpdater));
            }

            foreach (var item in classInfo.DeletedMethods)
            {
                DeletedMethods.Add(new MethodInfoViewModel(classInfo, item, Update, VersionUpdater));
            }
        }
    }
}
