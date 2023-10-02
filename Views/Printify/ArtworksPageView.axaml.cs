using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using TheMule.Services;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class ArtworksPageView : ReactiveUserControl<ArtworksPageViewModel>
    {
        public ArtworksPageView() {
            DataContext = new ArtworksPageViewModel(ServiceMediator.Instance);
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
