using Avalonia.Controls;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class SettingsPageView : UserControl
    {
        public SettingsPageView() {
            DataContext = new SettingsPageViewModel();
            InitializeComponent();
        }
    }
}
