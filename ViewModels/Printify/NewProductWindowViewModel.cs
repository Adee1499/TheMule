using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using TheMule.Models.Printify;
using TheMule.Services;

namespace TheMule.ViewModels.Printify
{
    public class NewProductWindowViewModel : ViewModelBase
    {
        private string? _inputTitle;
        public string? InputTitle {
            get => _inputTitle;
            set => this.RaiseAndSetIfChanged(ref _inputTitle, value);
        }

        private Blueprint? _selectedBlueprint;
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public Blueprint? SelectedBlueprint
        {
            get => _selectedBlueprint;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBlueprint, value);
                // Get print providers for this blueprint from settings
                var bpSettings = SettingsManager.appSettings.Printify.Blueprints[SelectedBlueprint!.Id];
                int[] printProvidersIds = new int[4];
                printProvidersIds[0] = bpSettings.UK.PrintProviderId;
                printProvidersIds[1] = bpSettings.EU.PrintProviderId;
                printProvidersIds[2] = bpSettings.US.PrintProviderId;
                printProvidersIds[3] = bpSettings.AU.PrintProviderId;
                FetchPrintProviders(printProvidersIds);
            }
        }
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();


        private List<Blueprint.BlueprintVariant> _availableVariants = new();
        public ObservableCollection<VariantColour> VariantsColours { get; } = new();
        public ObservableCollection<VariantSize> VariantsSizes { get; } = new();

        private ArtworkViewModel? _selectedArtwork;
        public ObservableCollection<ArtworkViewModel> PrintifyArtworks { get; } = new();
        public ArtworkViewModel? SelectedArtwork {
            get => _selectedArtwork;
            set {
                this.RaiseAndSetIfChanged(ref _selectedArtwork, value);
                _selectedArtwork.LoadPreview();
            }
        }

        private Product[]? _newProducts;

        private bool _isBusy;
        public bool IsBusy {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public ReactiveCommand<Unit, Product[]?> CreateProductCommand { get; }

        public NewProductWindowViewModel() {
            CreateProductCommand = ReactiveCommand.Create(() => {
                CreateProductsAsync();
                return _newProducts;
            });

            FetchBlueprints();
            FetchArtworks();
        }

        private async void FetchBlueprints() { 
            IsBusy = true;

            PrintifyBlueprints.Clear();

            var blueprints = await Blueprint.GetBlueprintsAsync();

            int[] setBlueprints = SettingsManager.appSettings.Printify.Blueprints.Keys.ToArray();

            foreach (Blueprint blueprint in blueprints)
            {
                if (setBlueprints.Contains(blueprint.Id))
                {
                    PrintifyBlueprints.Add(blueprint);
                }
            }

            IsBusy = false;
        }

        private async void FetchArtworks() {
            IsBusy = true;

            PrintifyArtworks.Clear();

            var artworks = await Artwork.GetArtworksAsync();

            foreach (var artwork in artworks) {
                var vm = new ArtworkViewModel(artwork);
                PrintifyArtworks.Add(vm);
            }

            IsBusy = false;
        }

        private async void FetchPrintProviders(int[] printProvidersIds)
        {
            PrintProviders.Clear();
            _availableVariants.Clear();

            var printProviders = await PrintProvider.GetPrintProvidersAsync();

            List<IEnumerable<Blueprint.BlueprintVariant>> variantsList = new();

            foreach (PrintProvider printProvider in printProviders)
            {
                if (printProvidersIds.Contains(printProvider.Id))
                {
                    PrintProviders.Add(printProvider);
                    variantsList.Add(await Blueprint.BlueprintVariant.GetVariantsAsync(SelectedBlueprint!.Id, printProvider.Id));
                }
            }


            // Check if there's at least one list of variants
            if (variantsList.Count > 0)
            {
                // Initialize availableVariants with the first list as a starting point
                _availableVariants.AddRange(variantsList.First());

                // Iterate through the rest of the lists and find common variants
                foreach (var variants in variantsList.Skip(1))
                {
                    // Use LINQ's Intersect to find common variants by Id
                    _availableVariants = _availableVariants
                        .Join(variants, av => av.Options.Colour, v => v.Options.Colour, (av, v) => av)
                        .ToList();
                }
            }

            List<string> colours = _availableVariants
                .Select(variant => variant.Options.Colour)
                .Distinct()
                .ToList();

            foreach (string colour in colours) {
                VariantsColours.Add(new VariantColour { Colour = colour, Selected = false });
            }

            List<string> sizes = _availableVariants
                .Select(variant => variant.Options.Size)
                .Distinct()
                .ToList();

            foreach (string size in sizes) {
                VariantsSizes.Add(new VariantSize { Size = size, Selected = false });
            }
        }

        private async void CreateProductsAsync() {
            Debug.WriteLine("Creating products");

            // Create 4 products on Printify, one for each market
            List<Product.ProductVariant> variants = new();

            foreach (VariantColour colour in VariantsColours) {
                if (colour.Selected) {
                    foreach (VariantSize size in VariantsSizes) {
                        if (size.Selected) {
                            var variant = _availableVariants.FirstOrDefault(v => v.Options.Colour.Equals(colour.Colour) && v.Options.Size.Equals(size.Size));
                            if (variant != null) {
                                variants.Add(new Product.ProductVariant(variant.Id, variant.Options.Colour));
                            }
                        }
                    }
                }
            }

            await Product.CreateProductAsync(CreateProductObject(variants, 
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].UK, "UK"));
            /*
            await Product.CreateProductAsync(CreateProductObject(variants, 
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].EU, "EU"));
            await Product.CreateProductAsync(CreateProductObject(variants, 
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].US, "US"));
            await Product.CreateProductAsync(CreateProductObject(variants, 
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].AU, "AU"));
            */
        }

        private Product CreateProductObject(List<Product.ProductVariant> variants, AppSettings.BlueprintPrintProviderSettings blueprintSettings, string titlePrefix) {
            Product.ProductPlaceholderImage frontPlaceholderImage = new Product.ProductPlaceholderImage {
                Id = _selectedArtwork!.Id,
                X = blueprintSettings.Placeholders.Front.X,
                Y = blueprintSettings.Placeholders.Front.Y,
                Scale = blueprintSettings.Placeholders.Front.Scale,
                Angle = blueprintSettings.Placeholders.Front.Angle
            };

            Product.ProductPlaceholder frontPlaceholder = new Product.ProductPlaceholder {
                Position = "front",
                Images = new Product.ProductPlaceholderImage[] { frontPlaceholderImage }
            };

            Product.ProductPlaceholderImage blackNeckPlaceholderImage = new Product.ProductPlaceholderImage {
                Id = SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.BlackLogoArtworkId,
                X = blueprintSettings.Placeholders.Neck.X,
                Y = blueprintSettings.Placeholders.Neck.Y,
                Scale = blueprintSettings.Placeholders.Neck.Scale,
                Angle = blueprintSettings.Placeholders.Neck.Angle
            };

            Product.ProductPlaceholderImage whiteNeckPlaceholderImage = new Product.ProductPlaceholderImage {
                Id = SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.WhiteLogoArtworkId,
                X = blueprintSettings.Placeholders.Neck.X,
                Y = blueprintSettings.Placeholders.Neck.Y,
                Scale = blueprintSettings.Placeholders.Neck.Scale,
                Angle = blueprintSettings.Placeholders.Neck.Angle
            };

            Product.ProductPlaceholder blackNeckPlaceholder = new Product.ProductPlaceholder {
                Position = "neck",
                Images = new Product.ProductPlaceholderImage[] { blackNeckPlaceholderImage }
            };

            Product.ProductPlaceholder whiteNeckPlaceholder = new Product.ProductPlaceholder {
                Position = "neck",
                Images = new Product.ProductPlaceholderImage[] { whiteNeckPlaceholderImage }
            };

            Dictionary<string, bool> colourVariants = SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.BlackLogoColours;

            Product.ProductPrintArea blackLogoPrintArea = new Product.ProductPrintArea {
                Variants = variants.Where(v => colourVariants.ContainsKey(v.Title) && colourVariants[v.Title]).Select(v => v.Id).ToArray(),
                Placeholders = new Product.ProductPlaceholder[] { frontPlaceholder, blackNeckPlaceholder }
            };

            Product.ProductPrintArea whiteLogoPrintArea = new Product.ProductPrintArea {
                Variants = variants.Where(v => colourVariants.ContainsKey(v.Title) && !colourVariants[v.Title]).Select(v => v.Id).ToArray(),
                Placeholders = new Product.ProductPlaceholder[] { frontPlaceholder, whiteNeckPlaceholder }
            };

            return new Product($"{titlePrefix}_{_inputTitle!}", _selectedBlueprint!.Id, blueprintSettings.PrintProviderId, variants.ToArray(), new Product.ProductPrintArea[] { blackLogoPrintArea, whiteLogoPrintArea });
        }

        public class VariantColour {
            public bool Selected { get; set; }
            public string Colour { get; set; }
        }

        public class VariantSize {
            public bool Selected { get; set; }
            public string Size { get; set; }
        }
    }
}
