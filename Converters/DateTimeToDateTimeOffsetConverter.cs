using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace TaskList
{
    /// <summary>
    /// A Helper class that Converts the DatTime to a DateTime with an offset for use with CalenderDatePicker
    /// this is because we are databinding to the ViewModel of the Task and CalenderDatePicker expects DateTImeOffset
    /// So we need to convert between the two
    /// </summary>
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return new DateTimeOffset(dateTime);
            }

            return DateTime.Today.AddDays(1).AddTicks(-1); // default value
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.DateTime;
            }

            return DateTime.Today.AddDays(1).AddTicks(-1); // default value
        }
    }
}
