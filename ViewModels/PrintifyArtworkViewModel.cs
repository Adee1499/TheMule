using Avalonia.Media.Imaging;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Threading.Tasks;
using TheMule.Models;

namespace TheMule.ViewModels
{
    public class PrintifyArtworkViewModel : ViewModelBase
    {
        private readonly PrintifyArtwork _printifyArtwork;
        private readonly ObservableAsPropertyHelper<bool> _artworkArchived;
        public ReactiveCommand<Unit, bool> ArchiveCommand { get; }
        public bool ArtworkArchived => _artworkArchived.Value;

        public PrintifyArtworkViewModel(PrintifyArtwork printifyArtwork) {
            _printifyArtwork = printifyArtwork;
            ArchiveCommand = ReactiveCommand.CreateFromTask(ArchiveArtworkAsync);
            _artworkArchived = ArchiveCommand.ToProperty(
                this, x => x.ArtworkArchived, scheduler: RxApp.MainThreadScheduler);
            ArchiveCommand.ThrownExceptions.Subscribe(exception => {
                this.Log().Warn("Error!", exception);
            });
        }

        public string FileName => _printifyArtwork.FileName;
        public string PreviewUrl => _printifyArtwork.PreviewUrl;

        private Bitmap? _previewImage;
        public Bitmap? PreviewImage {
            get => _previewImage;
            private set => this.RaiseAndSetIfChanged(ref _previewImage, value);
        }

        public async Task LoadPreview() {
            await using (var imageStream = await _printifyArtwork.LoadPreviewImageAsync()) {
                PreviewImage = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
            await SaveToDiskAsync();
        }

        private async Task SaveToDiskAsync() {
            if (PreviewImage != null) {
                var bitmap = PreviewImage;

                await Task.Run(() => {
                    using (var fs = _printifyArtwork.SavePreviewImageStream()) {
                        bitmap.Save(fs);
                    }
                });
            }
        }

        private async Task<bool> ArchiveArtworkAsync() {
            return await _printifyArtwork.ArchiveArtworkAsync();
        }
    }
}
