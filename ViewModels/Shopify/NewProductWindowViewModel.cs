using Avalonia.Input;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
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
        private string? _inputDescription;
        public string? InputDescription {
            get => _inputDescription;
            set => this.RaiseAndSetIfChanged(ref _inputDescription, value);
        }
        private string? _inputPrice;
        public string? InputPrice {
            get => _inputPrice;
            set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    // Remove non-numeric characters and allow a single decimal point
                    string transformedValue = new string(value.Where(c => char.IsDigit(c) || c == '.').ToArray());

                    // Ensure only one decimal point exists
                    int countDecimalPoints = transformedValue.Count(c => c == '.');
                    if (countDecimalPoints > 1) {
                        transformedValue = transformedValue.Remove(transformedValue.LastIndexOf('.'), 1);
                    }
                    this.RaiseAndSetIfChanged(ref _inputPrice, "£" + transformedValue);
                } else {
                    this.RaiseAndSetIfChanged(ref _inputPrice, value);
                }
            }
        }
        public ObservableCollection<Printify.ProductViewModel> UniquePrintifyProducts => _mediator.UniquePrintifyProducts;

        private Printify.ProductViewModel? _selectedPrintifyProduct;
        public Printify.ProductViewModel? SelectedPrintifyProduct {
            get => _selectedPrintifyProduct;
            set {
                this.RaiseAndSetIfChanged(ref _selectedPrintifyProduct, value);
                OptionsColours.Clear();
                OptionsSizes.Clear();
                foreach (Product.ProductVariant variant in _selectedPrintifyProduct.Variants) {
                    if (variant.IsEnabled) {
                        int optionId1 = variant.Options[0];
                        int optionId2 = variant.Options[1];
                        foreach (Product.ProductOption option in _selectedPrintifyProduct.Options) {
                            if (option.Type.Equals("color")) {
                                string? colour = option.Values.FirstOrDefault(o => o.Id.Equals(optionId1))?.Title;
                                if (colour != null)
                                    if (!OptionsColours.Contains(colour))
                                        OptionsColours.Add(colour);

                            } else if (option.Type.Equals("size")) {
                                string? size = option.Values.FirstOrDefault(o => o.Id.Equals(optionId2))?.Title;
                                if (size != null)
                                    if (!OptionsSizes.Contains(size))
                                        OptionsSizes.Add(size);
                            }
                        }
                    }
                }
            }
        }

        public ObservableCollection<string> OptionsColours { get; } = new();
        public ObservableCollection<string> OptionsSizes { get; } = new();
        public string[] ProductStatusOptions { get; } = new[] { "Active", "Archived", "Draft" };

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
