using Awiz.Core.Contract;
using Awiz.Core.Contract.CodeInfo;
using Prism.Commands;
using System;
using System.Windows.Input;

namespace Awiz.ViewModel.ClassDiagram
{
    public class PropertyInfoViewModel : ElementViewModel
    {
        private PropertyInfo _propertyInfo;

        public PropertyInfoViewModel(ClassInfo classInfo,
                                     PropertyInfo propertyInfo,
                                     Action<ClassInfo> updateAction,
                                     IVersionUpdater versionUpdater) :
            base(classInfo, updateAction, versionUpdater)
        {
            _classInfo = classInfo;
            _propertyInfo = propertyInfo;
            _updateAction = updateAction;
            _versionUpdater = versionUpdater;
        }

        public ICommand ApplyAddProperyCommand => new DelegateCommand(() =>
        {
            _versionUpdater.UpdateAdd(_classInfo, _propertyInfo);
            _updateAction.Invoke(_classInfo);
        });

        public ICommand ApplyDeletePropertyCommand => new DelegateCommand(() =>
        {
            _versionUpdater.UpdateDelete(_classInfo, _propertyInfo);
            _updateAction.Invoke(_classInfo);
        });

        public string Content => _propertyInfo.ToString();

        public ICommand DeleteAddedPropertyCommand => new DelegateCommand(() =>
        {
            _versionUpdater.DeleteAddedSuggestion(_classInfo, _propertyInfo);
            _updateAction.Invoke(_classInfo);
        });

        public ICommand DeleteDeletedPropertyCommand => new DelegateCommand(() =>
        {
            _versionUpdater.DeleteDeletedSuggestion(_classInfo, _propertyInfo);
            _updateAction.Invoke(_classInfo);
        });
    }
}
