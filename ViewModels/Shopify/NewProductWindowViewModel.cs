using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive;
using TheMule.Models.Printify;
using TheMule.Services;

namespace TheMule.ViewModels.Shopify
{
	public class NewProductWindowViewModel : ViewModelBase
	{
		private readonly ServiceMediator _mediator;
		private string? _inputTitle;

		public string? InputTitle
		{
			get => _inputTitle;
			set => this.RaiseAndSetIfChanged(ref _inputTitle, value);
		}

		private string? _inputDescription;

		public string? InputDescription
		{
			get => _inputDescription;
			set => this.RaiseAndSetIfChanged(ref _inputDescription, value);
		}

		private string? _inputTags;

		public string? InputTags
		{
			get => _inputTags;
			set => this.RaiseAndSetIfChanged(ref _inputTags, value);
		}

		private string? _inputPrice;

		public string? InputPrice
		{
			get => _inputPrice;
			set
			{
				if (!string.IsNullOrWhiteSpace(value)) {
					// Remove non-numeric characters and allow a single decimal point
					string transformedValue = new string(value.Where(c => char.IsDigit(c) || c == '.').ToArray());

					// Ensure only one decimal point exists
					int countDecimalPoints = transformedValue.Count(c => c == '.');
					if (countDecimalPoints > 1) {
						transformedValue = transformedValue.Remove(transformedValue.LastIndexOf('.'), 1);
					}

					this.RaiseAndSetIfChanged(ref _inputPrice, transformedValue);
				} else {
					this.RaiseAndSetIfChanged(ref _inputPrice, value);
				}
			}
		}

		public ObservableCollection<Printify.ProductViewModel> UniquePrintifyProducts =>
			_mediator.UniquePrintifyProducts;

		private Printify.ProductViewModel? _selectedPrintifyProduct;

		public Printify.ProductViewModel? SelectedPrintifyProduct
		{
			get => _selectedPrintifyProduct;
			set
			{
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
				// Grab images
				FetchPrintifyImages();
			}
		}

		public ObservableCollection<string> OptionsColours { get; } = new();
		public ObservableCollection<string> OptionsSizes { get; } = new();
		public string[] ProductStatusOptions { get; } = new[] { "Active", "Archived", "Draft" };
		public string? SelectedProductStatus { get; set; }

		private Models.Shopify.Product? _newProduct;

		private bool _isBusy;

		public bool IsBusy
		{
			get => _isBusy;
			set => this.RaiseAndSetIfChanged(ref _isBusy, value);
		}

		public ReactiveCommand<Unit, Models.Shopify.Product?> CreateProductCommand { get; }

		public ObservableCollection<NewProductImageViewModel> NewProductImages { get; } = new();
		
		public NewProductWindowViewModel(ServiceMediator mediator)
		{
			_mediator = mediator;
			CreateProductCommand = ReactiveCommand.Create(() =>
			{
				CreateProductAsync();
				return _newProduct;
			});

			if (mediator.UniquePrintifyProducts.Count <= 0)
				FetchPrintifyProducts();
		}

		private async void FetchPrintifyProducts()
		{
			IsBusy = true;

			_mediator.PrintifyProducts.Clear();

			var products = await Product.GetProductsAsync();

			foreach (var product in products) {
				var vm = new Printify.ProductViewModel(product);
				_mediator.PrintifyProducts.Add(vm);
			}

			IsBusy = false;
		}

		private async void FetchPrintifyImages()
		{
			NewProductImages.Clear();
			NewProductImages.Add(new NewProductImageViewModel());
			foreach (var image in SelectedPrintifyProduct.Images) {
				Stream imageData = await image.LoadImageAsync();
				var vm = new NewProductImageViewModel(imageData);
				NewProductImages.Add(vm);
			}
		}

		private async void CreateProductAsync()
		{
			// Create variants
			List<Models.Shopify.Product.ProductVariant> productVariants = new();
			
			foreach (var colour in OptionsColours) {
				foreach (var size in OptionsSizes) {
					productVariants.Add(new Models.Shopify.Product.ProductVariant
					{
						Title = $"{colour} / {size}",
						Option1 = colour,
						Option2 = size,
						Price = float.Parse(InputPrice)
					});
				}
			}
			
			// Create options
			Models.Shopify.Product.ProductOption[] productOptions =
			{
				new()
				{
					Name = "Colour",
					Values = OptionsColours.ToArray()
				},
				new()
				{
					Name = "Size",
					Values = OptionsSizes.ToArray()
				}
			};
			
			// Get images
			List<Models.Shopify.Product.ProductImage> productImages = new();
			
			foreach (var image in SelectedPrintifyProduct.Images) {
				// Upload to Cloudflare R2
				string r2Url = await CloudflareService.UploadFromUrl(image.Url, image.Variants[0] + image.Position, "image/jpeg");
				
				// Create list of R2 Urls
				productImages.Add(new Models.Shopify.Product.ProductImage
				{
					Source = r2Url
				});
			}
			
			// Create a product
			Models.Shopify.Product newShopifyProduct = new Models.Shopify.Product
			{
				Title = InputTitle,
				BodyHtml = InputDescription,
				Tags = InputTags,
				Status = SelectedProductStatus?.ToLower(),
				ProductType = "T-Shirt",
				Vendor = "Aodach Avenue",
				Variants = productVariants.ToArray(),
				Options = productOptions,
				Images = productImages.ToArray()
			};

			await Models.Shopify.Product.CreateProductAsync(newShopifyProduct);
		}
	}
}
