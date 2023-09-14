using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TheMule.Models.Printify;
using TheMule.Services;
using static TheMule.Services.AppSettings;

namespace TheMule.ViewModels.Printify
{
    public class SettingsPageViewModel : ViewModelBase
    {
        // Blueprint settings
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();
        private Blueprint? _selectedBlueprint;
        public Blueprint? SelectedBlueprint
        {
            get => _selectedBlueprint;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedBlueprint, value);
                // Grab settings for the selected blueprint
                SettingsManager.LoadSettings();
                if (SettingsManager.appSettings.Printify.Blueprints.ContainsKey(_selectedBlueprint.Id))
                {
                    BlueprintSettings blueprintSettings = SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint.Id];
                    UKBlueprintSettings = blueprintSettings.UK;
                    UKPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(UKBlueprintSettings!.PrintProviderId)).First();
                    EUBlueprintSettings = blueprintSettings.EU;
                    EUPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(EUBlueprintSettings!.PrintProviderId)).First();
                    USBlueprintSettings = blueprintSettings.US;
                    USPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(USBlueprintSettings!.PrintProviderId)).First();
                    AUBlueprintSettings = blueprintSettings.AU;
                    AUPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(AUBlueprintSettings!.PrintProviderId)).First();
                }
                else
                {
                    BlueprintSettings blueprintSettings = new BlueprintSettings();
                    SettingsManager.appSettings.Printify.Blueprints.Add(_selectedBlueprint.Id, blueprintSettings);
                    UKBlueprintSettings = blueprintSettings.UK;
                    UKPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(0)).First();
                    EUBlueprintSettings = blueprintSettings.EU;
                    EUPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(0)).First();
                    USBlueprintSettings = blueprintSettings.US;
                    USPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(0)).First();
                    AUBlueprintSettings = blueprintSettings.AU;
                    AUPrintProvider = PrintProviders.Where(pp => pp.Id.Equals(0)).First();
                }
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
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].UK = _ukBlueprintSettings!;
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
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].EU = _euBlueprintSettings!;
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
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].US = _usBlueprintSettings!;
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
                SettingsManager.appSettings.Printify.Blueprints[_selectedBlueprint!.Id].AU = _auBlueprintSettings!;
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

        public SettingsPageViewModel()
        {
            FetchBlueprints();
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

        private async void FetchPrintProviders()
        {
            PrintProviders.Clear();

            // Add a blank entry
            PrintProviders.Add(new PrintProvider(
                0,
                "No provider selected",
                new PrintProviderLocation()
            ));

            var printProviders = await PrintProvider.GetPrintProvidersAsync();

            foreach (PrintProvider printProvider in printProviders)
            {
                PrintProviders.Add(printProvider);
            }
        }
    }
}
