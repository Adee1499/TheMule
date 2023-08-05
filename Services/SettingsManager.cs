using System;
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
        public PrintifySettings Printify { get; set; }
        public CloudflareSettings Cloudflare {  get; set; }

        public class PrintifySettings
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
    }
}
