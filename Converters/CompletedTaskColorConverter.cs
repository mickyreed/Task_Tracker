using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI;
using Windows.UI.Xaml;


namespace TaskList
{
    public class CompletedTaskColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Check if the task is completed
            bool isCompleted = (bool)value;

            // If the task is completed, return a semi-transparent color
            if (isCompleted)
                return new SolidColorBrush(Color.FromArgb(128, 153, 153, 153)); // Semi-transparent gray
            else
                return new SolidColorBrush(Color.FromArgb(255, 153, 153, 153)); // Original color: Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
