using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;
using static TheMule.Models.Printify.Blueprint;
using static TheMule.Models.Printify.Product;

namespace TheMule.Models.Printify
{
    public class Blueprint
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("images")]
        public object Images {
            get => _previewImages;
            set {
                try {
                    // Try deserializing into string[]
                    _previewImages = JsonSerializer.Deserialize<string[]>(value.ToString());
                } catch (JsonException) {
                    try {
                        // Try deserializing into Dictionary<string, string>
                        var options = new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        };
                        var jsonObject = JsonSerializer.Deserialize<Dictionary<string, string>>(value.ToString());
                        _previewImages = jsonObject.Values.ToArray();
                    } catch (JsonException) {
                        _previewImages = System.Array.Empty<string>();                
                    }
                }
            }
        }

        [JsonIgnore]
        private string[] _previewImages { get; set; }

        [JsonIgnore]
        public string ComboBoxText => $"{Title}  -  ({Brand} {Model})";

        [JsonConstructor]
        public Blueprint(int id, string title, string brand, string model, object images) 
        {
            Id = id;
            Title = title;
            Brand = brand; 
            Model = model;
            Images = images;
        }

        public static async Task<IEnumerable<Blueprint>> GetBlueprintsAsync() 
        {
            return await PrintifyService.GetBlueprintsAsync();
        }

        public class BlueprintVariant 
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("options")]
            public BlueprintOptions Options { get; set; }

            [JsonPropertyName("placeholders")]
            public BlueprintPlaceholder[] Placeholders { get; set; }


            public static async Task<IEnumerable<BlueprintVariant>> GetVariantsAsync(int blueprintId, int printProviderId) {
                return await PrintifyService.GetVariantsAsync(blueprintId, printProviderId);
            }
        }

        public class BlueprintOptions 
        {
            [JsonPropertyName("color")]
            public string Colour { get; set; }

            [JsonPropertyName("size")]
            public string Size { get; set; }
        }

        public class BlueprintPlaceholder 
        {
            [JsonPropertyName("position")]
            public string Position { get; set; }

            [JsonPropertyName("height")]
            public int Height { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }
        }
    }


    public class BlueprintVariantsResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string PrintProvider { get; set; }

        [JsonPropertyName("variants")]
        public BlueprintVariant[] Variants { get; set; }
    }
}
