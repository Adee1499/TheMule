using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TheMule.Services;

namespace TheMule.Models.Printify
{
    public class PrintProvider
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("location")]
        public PrintProviderLocation Location { get; set; }

        [JsonIgnore]
        public string ComboBoxText => $"{Title}  ({Location.Country})";


        [JsonConstructor]
        public PrintProvider(int id, string title, PrintProviderLocation location) 
        {
            Id = id;
            Title = title;
            Location = location;
        }

        public static async Task<IEnumerable<PrintProvider>> GetPrintProvidersAsync() 
        {
            return await PrintifyService.GetPrintProvidersAsync();
        }

        public static async Task<IEnumerable<PrintProvider>> GetPrintProvidersForBlueprintAsync(int blueprintId) 
        {
            return await PrintifyService.GetPrintProvidersForBlueprintAsync(blueprintId);
        }
    }
    
    public class PrintProviderLocation 
    {
        [JsonPropertyName("address1")]
        public string Address1 { get; set; }

        [JsonPropertyName("address2")]
        public string Address2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("zip")]
        public string PostCode { get; set; }
    }
}
