using Microsoft.UI.Xaml;
using Awiz.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Awiz.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var viewModel = new MainWindowViewModel();

            _mainGrid.DataContext = viewModel;

            // Populate the menu dynamically
            foreach (var menuItem in viewModel.MenuItems)
            {
                _menuBar.Items.Add(menuItem);
            }          
        }
    }
}
