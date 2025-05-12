using Awiz.Core.Contract.CodeInfo;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace Awiz.ViewModel.ClassDiagram
{
    public class MethodInfoViewModel : ElementViewModel
    {
        private MethodInfo _methodInfo;

        public MethodInfoViewModel(ClassInfo classInfo, 
            MethodInfo methodInfo, 
            Action<ClassInfo> updateAction, 
            IVersionUpdater versionUpdater) : 
            base(classInfo, updateAction, versionUpdater)
        {
            _methodInfo = methodInfo;
        }

        public ICommand ApplyAddMethodCommand => new DelegateCommand(() =>
        {
            _versionUpdater.UpdateAdd(_classInfo, _methodInfo);
            _updateAction.Invoke(_classInfo);
        });

        public ICommand ApplyDeleteMethodCommand => new DelegateCommand(() =>
        {
            _versionUpdater.UpdateDelete(_classInfo, _methodInfo);
            _updateAction.Invoke(_classInfo);
        });

        public string Content => _methodInfo.ToString();

        public ICommand DeleteAddedMethodCommand => new DelegateCommand(() =>
        {
            _versionUpdater.DeleteAddedSuggestion(_classInfo, _methodInfo);
            _updateAction.Invoke(_classInfo);
        });

        public ICommand DeleteDeletedMethodCommand => new DelegateCommand(() =>
        {
            _versionUpdater.DeleteDeletedSuggestion(_classInfo, _methodInfo);
            _updateAction.Invoke(_classInfo);
        });
    }
}
