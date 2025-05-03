using Microsoft.UI.Xaml;
using Prism.Mvvm;

namespace Awiz.ViewModel
{
    public class ClassPanelViewModel : BindableBase
    {
        private Visibility _visbility = Visibility.Collapsed;

        public Visibility Visibility
        {
            get => _visbility;
            set
            {
                SetProperty(ref _visbility, value);
            }
        }
    }
}
