using Microsoft.UI.Xaml;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awiz
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
