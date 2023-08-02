using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheMule.Models;

namespace TheMule.Services
{
    public static class PrintifyService
    {
        private static RestClient? _client;

        private static void InitializeRestClient() { 
            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6ImNiZmE4MGFmOWMyNTg1MTg5ZDI2ODAyOWUyZWQzYTE0NmMxOWJlMjM0OTY3YjMyNmJmZTQwN2Q0ZDdmOTk2YWRiMjcwZGJmNTc1MTRlNjI1IiwiaWF0IjoxNjg5NTMwOTUwLjQ0MDc2MywibmJmIjoxNjg5NTMwOTUwLjQ0MDc2NywiZXhwIjoxNzIxMTUzMzUwLjQzNjExNCwic3ViIjoiMTI1NTg2MTMiLCJzY29wZXMiOlsic2hvcHMubWFuYWdlIiwic2hvcHMucmVhZCIsImNhdGFsb2cucmVhZCIsIm9yZGVycy5yZWFkIiwib3JkZXJzLndyaXRlIiwicHJvZHVjdHMucmVhZCIsInByb2R1Y3RzLndyaXRlIiwid2ViaG9va3MucmVhZCIsIndlYmhvb2tzLndyaXRlIiwidXBsb2Fkcy5yZWFkIiwidXBsb2Fkcy53cml0ZSIsInByaW50X3Byb3ZpZGVycy5yZWFkIl19.AinHq-eyvfu_T6foongpRtOK5Xhshi8RFjR6CmGQhbAfpqRmcddFz7LqgehOARQGNobkCNscP749TCdWCx4", "Bearer");

            var options = new RestClientOptions("https://api.printify.com/v1/") {
                Authenticator = authenticator
            };

            _client = new RestClient(options);
        }

        public static async Task<List<PrintifyArtwork>> GetArtworks() {
            if (_client == null) InitializeRestClient();

            List<PrintifyArtwork> uploadsData = new();

            int currentPage = 1;

            var response = await _client!.GetJsonAsync<PrintifyArtworkResponse>($"uploads.json?page={currentPage}");

            if (response != null) {
                foreach (PrintifyArtwork data in response.Data) {
                    uploadsData.Add(data);
                }

                while (response!.Total > uploadsData.Count) {
                    currentPage++;

                    response = await _client!.GetJsonAsync<PrintifyArtworkResponse>($"uploads.json?page={currentPage}");

                    if (response != null) {
                        foreach (PrintifyArtwork data in response.Data) {
                            uploadsData.Add(data);
                        }
                    }
                }
            }


            return uploadsData;
        }
    }
}
