using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TheMule.Models.Printify;

namespace TheMule.Services
{
    public static class PrintifyService
    {
        private static RestClient? _client;
        private static string _shopId = "10241097";

        private static void InitializeRestClient() { 
            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6ImNiZmE4MGFmOWMyNTg1MTg5ZDI2ODAyOWUyZWQzYTE0NmMxOWJlMjM0OTY3YjMyNmJmZTQwN2Q0ZDdmOTk2YWRiMjcwZGJmNTc1MTRlNjI1IiwiaWF0IjoxNjg5NTMwOTUwLjQ0MDc2MywibmJmIjoxNjg5NTMwOTUwLjQ0MDc2NywiZXhwIjoxNzIxMTUzMzUwLjQzNjExNCwic3ViIjoiMTI1NTg2MTMiLCJzY29wZXMiOlsic2hvcHMubWFuYWdlIiwic2hvcHMucmVhZCIsImNhdGFsb2cucmVhZCIsIm9yZGVycy5yZWFkIiwib3JkZXJzLndyaXRlIiwicHJvZHVjdHMucmVhZCIsInByb2R1Y3RzLndyaXRlIiwid2ViaG9va3MucmVhZCIsIndlYmhvb2tzLndyaXRlIiwidXBsb2Fkcy5yZWFkIiwidXBsb2Fkcy53cml0ZSIsInByaW50X3Byb3ZpZGVycy5yZWFkIl19.AinHq-eyvfu_T6foongpRtOK5Xhshi8RFjR6CmGQhbAfpqRmcddFz7LqgehOARQGNobkCNscP749TCdWCx4", "Bearer");

            var options = new RestClientOptions("https://api.printify.com/v1/") {
                Authenticator = authenticator
            };

            _client = new RestClient(options);
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

        public static async Task<bool> ArchiveArtwork(string imageId) {
            if (_client == null) InitializeRestClient();

            var request = new RestRequest();
            request.Resource = $"uploads/{imageId}/archive.json";

            var response = await _client!.PostAsync(request);

            Debug.WriteLine(response.StatusCode);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        public static async Task<List<Product>> GetProductsAsync() {
            if (_client == null) InitializeRestClient();

            List<Product> productsData = new();

            int currentPage = 1;

            var response = await _client.GetJsonAsync<ProductResponse>($"shops/{_shopId}/products.json?page={currentPage}&limit=100");

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
    }
}
