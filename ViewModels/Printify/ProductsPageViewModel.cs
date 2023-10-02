using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using TheMule.Models.Printify;
using TheMule.Services;

namespace TheMule.ViewModels.Printify
{
    public class ProductsPageViewModel : ViewModelBase
    {
        private readonly ServiceMediator _mediator;

        private ProductViewModel? _selectedProduct;
        public ObservableCollection<ProductViewModel> PrintifyProducts => _mediator.PrintifyProducts;
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
        private string _printifyProductsCount;
        public string PrintifyProductsCount
        {
            get => _printifyProductsCount;
            set => this.RaiseAndSetIfChanged(ref _printifyProductsCount, value);
        }

        private CancellationTokenSource? _cancellationTokenSource;

        public ProductsPageViewModel(ServiceMediator mediator)
        {
            _mediator = mediator;
            PrintifyProductsCount = $"Printify Products: {PrintifyProducts.Count}";
            ShowNewProductDialog = new Interaction<NewProductWindowViewModel, ProductViewModel?>();

            CreateNewProductCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var newProductDialog = new NewProductWindowViewModel(mediator);

                var result = await ShowNewProductDialog.Handle(newProductDialog);
            });

            FetchProducts();
        }

        private async void FetchProducts()
        {
            IsBusy = true;
            PrintifyProducts.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var products = await Product.GetProductsAsync();

            foreach (var product in products)
            {
                var vm = new ProductViewModel(product);
                PrintifyProducts.Add(vm);
            }

            PrintifyProductsCount = $"Printify Products: {PrintifyProducts.Count}";

            if (!cancellationToken.IsCancellationRequested)
            {
                LoadPreviewImages(cancellationToken);
            }

            IsBusy = false;
        }

        private async void LoadPreviewImages(CancellationToken cancellationToken)
        {
            foreach (var product in PrintifyProducts.ToList())
            {
                await product.LoadPreview();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
