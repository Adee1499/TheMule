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
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("url")]
        public string ImageUploadUrl { get; set; }

        [JsonPropertyName("height"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Height { get; set; }

        [JsonPropertyName("width"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Width { get; set; }

        [JsonPropertyName("size"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Size { get; set; }

        [JsonPropertyName("mime_type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string MimeType { get; set; }

        [JsonPropertyName("preview_url"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("upload_time"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(PrintifyDateTimeOffsetConverter))]
        public DateTimeOffset UploadTime { get; set; }

        [JsonConstructor]
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

        public Artwork(string filename, string imageUrl) {
            FileName = filename;
            ImageUploadUrl = imageUrl;
        }

        public static async Task<IEnumerable<Artwork>> GetArtworksAsync()
        {
            return await PrintifyService.GetArtworksAsync();
        }

        private static HttpClient s_httpClient = new();
        private string CachePath => $"{SettingsManager.CachePath}/{Id}";

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
            if (!Directory.Exists($"{SettingsManager.CachePath}"))
            {
                Directory.CreateDirectory($"{SettingsManager.CachePath}");
            }

            return File.OpenWrite($"{CachePath}-{FileName}");
        }

        public async Task<bool> ArchiveArtworkAsync()
        {
            return await PrintifyService.ArchiveArtworkAsync(Id);
        }

        public static async Task<Artwork> UploadArtworkAsync(string filePath, string fileName)
        {
            string cloudflareResourceUrl = await CloudflareService.UploadFile(filePath, fileName);
            if (cloudflareResourceUrl.StartsWith("Error")) return null!;
            Artwork newArtwork = new Artwork(fileName, cloudflareResourceUrl);
            if (await PrintifyService.UploadArtworkAsync(newArtwork)) {
                await CloudflareService.DeleteFile(fileName);
                return newArtwork;
            } else  {
                return null!;
            }
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
