using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Linq;
using System.Threading.Tasks;
using TheMule.Models.Shopify;

namespace TheMule.ViewModels.Shopify
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly Product _shopifyProduct;

        public ProductViewModel(Product shopifyProduct)
        {
            _shopifyProduct = shopifyProduct;
        }

        public string ProductName 
        {
            get
            {
                if (_shopifyProduct.Title.Length > 42) {
                    return _shopifyProduct.Title.Substring(0, 42) + "...";
                } else {
                    return _shopifyProduct.Title;
                }

            }
        }
        public string? ProductType => _shopifyProduct.ProductType;
        public string? Variants
        {
            get
            {
                var colours = _shopifyProduct.Options.FirstOrDefault(o => o.Name.Equals("Colors") || o.Name.Equals("Colours") || o.Name.Equals("Color") || o.Name.Equals("Colour"))?.Values.Count();
                var sizes = _shopifyProduct.Options.FirstOrDefault(o => o.Name.Equals("Sizes") || o.Name.Equals("Size"))?.Values.Count();

                string variants = string.Empty;

                if (sizes == null && colours == null || colours == null) {
                    variants = "Total 1 variant";
                } else if (sizes == null) {
                    variants = $"{colours} colors  •  Total {colours} variants";
                } else {
                    variants = $"{colours} colors  •  {sizes} sizes  •  Total {colours * sizes} variants";
                }

                return variants;
            }
        }

        public string? StatusForeground
        {
            get
            {
                switch (_shopifyProduct.Status)
                {
                    case "active":
                        return "#08543D";
                    case "archived":
                        return "#6C7A87";
                    case "draft":
                        return "#185A85";
                    default: return null;
                }
            }
        }

        public string? StatusBackground
        {
            get
            {
                switch (_shopifyProduct.Status)
                {
                    case "active":
                        return "#47FCC6";
                    case "archived":
                        return "#F0F0F0";
                    case "draft":
                        return "#E0EFFF";
                    default: return null;
                }
            }
        }

        public string? StatusText
        {
            get
            {
                switch (_shopifyProduct.Status)
                {
                    case "active":
                        return "Active";
                    case "archived":
                        return "Archived";
                    case "draft":
                        return "Draft";
                    default: return null;
                }
            }
        }

        public string? Price
        {
            get
            {
                return (bool)_shopifyProduct.Variants?[0].Price.HasValue
                    ? "£" + _shopifyProduct.Variants?[0].Price.Value.ToString("0.00")
                    : "N/A";
            }
        } 

        private Bitmap? _image;
        public Bitmap? Image
        {
            get => _image;
            private set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public async Task LoadPreview()
        {
            await using (var imageStream = await _shopifyProduct.LoadPreviewImageAsync())
            {
                if (imageStream != null) {
                    Image = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
                }
            }
            await SaveToDiskAsync();
        }

        private async Task SaveToDiskAsync()
        {
            if (Image != null)
            {
                var bitmap = Image;

                await Task.Run(() =>
                {
                    using (var fs = _shopifyProduct.SavePreviewImageStream())
                    {
                        bitmap.Save(fs);
                    }
                });
            }
        }
    }
}
