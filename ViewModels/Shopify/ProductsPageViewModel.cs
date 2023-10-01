using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TheMule.Models.Shopify;

namespace TheMule.ViewModels.Shopify
{
    public class ProductsPageViewModel : ViewModelBase
    {
        private ProductViewModel? _selectedProduct;
        public ObservableCollection<ProductViewModel> ShopifyProducts { get; } = new();
        public ProductViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        private string? _shopifyProductsCount;
        public string? ShopifyProductsCount
        {
            get => _shopifyProductsCount;
            set => this.RaiseAndSetIfChanged(ref _shopifyProductsCount, value);
        }

        public ProductsPageViewModel()
        {
            ShopifyProductsCount = $"Shopify Products: {ShopifyProducts.Count}";

            FetchProducts();
        }

        private async void FetchProducts()
        {
            IsBusy = true;
            ShopifyProducts.Clear();

            var products = await Product.GetProductsAsync();

            foreach (var product in products)
            {
                var vm = new ProductViewModel(product);
                ShopifyProducts.Add(vm);
            }

            ShopifyProductsCount = $"Shopify Products: {ShopifyProducts.Count}";

            LoadPreviewImages();

            IsBusy = false;
        }

        private async void LoadPreviewImages()
        {
            foreach (var product in ShopifyProducts.ToList())
            {
                await product.LoadPreview();
            }
        }
    }
}
