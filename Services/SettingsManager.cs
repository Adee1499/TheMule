using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace TheMule.Services
{
    public static class SettingsManager
    {
        public static AppSettings appSettings;

        private static readonly string _settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheMule", "settings.json");

        public static void SaveSettings() {
            if (appSettings == null) return;

            var settingsDirectory = Path.GetDirectoryName(_settingsFilePath);
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);

            var serializedSettings = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, serializedSettings);
        }

        public static void LoadSettings() {
            if (!File.Exists(_settingsFilePath)) return;

            var serializedSettings = File.ReadAllText(_settingsFilePath);
            appSettings = JsonSerializer.Deserialize<AppSettings>(serializedSettings);
        }
    }

    public class AppSettings 
    {
        public PrintifyServiceSettings PrintifyService { get; set; }
        public CloudflareSettings CloudflareService {  get; set; }
        public PrintifySettings Printify { get; set; }

        public class PrintifyServiceSettings
        {
            public string API_Key { get; set; }
            public string Shop_Id { get; set; }
        }

        public class CloudflareSettings 
        {
            public string Access_Key { get; set; }
            public string Secret_Key { get; set; }
            public string PublicUrl { get; set; }
        }

        public class PrintifySettings 
        {
            public Dictionary<int, BlueprintSettings> Blueprints { get; set; }
        }

        public class BlueprintSettings 
        {
            public BlueprintPrintProviderSettings UK { get; set; }
            public BlueprintPrintProviderSettings EU { get; set; }
            public BlueprintPrintProviderSettings US { get; set; }
            public BlueprintPrintProviderSettings AU { get; set; }
        }

        public class BlueprintPrintProviderSettings 
        {
            public int PrintProviderId { get; set; }
            public PlaceholdersKey Placeholders { get; set; }
        }

        public class PlaceholdersKey 
        {
            public PlaceholderSettings Front { get; set; }
            public PlaceholderSettings Neck { get; set; }
        }

        public class PlaceholderSettings 
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Scale { get; set; }
            public float Angle { get; set; }
        }
    }
}
