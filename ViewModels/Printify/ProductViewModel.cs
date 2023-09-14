using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Threading.Tasks;
using TheMule.Models.Printify;

namespace TheMule.ViewModels.Printify
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly Product _printifyProduct;

        public ProductViewModel(Product printifyProduct)
        {
            _printifyProduct = printifyProduct;
        }

        public string ProductName
        {
            get
            {
                string title = _printifyProduct.Title;
                if (title.Contains(")"))
                {
                    title = title.Split(')')[1];
                }
                if (title.Contains('-'))
                {
                    title = title.Split('-')[0];
                }
                return title;
            }
        }

        public string ProductNameFull => _printifyProduct.Title;
        public string Tags => string.Join(", ", _printifyProduct.Tags!);
        public string CreatedAt => _printifyProduct.CreatedAt.ToLocalTime().ToString();
        public string UpdatedAt => _printifyProduct.UpdatedAt.ToLocalTime().ToString();

        public string BlueprintDetails => $"{_printifyProduct.PrintProviderId} • {_printifyProduct.BlueprintId}";

        private Bitmap? _previewImage;
        public Bitmap? PreviewImage
        {
            get => _previewImage;
            private set => this.RaiseAndSetIfChanged(ref _previewImage, value);
        }

        public async Task LoadPreview()
        {
            await using (var imageStream = await _printifyProduct.LoadPreviewImageAsync())
            {
                PreviewImage = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
            await SaveToDiskAsync();
        }

        private async Task SaveToDiskAsync()
        {
            if (PreviewImage != null)
            {
                var bitmap = PreviewImage;

                await Task.Run(() =>
                {
                    using (var fs = _printifyProduct.SavePreviewImageStream())
                    {
                        bitmap.Save(fs);
                    }
                });
            }
        }
    }
}
