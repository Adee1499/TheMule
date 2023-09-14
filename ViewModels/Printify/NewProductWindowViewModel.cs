using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TheMule.Models.Printify;
using TheMule.Services;

namespace TheMule.ViewModels.Printify
{
    public class NewProductWindowViewModel : ViewModelBase
    {
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

        public ObservableCollection<string> VariantsColours { get; } = new();
        public ObservableCollection<string> VariantsSizes { get; } = new();

        private CancellationTokenSource? _cancellationTokenSource;

        public NewProductWindowViewModel()
        {
            FetchBlueprints();
        }

        private async void FetchBlueprints()
        {
            PrintifyBlueprints.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var blueprints = await Blueprint.GetBlueprintsAsync();

            int[] setBlueprints = SettingsManager.appSettings.Printify.Blueprints.Keys.ToArray();

            foreach (Blueprint blueprint in blueprints)
            {
                if (setBlueprints.Contains(blueprint.Id))
                {
                    PrintifyBlueprints.Add(blueprint);
                }
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                // LoadPreviewImages(cancellationToken);
            }
        }

        private async void FetchPrintProviders(int[] printProvidersIds)
        {
            PrintProviders.Clear();

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

            List<Blueprint.BlueprintVariant> availableVariants = new();

            // Check if there's at least one list of variants
            if (variantsList.Count > 0)
            {
                // Initialize availableVariants with the first list as a starting point
                availableVariants.AddRange(variantsList.First());

                // Iterate through the rest of the lists and find common variants
                foreach (var variants in variantsList.Skip(1))
                {
                    // Use LINQ's Intersect to find common variants by Id
                    availableVariants = availableVariants
                        .Join(variants, av => av.Id, v => v.Id, (av, v) => av)
                        .ToList();
                }
            }

            List<string> colours = availableVariants
                .Select(variant => variant.Options.Colour)
                .Distinct()
                .ToList();

            foreach (string colour in colours)
            {
                VariantsColours.Add(colour);
            }

            List<string> sizes = availableVariants
                .Select(variant => variant.Options.Size)
                .Distinct()
                .ToList();

            foreach (string size in sizes)
            {
                VariantsSizes.Add(size);
            }
        }
    }
}
