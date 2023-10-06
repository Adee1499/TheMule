using ReactiveUI;
using System.Collections.ObjectModel;
using TheMule.Models.Printify;
using TheMule.Services;

namespace TheMule.ViewModels.Shopify
{
    public class NewProductWindowViewModel : ViewModelBase
    {
        private readonly ServiceMediator _mediator;
        private string? _inputTitle;
        public string? InputTitle {
            get => _inputTitle;
            set => this.RaiseAndSetIfChanged(ref _inputTitle, value);
        }
        public ObservableCollection<Printify.ProductViewModel> UniquePrintifyProducts => _mediator.UniquePrintifyProducts;

        private bool _isBusy;
        public bool IsBusy {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public NewProductWindowViewModel(ServiceMediator mediator) {
            _mediator = mediator;

            if (mediator.UniquePrintifyProducts.Count <= 0)
                FetchPrintifyProducts();
        }

        private async void FetchPrintifyProducts() {
            IsBusy = true;

            _mediator.PrintifyProducts.Clear();

            var products = await Product.GetProductsAsync();

            foreach (var product in products) {
                var vm = new Printify.ProductViewModel(product);
                _mediator.PrintifyProducts.Add(vm);
            }

            IsBusy = false;
        }
    }
}
