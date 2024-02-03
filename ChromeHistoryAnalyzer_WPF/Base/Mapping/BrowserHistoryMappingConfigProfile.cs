using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChromeHistoryAnalyzer_WPF.Models;
using ChromeHistoryAnalyzer_WPF.ViewModels;
using ChromeHistoryParser_ClassLib;

namespace BrowserHistoryAnalyzer_WPF.Base.Mapping
{
    public class BrowserHistoryMappingConfigProfile : Profile
    {
        public BrowserHistoryMappingConfigProfile()
        {
            CreateMap<BrowserHistoryModel, BrowserHistoryViewModel>();
            CreateMap<BrowserHistoryViewModel, BrowserHistoryModel>();

            CreateMap<HistoryItem, HistoryItemViewModel>();
            CreateMap<HistoryItemViewModel, HistoryItem>();
        }
    }
}
