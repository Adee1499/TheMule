using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using TheMule.Models.Shopify;
using TheMule.Services;

namespace TheMule.ViewModels.Shopify
{
    public class ProductsPageViewModel : ViewModelBase
    {
        private readonly ServiceMediator _mediator;
        private ProductViewModel? _selectedProduct;
        public ObservableCollection<ProductViewModel> ShopifyProducts => _mediator.ShopifyProducts;
        public ProductViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
        }
        public Interaction<NewProductWindowViewModel, ProductViewModel?> ShowNewProductDialog { get; }
        public ICommand CreateNewProductCommand { get; }

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

        public ProductsPageViewModel(ServiceMediator mediator)
        {
            _mediator = mediator;
            ShopifyProductsCount = $"Shopify Products: {ShopifyProducts.Count}";
            ShowNewProductDialog = new Interaction<NewProductWindowViewModel, ProductViewModel?>();

            CreateNewProductCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var newProductDialog = new NewProductWindowViewModel(ServiceMediator.Instance);

                var result = await ShowNewProductDialog.Handle(newProductDialog);
            });

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
