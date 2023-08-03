using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TheMule.Models.Printify;
using TheMule.Views;

namespace TheMule.ViewModels
{
    public class PrintifyProductsPageViewModel : ViewModelBase
    {
        private PrintifyProductView? _selectedProduct;
        public ObservableCollection<PrintifyProductViewModel> PrintifyProducts { get; } = new();
        public PrintifyProductView? SelectedProduct {
            get => _selectedProduct;
            set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
        }
        private bool _isBusy;
        public bool IsBusy {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        private CancellationTokenSource? _cancellationTokenSource;

        public PrintifyProductsPageViewModel() {
            FetchProducts();
        }

        private async void FetchProducts() {
            IsBusy = true;
            PrintifyProducts.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var products = await Product.GetProductsAsync();

            foreach (var product in products) {
                var vm = new PrintifyProductViewModel(product);
                PrintifyProducts.Add(vm);
            }

            if (!cancellationToken.IsCancellationRequested) {
                LoadPreviewImages(cancellationToken);
            }

            IsBusy = false;
        }

        private async void LoadPreviewImages(CancellationToken cancellationToken) {
            foreach (var product in PrintifyProducts.ToList()) {
                await product.LoadPreview();

                if (cancellationToken.IsCancellationRequested) {
                    return;
                }
            }
        }
    }
}
