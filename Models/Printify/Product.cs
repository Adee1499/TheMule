using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;
using TheMule.Shared;

namespace TheMule.Models.Printify
{
    public class Product
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title"), JsonRequired]
        public string Title { get; set; }

        [JsonPropertyName("description"), JsonRequired]
        public string Description { get; set; }

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }

        [JsonPropertyName("options")]
        public ProductOption[] Options { get; set; }

        [JsonPropertyName("variants"), JsonRequired]
        public ProductVariant[] Variants { get; set; }

        [JsonPropertyName("images")]
        public ProductImage[] Images { get; set; }

        [JsonPropertyName("created_at"), JsonConverter(typeof(PrintifyDateTimeOffsetConverter))]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at"), JsonConverter(typeof(PrintifyDateTimeOffsetConverter))]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("blueprint_id"), JsonRequired]
        public int BlueprintId { get; set; }

        [JsonPropertyName("print_provider_id"), JsonRequired]
        public int PrintProviderId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("shop_id")]
        public int ShopId { get; set; }

        [JsonPropertyName("print_areas"), JsonRequired]
        public ProductPrintArea[] PrintAreas { get; set; }


        public Product(
            string id, string title, string description, string[] tags, ProductOption[] options, ProductVariant[] variants,
            ProductImage[] images, DateTimeOffset createdAt, DateTimeOffset updatedAt, bool visible, int blueprintId,
            int printProviderId, int userId, int shopId, ProductPrintArea[] printAreas) 
        {
            Id = id;
            Title = title;
            Description = description;
            Tags = tags;
            Options = options;
            Variants = variants;
            Images = images;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Visible = visible;
            BlueprintId = blueprintId;
            PrintProviderId = printProviderId;
            UserId = userId;
            ShopId = shopId;
            PrintAreas = printAreas;
        }

        public static async Task<IEnumerable<Product>> GetProductsAsync() 
        {
            return await PrintifyService.GetProductsAsync();
        }

        private static HttpClient s_httpClient = new();
        private string CachePath => $"{SettingsManager.CachePath}/{Id}";

        public async Task<Stream> LoadPreviewImageAsync() {
            if (File.Exists($"{CachePath}-{Title}.png")) {
                return File.OpenRead($"{CachePath}-{Title}.png");
            } else {
                var data = await s_httpClient.GetByteArrayAsync(Images[0].Url);
                return new MemoryStream(data);
            }
        }

        public Stream SavePreviewImageStream() {
            if (!Directory.Exists($"{SettingsManager.CachePath}")) {
                Directory.CreateDirectory($"{SettingsManager.CachePath}");
            }

            return File.OpenWrite($"{CachePath}-{Title}.png");
        }


        public class ProductOption
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("values")]
            public ProductOptionValue[] Values { get; set; }
        }

        public class ProductOptionValue
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }
        }

        public class ProductVariant 
        {
            [JsonPropertyName ("id")]
            public int Id { get; set; }

            [JsonPropertyName("sku")]
            public string SKU { get; set; }

            [JsonPropertyName("cost")]
            public int Cost { get; set; }

            [JsonPropertyName("price")]
            public int Price { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("grams")]
            public int Grams { get; set; }

            [JsonPropertyName("is_enabled")]
            public bool IsEnabled { get; set; }

            [JsonPropertyName("is_default")]
            public bool IsDefault { get; set; }

            [JsonPropertyName("is_available")]
            public bool IsAvailable { get; set; }

            [JsonPropertyName("options")]
            public int[] Options { get; set; }
        }

        public class ProductImage 
        {
            [JsonPropertyName("src")]
            public string Url { get; set; }

            [JsonPropertyName("variant_ids")]
            public int[] Variants { get; set; }

            [JsonPropertyName("position")]
            public string Position { get; set; }

            [JsonPropertyName("is_default")]
            public bool IsDefault { get; set; }
        }

        public class ProductPrintArea 
        {
            [JsonPropertyName("variant_ids"), JsonRequired]
            public int[] Variants { get; set; }

            [JsonPropertyName("placeholders")]
            public ProductPlaceholder[] Placeholders { get; set; }

            [JsonPropertyName("background")]
            public string Background { get; set; }
        }

        public class ProductPlaceholder 
        {
            [JsonPropertyName("position"), JsonRequired]
            public string Position { get; set; }

            [JsonPropertyName("images"), JsonRequired]
            public ProductPlaceholderImage[] Images { get; set; }
        }

        public class ProductPlaceholderImage 
        {
            [JsonPropertyName("id"), JsonRequired]
            public string Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("height")]
            public int Height { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }

            [JsonPropertyName("x"), JsonRequired]
            public float X { get; set; }

            [JsonPropertyName("y"), JsonRequired]
            public float Y { get; set; }

            [JsonPropertyName("scale"), JsonRequired]
            public float Scale { get; set; }

            [JsonPropertyName("angle"), JsonRequired]
            public int Angle { get; set; }
        }
    }

    public class ProductResponse
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("data")]
        public Product[] Data { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
