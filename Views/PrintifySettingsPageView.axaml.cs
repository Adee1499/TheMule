using Avalonia.Controls;
using TheMule.ViewModels;

namespace TheMule.Views
{
    public partial class PrintifySettingsPageView : UserControl
    {
        public PrintifySettingsPageView() {
            DataContext = new PrintifySettingsPageViewModel();
            InitializeComponent();
        }
    }
}
