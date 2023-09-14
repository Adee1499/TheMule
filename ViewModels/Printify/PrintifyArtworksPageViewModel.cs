using Avalonia.Platform.Storage;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using TheMule.Models.Printify;
using TheMule.Views.Printify;

namespace TheMule.ViewModels.Printify
{
    public class PrintifyArtworksPageViewModel : ViewModelBase
    {
        private PrintifyArtworkView? _selectedArtwork;
        public ObservableCollection<PrintifyArtworkViewModel> PrintifyArtworks { get; } = new();
        public PrintifyArtworkView? SelectedArtwork
        {
            get => _selectedArtwork;
            set => this.RaiseAndSetIfChanged(ref _selectedArtwork, value);
        }
        public ICommand OpenFileDialogCommand { get; }
        public Interaction<Unit, IStorageFile?> OpenFileDialog { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        private string _printifyArtworksCount;
        public string PrintifyArtworksCount
        {
            get => _printifyArtworksCount;
            set => this.RaiseAndSetIfChanged(ref _printifyArtworksCount, value);
        }

        private CancellationTokenSource? _cancellationTokenSource;

        public PrintifyArtworksPageViewModel()
        {
            PrintifyArtworksCount = $"Printify Artworks: {PrintifyArtworks.Count}";
            OpenFileDialog = new Interaction<Unit, IStorageFile?>();
            OpenFileDialogCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await OpenFileDialog.Handle(Unit.Default);
                if (file is not null)
                {
                    string filePath = file.TryGetLocalPath()!;
                    var newArtwork = await Artwork.UploadArtwork(filePath, Path.GetFileName(filePath));
                    FetchArtworks();
                }
            });
            FetchArtworks();
        }

        private async void FetchArtworks()
        {
            IsBusy = true;
            PrintifyArtworks.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var artworks = await Artwork.GetArtworksAsync();

            foreach (var artwork in artworks)
            {
                var vm = new PrintifyArtworkViewModel(artwork);
                PrintifyArtworks.Add(vm);
            }

            PrintifyArtworksCount = $"Printify Artworks: {PrintifyArtworks.Count}";

            if (!cancellationToken.IsCancellationRequested)
            {
                LoadPreviewImages(cancellationToken);
            }

            IsBusy = false;
        }

        private async void LoadPreviewImages(CancellationToken cancellationToken)
        {
            foreach (var artwork in PrintifyArtworks.ToList())
            {
                await artwork.LoadPreview();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
