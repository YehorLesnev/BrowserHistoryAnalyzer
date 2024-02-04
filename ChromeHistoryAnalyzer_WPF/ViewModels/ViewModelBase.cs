using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoMapper;
using BrowserHistoryAnalyzer_WPF.Base.Mapping;

namespace BrowserHistoryAnalyzer_WPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected Mapper _mapper = new Mapper(BrowserHistoryMappingConfig.GetConfig());

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
