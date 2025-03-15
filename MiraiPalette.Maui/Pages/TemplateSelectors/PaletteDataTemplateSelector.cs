using MiraiPalette.Maui.Models;

namespace MiraiPalette.Maui.Pages.TemplateSelectors
{
    public class PaletteDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? Selected { get; set; }
        public DataTemplate? Default { get; set; }
        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            return item is Palette { IsSelected: true } ? Selected : Default;
        }
    }
}
