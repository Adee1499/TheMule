using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class PrintifyProductsPageView : ReactiveUserControl<PrintifyProductsPageViewModel>
    {
        public PrintifyProductsPageView() {
            DataContext = new PrintifyProductsPageViewModel();
            InitializeComponent();
            this.WhenActivated(action => action(ViewModel!.ShowNewProductDialog.RegisterHandler(DoShowNewProductDialog)));
        }

        private async Task DoShowNewProductDialog(InteractionContext<PrintifyNewProductWindowViewModel, PrintifyProductViewModel?> interaction) {
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

            var dialog = new PrintifyProductNewWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<PrintifyProductViewModel?>(mainWindow!);
            interaction.SetOutput(result);
        }
    }
}
