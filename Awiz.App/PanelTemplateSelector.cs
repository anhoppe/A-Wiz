using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Awiz
{
    public class PanelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ClassPanelTemplate { get; set; }

        public DataTemplate? UseCasePanelTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            return item switch
            {
                ClassPanelViewModel => ClassPanelTemplate,
                UseCasePanelViewModel => UseCasePanelTemplate,
                _ => null
            };
        }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject containe)
        {
            return SelectTemplateCore(item);
        }
    }
}
