using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using System.Diagnostics;

namespace TaskList
{
    public class CompletedTaskColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Tasks task = value as Tasks;
            if (task == null)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (task.IsCompleted)
            {
                return new SolidColorBrush(Colors.Gray); // Completed tasks are gray
            }
            else if (task.dateDue < DateTime.Today)
            {
                return new SolidColorBrush(Colors.Red); // Overdue tasks are red
            }
            else if (task.dateDue == DateTime.Today)
            {
                return new SolidColorBrush(Colors.Green); // Due today tasks are green
            }
            else
            {
                return new SolidColorBrush(Colors.White); // Normal tasks are white
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}