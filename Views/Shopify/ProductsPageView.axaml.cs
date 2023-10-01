using Avalonia.Controls;
using Avalonia.ReactiveUI;
using TheMule.ViewModels.Shopify;

namespace TheMule.Views.Shopify
{
    public partial class ProductsPageView : ReactiveUserControl<ProductsPageViewModel>
    {
        public ProductsPageView() {
            DataContext = new ProductsPageViewModel();
            InitializeComponent();
        }
    }
}
