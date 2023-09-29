using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheMule.Services
{
    public static class ShopifyService
    {
        private static RestClient? _client;
        private static string? _apiVersion;
        
        private static void InitializeRestClient() 
        {
            SettingsManager.LoadSettings();

            _client = new RestClient(SettingsManager.AppSettings.ShopifyService.BaseUrl);
            _apiVersion = SettingsManager.AppSettings.ShopifyService.APIVersion;
        }

        public static async Task<List<string>> GetProductsAsync()
        {
            if (_client == null) InitializeRestClient();

            var response = await _client!.GetJsonAsync<string[]>($"admin/api/{_apiVersion}/products.json");

            return response!.ToList();
        }
    }
}
