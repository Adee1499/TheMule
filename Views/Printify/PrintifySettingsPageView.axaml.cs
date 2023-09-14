using Avalonia.Controls;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class PrintifySettingsPageView : UserControl
    {
        public PrintifySettingsPageView() {
            DataContext = new PrintifySettingsPageViewModel();
            InitializeComponent();
        }
    }
}
