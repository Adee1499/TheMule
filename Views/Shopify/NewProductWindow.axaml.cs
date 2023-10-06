using Avalonia.Controls;
using Avalonia.ReactiveUI;
using TheMule.Services;
using TheMule.ViewModels.Shopify;

namespace TheMule.Views.Shopify
{
    public partial class NewProductWindow : ReactiveWindow<NewProductWindowViewModel>
    {
        public NewProductWindow() 
        {
            if (Design.IsDesignMode)
                DataContext = new NewProductWindowViewModel(ServiceMediator.Instance);
            InitializeComponent();
        }
    }
}
