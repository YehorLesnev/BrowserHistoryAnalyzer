using AutoMapper;

namespace BrowserHistoryAnalyzer_WPF.Base.Mapping
{
    public class BrowserHistoryMappingConfig
    {
        public static MapperConfiguration GetConfig()
        {
            return new MapperConfiguration(mc =>
            {
                mc.AllowNullCollections = true;

                mc.AddProfile(new BrowserHistoryMappingConfigProfile());
            });
        }
    }
}
