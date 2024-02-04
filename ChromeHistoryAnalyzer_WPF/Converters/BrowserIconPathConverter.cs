using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BrowserHistoryParser_ClassLib;

namespace BrowserHistoryAnalyzer_WPF.Converters
{
    public class BrowserIconPathConverter : IValueConverter
    {
        private readonly Dictionary<string, BitmapImage> _cache = new Dictionary<string, BitmapImage>();
        private readonly string? _projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = (BrowserName)value;
            var dir = (string)parameter;

            if (false == _cache.ContainsKey(name + dir))
            {
                var uri = new Uri(_projectDirectory + $@"\Images\{dir}\{name}.png", UriKind.RelativeOrAbsolute);
                _cache.Add(name + dir, new BitmapImage(uri));
            }

            return _cache[name + dir];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
