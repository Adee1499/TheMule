using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using TheMule.ViewModels;

namespace TheMule.Views
{
    public partial class PrintifyArtworksPageView : ReactiveUserControl<PrintifyArtworksPageViewModel>
    {
        public PrintifyArtworksPageView() {
            DataContext = new PrintifyArtworksPageViewModel();
            InitializeComponent();
            this.WhenActivated(action => action(ViewModel!.OpenFileDialog.RegisterHandler(OpenFileDialogAsync!)));
        }

        private async Task OpenFileDialogAsync(InteractionContext<Unit, IStorageFile> interaction) {
            var topLevel = TopLevel.GetTopLevel(this);

            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
                Title = "Open Artwork File",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] {
                    FilePickerFileTypes.ImageAll
                }
            });

            if (files.Count >= 1) {
                interaction.SetOutput(files[0]);
            } else {
                interaction.SetOutput(null!);
            }
        }
    }
}
