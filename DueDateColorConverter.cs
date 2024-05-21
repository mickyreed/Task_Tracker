using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI
using Windows.UI.Xaml
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
namespace TaskList
{
    public class DueDateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dueDate)
            {
                if (dueDate.Date < DateTime.Now.Date)
                {
                    return new SolidColorBrush(Colors.Red); // Overdue
                }
                else if (dueDate.Date == DateTime.Now.Date)
                {
                    return new SolidColorBrush(Colors.Green); // Due today
                }
                else
                {
                    return new SolidColorBrush(Colors.Black); // Normal color
                }
            }

            return new SolidColorBrush(Colors.Black); // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}