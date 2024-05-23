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
    /// A Helper class that converts the Frequqncy enum as we are binding it to the UI control
    /// and need the enum values to display properly
    /// </summary>
    public class FrequencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Frequency frequency)
            {
                return frequency;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Frequency frequency)
            {
                return frequency;
            }
            return null;
        }

        //public object ConvertBack(object value, Type targetType, object parameter, string language)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
