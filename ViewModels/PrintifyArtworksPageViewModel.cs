using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TheMule.Models;
using TheMule.Views;

namespace TheMule.ViewModels
{
    public class PrintifyArtworksPageViewModel : ViewModelBase
    {
        private PrintifyArtworkView? _selectedArtwork;
        public ObservableCollection<PrintifyArtworkViewModel> PrintifyArtworks { get; } = new();
        public PrintifyArtworkView? SelectedArtwork {
            get => _selectedArtwork;
            set => this.RaiseAndSetIfChanged(ref _selectedArtwork, value);
        }
        private bool _isBusy;
        public bool IsBusy {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        private CancellationTokenSource? _cancellationTokenSource;

        public PrintifyArtworksPageViewModel() {
            FetchArtworks();
        }

        private async void FetchArtworks() {
            IsBusy = true;
            PrintifyArtworks.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var artworks = await PrintifyArtwork.GetArtworksAsync();

            foreach (var artwork in artworks) {
                var vm = new PrintifyArtworkViewModel(artwork);
                PrintifyArtworks.Add(vm);
            }

            if (!cancellationToken.IsCancellationRequested) {
                LoadPreviewImages(cancellationToken);
            }

            IsBusy = false;
        }

        private async void LoadPreviewImages(CancellationToken cancellationToken) {
            foreach (var artwork in PrintifyArtworks.ToList()) {
                await artwork.LoadPreview();

                if (cancellationToken.IsCancellationRequested) {
                    return;
                }
            }
        }
    }
}
