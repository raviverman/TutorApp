using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tutor.ValueConverter
{
    class TimeSpanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan t = (TimeSpan)value;
            return t.Duration().ToString().Substring(0, 8);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string timeSpan = (string)value;
            return TimeSpan.Parse(timeSpan).ToString(); ;
        }
    }
}
