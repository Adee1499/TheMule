using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TheMule.Models.Printify;
using Printify = TheMule.ViewModels.Printify;
using Shopify = TheMule.ViewModels.Shopify;

namespace TheMule.Services
{
    public class ServiceMediator
    {
        private static ServiceMediator? _instance;

        public static ServiceMediator Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new ServiceMediator();
                }
                _instance.PrintifyProducts.CollectionChanged += _instance.HandlePrintifyProductsUpdate;
                return _instance;
            }
        }

        public ObservableCollection<Printify.ArtworkViewModel> PrintifyArtworks { get; } = new();
        public ObservableCollection<Printify.ProductViewModel> PrintifyProducts { get; } = new();
        public ObservableCollection<Printify.ProductViewModel> UniquePrintifyProducts { get; } = new();
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();
        public ObservableCollection<Shopify.ProductViewModel> ShopifyProducts { get; } = new();

        private void HandlePrintifyProductsUpdate(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (Printify.ProductViewModel product in PrintifyProducts) {
                        if (UniquePrintifyProducts.Count(x => x.ProductName.Equals(product.ProductName)) <= 0) {
                            UniquePrintifyProducts.Add(product);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    UniquePrintifyProducts.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}