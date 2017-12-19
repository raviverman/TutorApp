using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tutor.ValueConverter
{
    public class TimeDurationToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = ((TimeSpan)value);
            return time.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalSeconds = ((double)value);
            return TimeSpan.FromSeconds(totalSeconds);
        }
    }
}
