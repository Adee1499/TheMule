using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
using TheMule.Services;
using TheMule.ViewModels.Shopify;

namespace TheMule.Views.Shopify
{
    public partial class ProductsPageView : ReactiveUserControl<ProductsPageViewModel>
    {
        public ProductsPageView() {
            DataContext = new ProductsPageViewModel(ServiceMediator.Instance);
            InitializeComponent();
            this.WhenActivated(action => action(ViewModel!.ShowNewProductDialog.RegisterHandler(DoShowNewProductDialog)));
        }
        
        private async Task DoShowNewProductDialog(InteractionContext<NewProductWindowViewModel, ProductViewModel?> interaction) {
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

            var dialog = new NewProductWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<ProductViewModel?>(mainWindow!);
            interaction.SetOutput(result);
        }
    }
}
