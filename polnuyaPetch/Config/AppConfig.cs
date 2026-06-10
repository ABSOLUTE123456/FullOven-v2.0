namespace polnuyaPetch.Config
{
    public class AppConfig
    {
        public string Role { get; set; } = "User";
        public string StorageMode { get; set; } = "Database";
        public bool AskOnStart { get; set; } = false;
        public string LogLevel { get; set; } = "Information";
        public bool DebugMode { get; set; } = false;
        public int MaxBackupsCount { get; set; } = 5;

        public string DataFolder { get; set; } = "data";
        public string LogsFolder { get; set; } = "logs";
        public string BackupsFolder { get; set; } = "backups";
        public string ExportsFolder { get; set; } = "exports";
        public string ReportsFolder { get; set; } = "reports";
    }
}
