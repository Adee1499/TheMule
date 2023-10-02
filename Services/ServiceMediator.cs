using System.Collections.ObjectModel;
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
                return _instance;
            }
        }

        public ObservableCollection<Printify.ArtworkViewModel> PrintifyArtworks { get; } = new();
        public ObservableCollection<Printify.ProductViewModel> PrintifyProducts { get; } = new();
        public ObservableCollection<Blueprint> PrintifyBlueprints { get; } = new();
        public ObservableCollection<PrintProvider> PrintProviders { get; } = new();
        public ObservableCollection<Shopify.ProductViewModel> ShopifyProducts { get; } = new();
    }
}