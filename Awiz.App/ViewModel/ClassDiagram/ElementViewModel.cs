using Awiz.Core.Contract.CodeInfo;
using Prism.Mvvm;
using System;

namespace Awiz.ViewModel.ClassDiagram
{
    public class ElementViewModel : BindableBase
    {
        protected ClassInfo _classInfo;

        protected Action<ClassInfo> _updateAction;

        protected IVersionUpdater _versionUpdater;

        public ElementViewModel(ClassInfo classInfo, Action<ClassInfo> updateAction, IVersionUpdater versionUpdater)
        {
            _classInfo = classInfo;
            _updateAction = updateAction;
            _versionUpdater = versionUpdater;
        }
    }
}
