using System;
using System.IO;
using System.Text.Json;

namespace polnuyaPetch.Config
{
    public class ConfigService
    {
        private readonly string _configPath;

        public ConfigService()
        {
            _configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        }

        public AppConfig LoadOrCreateDefault()
        {
            if (!File.Exists(_configPath))
            {
                var cfg = new AppConfig();
                Save(cfg);
                return cfg;
            }
            try
            {
                var json = File.ReadAllText(_configPath);
                var cfg = JsonSerializer.Deserialize<AppConfig>(json);
                return cfg ?? new AppConfig();
            }
            catch
            {
                var cfg = new AppConfig();
                Save(cfg);
                return cfg;
            }
        }

        public void Save(AppConfig cfg)
        {
            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_configPath, json);
        }
    }
}
