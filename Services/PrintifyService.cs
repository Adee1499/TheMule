using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TheMule.Models.Printify;
using static TheMule.Models.Printify.Blueprint;

namespace TheMule.Services
{
    public static class PrintifyService
    {
        private static RestClient? _client;
        private static string? _shopId;

        private static void InitializeRestClient() {
            SettingsManager.LoadSettings();
            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(SettingsManager.AppSettings.PrintifyService.APIKey, "Bearer");

            var options = new RestClientOptions(SettingsManager.AppSettings.PrintifyService.BaseUrl) {
                Authenticator = authenticator
            };

            _client = new RestClient(options);

            _shopId = SettingsManager.AppSettings.PrintifyService.ShopId;
        }

        public static async Task<List<Artwork>> GetArtworksAsync() {
            if (_client == null) InitializeRestClient();

            List<Artwork> uploadsData = new();

            int currentPage = 1;

            var response = await _client!.GetJsonAsync<ArtworkResponse>($"uploads.json?page={currentPage}");

            if (response != null) {
                foreach (Artwork data in response.Data) {
                    uploadsData.Add(data);
                }
                while (response!.Total > uploadsData.Count) {
                    currentPage++;

                    response = await _client!.GetJsonAsync<ArtworkResponse>($"uploads.json?page={currentPage}");

                    if (response != null) {
                        foreach (Artwork data in response.Data) {
                            uploadsData.Add(data);
                        }
                    }
                }
            }
            return uploadsData;
        }

        public static async Task<bool> ArchiveArtworkAsync(string imageId) {
            if (_client == null) InitializeRestClient();

            var request = new RestRequest {
                Resource = $"uploads/{imageId}/archive.json"
            };

            var response = await _client!.PostAsync(request);

            Debug.WriteLine(response.StatusCode);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static async Task<bool> UploadArtworkAsync(Artwork newArtwork) {
            if (_client == null) InitializeRestClient();

            string jsonBody = JsonSerializer.Serialize(newArtwork);

            var request = new RestRequest {
                Resource = "uploads/images.json"
            };
            request.AddStringBody(jsonBody, ContentType.Json);

            var response = await _client!.PostAsync(request);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static async Task<List<Product>> GetProductsAsync() {
            if (_client == null) InitializeRestClient();

            List<Product> productsData = new();

            int currentPage = 1;

            var response = await _client!.GetJsonAsync<ProductResponse>($"shops/{_shopId}/products.json?page={currentPage}&limit=100");

            foreach (Product data in response.Data) {
                productsData.Add(data);
            }

            while (response.Total > productsData.Count) {
                currentPage++;

                response = await _client.GetJsonAsync<ProductResponse>($"shops/{_shopId}/products.json?page={currentPage}&limit=100");

                foreach (Product data in response.Data) {
                    productsData.Add(data);
                }
            }

            return productsData;
        }

        public static async Task<bool> CreateProductAsync(Product newProduct) {
            if (_client == null) InitializeRestClient();

            string jsonBody = JsonSerializer.Serialize(newProduct);

            var request = new RestRequest {
                Resource = $"shops/{_shopId}/products.json"
            };
            request.AddStringBody(jsonBody, ContentType.Json);

            var response = await _client!.PostAsync(request);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static async Task<List<Blueprint>> GetBlueprintsAsync() {
            if (_client == null) InitializeRestClient();

            List<Blueprint> blueprintsData = new();

            var response = await _client!.GetJsonAsync<Blueprint[]>("catalog/blueprints.json");

            if (response != null) {
                blueprintsData = response.ToList();                
            }

            return blueprintsData;
        }

        public static async Task<List<PrintProvider>> GetPrintProvidersAsync() {
            if (_client == null) InitializeRestClient();

            List<PrintProvider> printProvidersData = new();

            var response = await _client!.GetJsonAsync<PrintProvider[]>("catalog/print_providers.json");

            if (response != null) {
                printProvidersData = response.ToList();
            }

            return printProvidersData;
        }

        public static async Task<List<PrintProvider>> GetPrintProvidersForBlueprintAsync(int blueprintId) {
            if (_client == null) InitializeRestClient();

            List<PrintProvider> printProvidersData = new();

            var response = await _client!.GetJsonAsync<PrintProvider[]>($"catalog/blueprints/{blueprintId}/print_providers.json");

            if (response != null) {
                printProvidersData = response.ToList();
            }

            return printProvidersData;
        }

        public static async Task<List<BlueprintVariant>> GetVariantsAsync(int blueprintId, int printProviderId) {
            if (_client == null) InitializeRestClient();

            List<BlueprintVariant> variantsData = new();

            var response = await _client!.GetJsonAsync<BlueprintVariantsResponse>($"catalog/blueprints/{blueprintId}/print_providers/{printProviderId}/variants.json");

            if (response != null) {
                variantsData = response.Variants.ToList();
            }

            return variantsData;
        }
    }
}
