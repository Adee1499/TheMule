using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TheMule.Services
{
    public static class SettingsManager
    {
        public static AppSettings appSettings;
        public static readonly string CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheMule", "Cache");

        private static readonly string _settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TheMule", "settings.json");

        public static void SaveSettings() {
            if (appSettings == null) return;

            var settingsDirectory = Path.GetDirectoryName(_settingsFilePath);
            if (!Directory.Exists(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);

            var serializedSettings = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
            serializedSettings = CleanUpSerializedReactiveObjectHack(serializedSettings);
            File.WriteAllText(_settingsFilePath, serializedSettings);
        }

        public static void LoadSettings() {
            if (!File.Exists(_settingsFilePath)) return;

            var serializedSettings = File.ReadAllText(_settingsFilePath);
            appSettings = JsonSerializer.Deserialize<AppSettings>(serializedSettings);
        }

        private static string CleanUpSerializedReactiveObjectHack(string json) {
            json = Regex.Replace(json, @"\s*""(Changing|Changed|ThrownExceptions)"":\s*\{},?\n?", string.Empty);
            json = Regex.Replace(json, @",(\s*})", "$1");
            return json;
        }
    }

    public class AppSettings 
    {
        public PrintifyServiceSettings PrintifyService { get; set; }
        public CloudflareSettings CloudflareService {  get; set; }
        public PrintifySettings Printify { get; set; }

        public class PrintifyServiceSettings
        {
            public string APIKey { get; set; }
            public string ShopId { get; set; }
        }

        public class CloudflareSettings 
        {
            public string AccessKey { get; set; }
            public string SecretKey { get; set; }
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

            public BlueprintSettings() {
                UK = new BlueprintPrintProviderSettings {
                    PrintProviderId = 0, Placeholders = new PlaceholdersKey {
                        Front = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 },
                        Neck = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 }
                    }
                };
                EU = new BlueprintPrintProviderSettings {
                    PrintProviderId = 0, Placeholders = new PlaceholdersKey {
                        Front = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 },
                        Neck = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 }
                    }
                };
                US = new BlueprintPrintProviderSettings {
                    PrintProviderId = 0, Placeholders = new PlaceholdersKey {
                        Front = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 },
                        Neck = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 }
                    }
                };
                AU = new BlueprintPrintProviderSettings {
                    PrintProviderId = 0, Placeholders = new PlaceholdersKey {
                        Front = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 },
                        Neck = new PlaceholderSettings { X = 0, Y = 0, Angle = 0, Scale = 0 }
                    }
                };
            }
        }

        public class BlueprintPrintProviderSettings : ReactiveObject
        {
            private int _printProviderId;
            public int PrintProviderId {
                get => _printProviderId;
                set {
                    this.RaiseAndSetIfChanged(ref _printProviderId, value);
                    SettingsManager.SaveSettings();
                }
            }

            private PlaceholdersKey _placeholders;
            public PlaceholdersKey Placeholders {
                get => _placeholders;
                set {
                    this.RaiseAndSetIfChanged(ref _placeholders, value);
                    SettingsManager.SaveSettings();
                }
            }
        }

        public class PlaceholdersKey : ReactiveObject
        {
            private PlaceholderSettings _front;
            public PlaceholderSettings Front {
                get => _front;
                set {
                    this.RaiseAndSetIfChanged(ref _front, value);
                    SettingsManager.SaveSettings();
                }
            }

            private PlaceholderSettings _neck;
            public PlaceholderSettings Neck {
                get => _neck;
                set {
                    this.RaiseAndSetIfChanged(ref _neck, value);
                    SettingsManager.SaveSettings();
                }
            }
        }

        public class PlaceholderSettings : ReactiveObject
        {
            private float _x;
            public float X {
                get => _x;
                set {
                    this.RaiseAndSetIfChanged(ref _x, value);
                    SettingsManager.SaveSettings();
                }
            }

            private float _y;
            public float Y {
                get => _y;
                set {
                    this.RaiseAndSetIfChanged(ref _y, value);
                    SettingsManager.SaveSettings();
                }
            }

            private float _scale;
            public float Scale {
                get => _scale;
                set {
                    this.RaiseAndSetIfChanged(ref _scale, value);
                    SettingsManager.SaveSettings();
                }
            }

            private float _angle;
            public float Angle {
                get => _angle;
                set {
                    this.RaiseAndSetIfChanged(ref _angle, value);
                    SettingsManager.SaveSettings();
                }
            }
        }
    }
}
