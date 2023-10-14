using System.IO;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace TheMule.ViewModels.Shopify;

public class NewProductImageViewModel : ViewModelBase
{
	private Bitmap? _image;

	public Bitmap? Image
	{
		get => _image;
		set => this.RaiseAndSetIfChanged(ref _image, value);
	}

	public NewProductImageViewModel(Stream imageData)
	{
		Image = new Bitmap(imageData);
	}
	
	public NewProductImageViewModel() {}
}
