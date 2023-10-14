using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TheMule.Models.Shopify;

namespace TheMule.Services
{
    public static class ShopifyService
    {
        private static RestClient? _client;
        private static string? _apiVersion;
        
        private static void InitializeRestClient() 
        {
            SettingsManager.LoadSettings();
            _apiVersion = SettingsManager.AppSettings.ShopifyService.APIVersion;

            _client = new RestClient(SettingsManager.AppSettings.ShopifyService.BaseUrl);
            _client.AddDefaultHeader("X-Shopify-Access-Token", SettingsManager.AppSettings.ShopifyService.APIKey);
        }

        public static async Task<List<Product>> GetProductsAsync()
        {
            if (_client == null) InitializeRestClient();

            var response = await _client!.GetJsonAsync<ProductResponse>($"admin/api/{_apiVersion}/products.json");

            return response!.Products?.ToList() ?? new List<Product>();
        }

        public static async Task<bool> CreateProductAsync(Product newProduct)
        {
	        if (_client == null) InitializeRestClient();

	        ProductRequest newProductRequest = new ProductRequest(newProduct);

	        string jsonBody = JsonSerializer.Serialize(newProductRequest);

	        var request = new RestRequest
	        {
		        Resource = $"admin/api/{_apiVersion}/products.json"
	        };

	        request.AddStringBody(jsonBody, ContentType.Json);

	        var response = await _client!.PostAsync(request);

	        return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
