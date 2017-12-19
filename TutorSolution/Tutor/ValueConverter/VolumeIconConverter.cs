using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tutor.ValueConverter
{
    class VolumeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double volume = double.Parse(value.ToString());
            if (volume >= 0.66)
                return "\uE995";
            else if (volume < 0.66 && volume >= 0.33)
                return "\uE994";
            else if (volume < 0.33 &&volume>=0.10)
                return "\uE993";
            else if (volume < 0.10 && volume >= 0.01)
                return "\uE992";
            else
                return "\uE198";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
