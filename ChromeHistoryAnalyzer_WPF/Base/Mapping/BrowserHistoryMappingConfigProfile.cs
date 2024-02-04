using AutoMapper;
using BrowserHistoryAnalyzer_WPF.Models;
using BrowserHistoryAnalyzer_WPF.ViewModels;
using BrowserHistoryParser_ClassLib;

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
