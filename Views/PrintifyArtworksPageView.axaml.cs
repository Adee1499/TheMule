using Avalonia.Controls;
using TheMule.ViewModels;

namespace TheMule.Views
{
    public partial class PrintifyArtworksPageView : UserControl
    {
        public PrintifyArtworksPageView() {
            DataContext = new PrintifyArtworksPageViewModel();
            InitializeComponent();
        }
    }
}
