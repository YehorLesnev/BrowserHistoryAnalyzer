using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
