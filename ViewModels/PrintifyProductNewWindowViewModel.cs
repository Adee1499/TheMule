using ReactiveUI;
using System.Collections.ObjectModel;
using System.Threading;
using TheMule.Models.Printify;

namespace TheMule.ViewModels
{
    public class PrintifyProductNewWindowViewModel : ViewModelBase
    {
        private Blueprint? _selectedBlueprint;
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public Blueprint? SelectedBlueprint {
            get => _selectedBlueprint;
            set => this.RaiseAndSetIfChanged(ref _selectedBlueprint, value);
        }
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();

        private CancellationTokenSource? _cancellationTokenSource;
        
        public PrintifyProductNewWindowViewModel() {
            FetchBlueprints();
            FetchPrintProviders();
        }

        private async void FetchBlueprints() { 
            PrintifyBlueprints.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var blueprints = await Blueprint.GetBlueprintsAsync();

            foreach (Blueprint blueprint in blueprints) {
                PrintifyBlueprints.Add(blueprint);
            }

            if (!cancellationToken.IsCancellationRequested) {
                // LoadPreviewImages(cancellationToken);
            }
        }

        private async void FetchPrintProviders() {
            PrintProviders.Clear();

            var printProviders = await PrintProvider.GetPrintProvidersAsync();

            foreach (PrintProvider printProvider in printProviders) {
                PrintProviders.Add(printProvider);
            }
        }
    }
}
