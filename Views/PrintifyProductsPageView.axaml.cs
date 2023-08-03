using Avalonia.Controls;
using TheMule.ViewModels;

namespace TheMule.Views
{
    public partial class PrintifyProductsPageView : UserControl
    {
        public PrintifyProductsPageView() {
            DataContext = new PrintifyProductsPageViewModel();
            InitializeComponent();
        }
    }
}
