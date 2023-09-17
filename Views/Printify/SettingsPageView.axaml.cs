using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;
using TheMule.ViewModels.Printify;

namespace TheMule.Views.Printify
{
    public partial class SettingsPageView : UserControl
    {
        public SettingsPageView() {
            DataContext = new SettingsPageViewModel();
            InitializeComponent();
        }
    }
}
