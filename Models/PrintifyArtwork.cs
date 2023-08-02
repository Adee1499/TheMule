using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;

namespace TheMule.Models
{
    public class PrintifyArtwork
    {
        [JsonPropertyName("id")]
        public string Id {  get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("upload_time")]
        public string UploadTime { get; set; }

        public PrintifyArtwork(string id, string filename, int height, int width, int size, string mimeType, string previewUrl, string uploadTime) {
            Id = id; 
            FileName = filename; 
            Height = height; 
            Width = width; 
            Size = size; 
            MimeType = mimeType; 
            PreviewUrl = previewUrl; 
            UploadTime = uploadTime;
        }

        public static async Task<IEnumerable<PrintifyArtwork>> GetArtworksAsync() {
            return await PrintifyService.GetArtworks();
        }

        private static HttpClient s_httpClient = new();
        private string CachePath => $"./Cache/{Id}-{FileName}";

        public async Task<Stream> LoadPreviewImageAsync() {
            if (File.Exists(CachePath)) {
                return File.OpenRead(CachePath);
            } else {
                var data = await s_httpClient.GetByteArrayAsync(PreviewUrl);
                return new MemoryStream(data);
            }
        }
    }

    public class PrintifyArtworkResponse 
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("data")]
        public PrintifyArtwork[] Data { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
