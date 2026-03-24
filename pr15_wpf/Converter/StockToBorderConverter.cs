using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace pr15_wpf.Converter
{
    public class StockToBorderConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock < 10)
                {
                    return (SolidColorBrush) new BrushConverter().ConvertFrom("#ffd129");
                }
            }
            return (SolidColorBrush) new BrushConverter().ConvertFrom("#00C4B4");
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
