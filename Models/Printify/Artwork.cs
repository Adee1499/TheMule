using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;
using TheMule.Shared;

namespace TheMule.Models.Printify
{
    public class Artwork
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

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

        [JsonPropertyName("upload_time"), JsonConverter(typeof(PrintifyDateTimeOffsetConverter))]
        public DateTimeOffset UploadTime { get; set; }

        public Artwork(string id, string filename, int height, int width, int size, string mimeType, string previewUrl, DateTimeOffset uploadTime)
        {
            Id = id;
            FileName = filename;
            Height = height;
            Width = width;
            Size = size;
            MimeType = mimeType;
            PreviewUrl = previewUrl;
            UploadTime = uploadTime;
        }

        public static async Task<IEnumerable<Artwork>> GetArtworksAsync()
        {
            return await PrintifyService.GetArtworksAsync();
        }

        private static HttpClient s_httpClient = new();
        private string CachePath => $"./Cache/{Id}";

        public async Task<Stream> LoadPreviewImageAsync()
        {
            if (File.Exists($"{CachePath}-{FileName}"))
            {
                return File.OpenRead($"{CachePath}-{FileName}");
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(PreviewUrl);
                return new MemoryStream(data);
            }
        }

        public Stream SavePreviewImageStream()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            return File.OpenWrite($"{CachePath}-{FileName}");
        }

        public async Task<bool> ArchiveArtworkAsync()
        {
            return await PrintifyService.ArchiveArtwork(Id);
        }
    }

    public class ArtworkResponse
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("data")]
        public Artwork[] Data { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
