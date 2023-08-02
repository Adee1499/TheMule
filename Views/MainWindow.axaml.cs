using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TheMule.Views
{
    public partial class MainWindow : Window
    {
        private TabControl _tabControl;
        
        private Button _printifyArtworksButton;
        private Button _printifyProductsButton;
        private Button _printifyOrdersButton;
        private Button _printifySettingsButton;
        private Button _shopifyProductsButton;
        private Button _shopifyOrdersButton;
        private Button _shopifySettingsButton;
        private List<Button> _buttons = new();

        private UserControl _printifyArtworksPage;
        private UserControl _printifyProductsPage;
        private UserControl _printifyOrdersPage;
        private UserControl _printifySettingsPage;
        private UserControl _shopifyProductsPage;
        private UserControl _shopifyOrdersPage;
        private UserControl _shopifySettingsPage;

        public MainWindow() {
            InitializeComponent();
            _tabControl = this.Get<TabControl>("tabControl_pages");
            
            _printifyArtworksButton = this.Get<Button>("btn_printifyArtworks");
            _printifyProductsButton = this.Get<Button>("btn_printifyProducts");
            _printifyOrdersButton = this.Get<Button>("btn_printifyOrders");
            _printifySettingsButton = this.Get<Button>("btn_printifySettings");
            _shopifyProductsButton = this.Get<Button>("btn_shopifyProducts");
            _shopifyOrdersButton = this.Get<Button>("btn_shopifyOrders");
            _shopifySettingsButton = this.Get<Button>("btn_shopifySettings");
            _buttons.Add(_printifyArtworksButton);
            _buttons.Add(_printifyProductsButton);
            _buttons.Add(_printifyOrdersButton);
            _buttons.Add(_printifySettingsButton);
            _buttons.Add(_shopifyProductsButton);
            _buttons.Add(_shopifyOrdersButton);
            _buttons.Add(_shopifySettingsButton);

            _printifyArtworksPage = this.Get<UserControl>("page_printifyArtworks");
            _printifyProductsPage = this.Get<UserControl>("page_printifyProducts");
            _printifyOrdersPage = this.Get<UserControl>("page_printifyOrders");
            _printifySettingsPage = this.Get<UserControl>("page_printifySettings");
            _shopifyProductsPage = this.Get<UserControl>("page_shopifyProducts");
            _shopifyOrdersPage = this.Get<UserControl>("page_shopifyOrders");
            _shopifySettingsPage = this.Get<UserControl>("page_shopifySettings");

            _printifyArtworksButton.Click += (o, e) => {
                _tabControl.SelectedItem = _printifyArtworksPage;
                ChangeActiveButton((Button)o!);
            };
            _printifyProductsButton.Click += (o, e) => {
                _tabControl.SelectedItem = _printifyProductsPage;
                ChangeActiveButton((Button)o!);
            };
            _printifyOrdersButton.Click += (o, e) => {
                _tabControl.SelectedItem = _printifyOrdersPage;
                ChangeActiveButton((Button)o!);
            };
            _printifySettingsButton.Click += (o, e) => {
                _tabControl.SelectedItem = _printifySettingsPage;
                ChangeActiveButton((Button)o!);
            };
            _shopifyProductsButton.Click += (o, e) => {
                _tabControl.SelectedItem = _shopifyProductsPage;
                ChangeActiveButton((Button)o!);
            };
            _shopifyOrdersButton.Click += (o, e) => {
                _tabControl.SelectedItem = _shopifyOrdersPage;
                ChangeActiveButton((Button)o!);
            };
            _shopifySettingsButton.Click += (o, e) => {
                _tabControl.SelectedItem = _shopifySettingsPage;
                ChangeActiveButton((Button)o!);
            };

            ChangeActiveButton(_printifyArtworksButton);
        }

        private void ChangeActiveButton(Button activeButton) {
            foreach (Button button in _buttons) {
                button.Background = new SolidColorBrush { Color = Color.FromArgb(0, 0, 0, 0) };
            }
            activeButton.Background = new SolidColorBrush { Color = Color.FromRgb(35, 41, 47) };
        }
    }
}