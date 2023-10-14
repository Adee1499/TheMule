using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;

namespace TheMule.Models.Shopify
{
    public class Product
    {
        [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long Id { get; set; }

        [JsonPropertyName("title"), JsonRequired]
        public string? Title { get; set; }

        [JsonPropertyName("body_html"), JsonRequired]
        public string? BodyHtml { get; set; }

        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }

        [JsonPropertyName("product_type")]
        public string? ProductType { get; set; }

        [JsonPropertyName("created_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("handle"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Handle { get; set; }

        [JsonPropertyName("udpated_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("published_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? PublishedAt { get; set; }

        [JsonPropertyName("template_suffix"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? TemplateSuffix { get; set; }

        [JsonPropertyName("published_scope"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PublishedScope { get; set; }

        [JsonPropertyName("tags")]
        public string? Tags { get; set; }

        [JsonPropertyName("status"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Status { get; set; }

        [JsonPropertyName("variants"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProductVariant[]? Variants { get; set; }

        [JsonPropertyName("options"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProductOption[]? Options { get; set; }

        [JsonPropertyName("images"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProductImage[]? Images { get; set; }

        [JsonPropertyName("image"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProductImage? Image { get; set; }


        [JsonConstructor]
        public Product(
            long id, string title, string bodyHtml, string? vendor, string? productType, DateTime? createdAt, 
            string? handle, DateTime? updatedAt, DateTime? publishedAt, string? tags, string? status, ProductVariant[]? variants,
            ProductImage[]? images, ProductImage? image)
        {
            Id = id; 
            Title = title;
            BodyHtml = bodyHtml;
            Vendor = vendor;
            ProductType = productType;
            CreatedAt = createdAt;
            Handle = handle;
            UpdatedAt = updatedAt;
            PublishedAt = publishedAt;
            Tags = tags;
            Status = status;
            Variants = variants;
            Images = images;
            Image = image;
        }

        public Product() {}

        public static async Task<IEnumerable<Product>> GetProductsAsync() => await ShopifyService.GetProductsAsync();

        private static HttpClient s_httpClient = new();
        private string CachePath => $"{SettingsManager.CachePath}/{Id}";

        public async Task<Stream> LoadPreviewImageAsync()
        {
            if (File.Exists($"{CachePath}-{Title}.png")) {
                return File.OpenRead($"{CachePath}-{Title}.png");
            } else {
                if (Image?.Source != null) {
                    var data = await s_httpClient.GetByteArrayAsync(Image?.Source);
                    return new MemoryStream(data);
                }
                return null;
            }
        }

        public Stream SavePreviewImageStream()
        {
            if (!Directory.Exists($"{SettingsManager.CachePath}")) {
                Directory.CreateDirectory($"{SettingsManager.CachePath}");
            }

            return File.OpenWrite($"{CachePath}-{Title}.png");
        }

        public static async Task<bool> CreateProductAsync(Product newProduct)
        {
	        return await ShopifyService.CreateProductAsync(newProduct);
        }

        public class ProductVariant
        {
            [JsonPropertyName("product_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long ProductId { get; set; }

            [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("price")]
            public float? Price { get; set; }

            [JsonPropertyName("sku")]
            public string? SKU { get; set; }

            [JsonPropertyName("compare_at_price")]
            public float? CompareAtPrice { get; set; }

            [JsonPropertyName("fulfillment_service")]
            public string? FulfillmentService { get; set; }

            [JsonPropertyName("option1")]
            public string? Option1 { get; set; }

            [JsonPropertyName("option2")]
            public string? Option2 { get; set; }

            [JsonPropertyName("option3")]
            public string? Option3 { get; set; }

            [JsonPropertyName("created_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTime? CreatedAt { get; set; }

            [JsonPropertyName("updated_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTime? UpdatedAt { get; set; }

            [JsonPropertyName("taxable")]
            public bool? Taxable { get; set; }

            [JsonPropertyName("grams")]
            public int? Grams { get; set; }

            [JsonPropertyName("image_id")]
            public long? ImageId { get; set; }

            [JsonPropertyName("weight")]
            public float? Weight { get; set; }

            [JsonPropertyName("weight_unit")]
            public string? WeightUnit { get; set; }
        }

        public class ProductOption
        {
            [JsonPropertyName("product_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long ProductId { get; set; }

            [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("values")]
            public string[]? Values { get; set; }
        }

        public class ProductImage
        {
            [JsonPropertyName("product_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long ProductId { get; set; }

            [JsonPropertyName("id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public long Id { get; set; }

            [JsonPropertyName("created_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTime? CreatedAt { get; set; }

            [JsonPropertyName("udpated_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTime? UpdatedAt { get; set; }

            [JsonPropertyName("width"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public int Width { get; set; }

            [JsonPropertyName("height"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public int Height { get; set; }

            [JsonPropertyName("src"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? Source {  get; set; }

            [JsonPropertyName("variant_ids")]
            public long[]? VariantIds { get; set; }
        }
    }

    public class ProductResponse
    {
        [JsonPropertyName("products")]
        public Product[] Products { get; set; }
    }

    public class ProductRequest
    {
	    [JsonPropertyName("product")] 
	    public Product Product { get; set; }

	    public ProductRequest(Product newProduct)
	    {
		    Product = newProduct;
	    }
    }
}
