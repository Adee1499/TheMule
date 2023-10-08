using Avalonia.Controls;
using Avalonia.Input;
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

        private void PriceTextBox_KeyDown(object? sender, KeyEventArgs e) 
        {
            // Allow numeric keys, backspace, and delete
            if (!IsNumericKey(e.Key) && e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Decimal && e.Key != Key.OemPeriod) {
                e.Handled = true;
            }
        }

        private bool IsNumericKey(Key key) 
        {
            return (key >= Key.NumPad0 && key <= Key.NumPad9) || key >= Key.D0 && key <= Key.D9;
        }
    }
}
