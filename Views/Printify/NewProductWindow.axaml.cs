using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class NewProductWindow : ReactiveWindow<NewProductWindowViewModel>
    {
        public NewProductWindow() {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CreateProductCommand.Subscribe(Close)));
        }
    }
}
