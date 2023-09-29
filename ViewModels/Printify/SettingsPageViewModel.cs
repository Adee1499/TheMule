using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using TheMule.Models.Printify;
using TheMule.Services;
using static TheMule.Services.AppSettings;
using static TheMule.ViewModels.Printify.NewProductWindowViewModel;

namespace TheMule.ViewModels.Printify
{
    public class SettingsPageViewModel : ViewModelBase
    {
        // Blueprint settings
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();
        public ObservableCollection<PrintProvider> AvailablePrintProviders { get; } = new();

        private Blueprint? _selectedBlueprint;
        public Blueprint? SelectedBlueprint
        {
            get => _selectedBlueprint;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBlueprint, value);
                // Fetch print providers for the selected blueprint
                FetchPrintProvidersForBlueprint(_selectedBlueprint!.Id);
            }
        }

        private PrintProvider? _ukPrintProvider;
        private PrintProvider? _euPrintProvider;
        private PrintProvider? _usPrintProvider;
        private PrintProvider? _auPrintProvider;

        public PrintProvider? UKPrintProvider
        {
            get => _ukPrintProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref _ukPrintProvider, value);
                UKBlueprintSettings!.PrintProviderId = _ukPrintProvider!.Id;
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].UK = _ukBlueprintSettings!;
                SettingsManager.SaveSettings();
            }
        }
        public PrintProvider? EUPrintProvider
        {
            get => _euPrintProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref _euPrintProvider, value);
                EUBlueprintSettings!.PrintProviderId = _euPrintProvider!.Id;
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].EU = _euBlueprintSettings!;
                SettingsManager.SaveSettings();
            }
        }
        public PrintProvider? USPrintProvider
        {
            get => _usPrintProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref _usPrintProvider, value);
                USBlueprintSettings!.PrintProviderId = _usPrintProvider!.Id;
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].US = _usBlueprintSettings!;
                SettingsManager.SaveSettings();
            }
        }
        public PrintProvider? AUPrintProvider
        {
            get => _auPrintProvider;
            set
            {
                this.RaiseAndSetIfChanged(ref _auPrintProvider, value);
                AUBlueprintSettings!.PrintProviderId = _auPrintProvider!.Id;
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].AU = _auBlueprintSettings!;
                SettingsManager.SaveSettings();
            }
        }

        private BlueprintPrintProviderSettings? _ukBlueprintSettings;
        private BlueprintPrintProviderSettings? _euBlueprintSettings;
        private BlueprintPrintProviderSettings? _usBlueprintSettings;
        private BlueprintPrintProviderSettings? _auBlueprintSettings;


        public BlueprintPrintProviderSettings? UKBlueprintSettings
        {
            get => _ukBlueprintSettings;
            set
            {
                this.RaiseAndSetIfChanged(ref _ukBlueprintSettings, value);
            }
        }
        public BlueprintPrintProviderSettings? EUBlueprintSettings
        {
            get => _euBlueprintSettings;
            set
            {
                this.RaiseAndSetIfChanged(ref _euBlueprintSettings, value);
            }
        }
        public BlueprintPrintProviderSettings? USBlueprintSettings
        {
            get => _usBlueprintSettings;
            set
            {
                this.RaiseAndSetIfChanged(ref _usBlueprintSettings, value);
            }
        }
        public BlueprintPrintProviderSettings? AUBlueprintSettings
        {
            get => _auBlueprintSettings;
            set
            {
                this.RaiseAndSetIfChanged(ref _auBlueprintSettings, value);
            }
        }

        public ObservableCollection<ArtworkViewModel> PrintifyArtworks { get; } = new();

        private ArtworkViewModel? _selectedWhiteLogo;
        private ArtworkViewModel? _selectedBlackLogo;
        public ArtworkViewModel? SelectedWhiteLogo {
            get => _selectedWhiteLogo;
            set {
                this.RaiseAndSetIfChanged(ref _selectedWhiteLogo, value);
                _selectedWhiteLogo.LoadPreview();
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.WhiteLogoArtworkId = _selectedWhiteLogo.Id;
                SettingsManager.SaveSettings();
            }
        }
        public ArtworkViewModel? SelectedBlackLogo {
            get => _selectedBlackLogo;
            set {
                this.RaiseAndSetIfChanged(ref _selectedBlackLogo, value);
                _selectedBlackLogo.LoadPreview();
                SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.BlackLogoArtworkId = _selectedBlackLogo.Id;
                SettingsManager.SaveSettings();
            }
        }

        private List<Blueprint.BlueprintVariant> _availableVariants = new();
        public ObservableCollection<ColourSelection> VariantsColoursBlackLogo { get; } = new();

        public SettingsPageViewModel()
        {
            FetchBlueprints();
            FetchArtworks();
            FetchPrintProviders();
        }

        private async void FetchBlueprints()
        {
            PrintifyBlueprints.Clear();

            var blueprints = await Blueprint.GetBlueprintsAsync();

            foreach (Blueprint blueprint in blueprints)
            {
                PrintifyBlueprints.Add(blueprint);
            }
        }

        private async void FetchArtworks() {
            PrintifyArtworks.Clear();

            var artworks = await Artwork.GetArtworksAsync();

            foreach (var artwork in artworks) {
                var vm = new ArtworkViewModel(artwork);
                PrintifyArtworks.Add(vm);
            }
        }

        private async void FetchPrintProviders()
        {
            PrintProviders.Clear();

            var printProviders = await PrintProvider.GetPrintProvidersAsync();

            foreach (PrintProvider printProvider in printProviders) {
                PrintProviders.Add(printProvider);
            }
        }

        private async void FetchPrintProvidersForBlueprint(int blueprintId) 
        {
            AvailablePrintProviders.Clear();

            // Add a blank entry
            AvailablePrintProviders.Add(new PrintProvider(
                0,
                "No provider selected",
                new PrintProviderLocation()
            ));

            var availablePrintProviders = await PrintProvider.GetPrintProvidersForBlueprintAsync(blueprintId);

            foreach (PrintProvider printProvider in availablePrintProviders) {
                var printProviderMatch = PrintProviders.FirstOrDefault(pp => pp.Id.Equals(printProvider.Id));
                if (printProviderMatch != null) {
                    AvailablePrintProviders.Add(printProviderMatch);
                }
            }

            // Grab settings for the selected blueprint
            SettingsManager.LoadSettings();
            if (SettingsManager.AppSettings.Printify.Blueprints.ContainsKey(blueprintId)) {
                BlueprintSettings blueprintSettings = SettingsManager.AppSettings.Printify.Blueprints[blueprintId];
                UKBlueprintSettings = blueprintSettings.UK;
                UKPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(UKBlueprintSettings!.PrintProviderId)).First();
                EUBlueprintSettings = blueprintSettings.EU;
                EUPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(EUBlueprintSettings!.PrintProviderId)).First();
                USBlueprintSettings = blueprintSettings.US;
                USPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(USBlueprintSettings!.PrintProviderId)).First();
                AUBlueprintSettings = blueprintSettings.AU;
                AUPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(AUBlueprintSettings!.PrintProviderId)).First();
                Dictionary<string, bool> colourVariants = blueprintSettings.LogoSettings.BlackLogoColours;
                FetchVariantsForPrintProviders(new int[] {
                    _ukPrintProvider.Id, _euPrintProvider.Id, _usPrintProvider.Id, _auPrintProvider.Id
                }, blueprintSettings.LogoSettings.BlackLogoColours);
                if (blueprintSettings.LogoSettings.WhiteLogoArtworkId != string.Empty)
                    SelectedWhiteLogo = PrintifyArtworks.First(a => a.Id.Equals(blueprintSettings.LogoSettings.WhiteLogoArtworkId));
                if (blueprintSettings.LogoSettings.BlackLogoArtworkId != string.Empty)
                    SelectedBlackLogo = PrintifyArtworks.First(a => a.Id.Equals(blueprintSettings.LogoSettings.BlackLogoArtworkId));
            } else {
                BlueprintSettings blueprintSettings = new BlueprintSettings();
                SettingsManager.AppSettings.Printify.Blueprints.Add(blueprintId, blueprintSettings);
                UKBlueprintSettings = blueprintSettings.UK;
                UKPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(0)).First();
                EUBlueprintSettings = blueprintSettings.EU;
                EUPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(0)).First();
                USBlueprintSettings = blueprintSettings.US;
                USPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(0)).First();
                AUBlueprintSettings = blueprintSettings.AU;
                AUPrintProvider = AvailablePrintProviders.Where(pp => pp.Id.Equals(0)).First();
            }
        }

        private async void FetchVariantsForPrintProviders(int[] printProvidersIds, Dictionary<string, bool> colourSettings) {
            _availableVariants.Clear();

            List<IEnumerable<Blueprint.BlueprintVariant>> variantsList = new();

            foreach (int printProviderId in printProvidersIds) {
                if (printProviderId > 0) {
                    variantsList.Add(await Blueprint.BlueprintVariant.GetVariantsAsync(SelectedBlueprint!.Id, printProviderId));
                }
            }


            // Check if there's at least one list of variants
            if (variantsList.Count > 0) {
                // Initialize availableVariants with the first list as a starting point
                _availableVariants.AddRange(variantsList.First());

                // Iterate through the rest of the lists and find common variants
                foreach (var variants in variantsList.Skip(1)) {
                    // Use LINQ's Intersect to find common variants by Id
                    _availableVariants = _availableVariants
                        .Join(variants, av => av.Id, v => v.Id, (av, v) => av)
                        .ToList();
                }
            }

            List<string> colours = _availableVariants
                .Select(variant => variant.Options.Colour)
                .Distinct()
                .ToList();

            VariantsColoursBlackLogo.Clear();

            foreach (string colour in colours) {
                VariantsColoursBlackLogo.Add(new ColourSelection(this, colour, colourSettings[colour]));
            }
        }

        public void SaveColourSelection() {
            Dictionary<string, bool> colours = new Dictionary<string, bool>();

            foreach (ColourSelection colourSelection in VariantsColoursBlackLogo) {
                colours.Add(colourSelection.Colour, colourSelection.Selected);
            }

            SettingsManager.AppSettings.Printify.Blueprints[_selectedBlueprint!.Id].LogoSettings.BlackLogoColours = colours;
            SettingsManager.SaveSettings();
        }

        public class ColourSelection : ReactiveObject {
            private bool _selected;
            private string _colour;
            private SettingsPageViewModel _parentViewModel;

            public ColourSelection(SettingsPageViewModel parentViewModel, string colour, bool selected) {
                _parentViewModel = parentViewModel;
                _colour = colour;
                _selected = selected;
            }

            public bool Selected {
                get => _selected;
                set {
                    this.RaiseAndSetIfChanged(ref _selected, value);
                    _parentViewModel.SaveColourSelection();
                }
            }

            public string Colour {
                get => _colour;
                set => this.RaiseAndSetIfChanged(ref _colour, value);
            }
        }
    }
}
