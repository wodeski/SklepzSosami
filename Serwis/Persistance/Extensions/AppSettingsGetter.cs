namespace Serwis.Models.Extensions
{
    static class AppSettingsGetter
    {
        public static IConfiguration AppSetting { get; }
        static AppSettingsGetter()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}
