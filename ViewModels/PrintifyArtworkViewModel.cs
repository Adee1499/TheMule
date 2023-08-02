using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Threading.Tasks;
using TheMule.Models;

namespace TheMule.ViewModels
{
    public class PrintifyArtworkViewModel : ViewModelBase
    {
        private readonly PrintifyArtwork _printifyArtwork;

        public PrintifyArtworkViewModel(PrintifyArtwork printifyArtwork) {
            _printifyArtwork = printifyArtwork;
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
        }
    }
}
