using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
namespace TaskList
{
    public class DueDateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime DateDue)
            {

                if (DateDue.Date < DateTime.Now.Date)
                {
                    Debug.WriteLine("RETURNING OVERDUE RED");
                    return new SolidColorBrush(Colors.Red); // Overdue tasks
                }
                else if (DateDue.Date == DateTime.Now.Date)
                {
                    return new SolidColorBrush(Colors.Green); // Tasks due today
                }
            }

            return new SolidColorBrush(Colors.AliceBlue); // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}