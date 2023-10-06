using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using TheMule.Services;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class NewProductWindow : ReactiveWindow<NewProductWindowViewModel>
    {
        public NewProductWindow()
        {
            if (Design.IsDesignMode)
                DataContext = new NewProductWindowViewModel(ServiceMediator.Instance);
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CreateProductCommand.Subscribe(Close)));
        }
    }
}
