using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TaskList
{
    /// <summary>
    /// A Helper class that hides/unhides xaml components based on actions fo the user
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            if (value is TaskType taskType)
            {
                Debug.WriteLine($"VisibilityConverterTO: TaskType = {taskType}");
                return (taskType == TaskType.Habit || taskType == TaskType.RepeatTask) ? Visibility.Visible : Visibility.Collapsed;
            }
            //if the Task has notes or if notes is an empty String
            else if (value is string notes && !string.IsNullOrEmpty(notes))
            {
                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                return Visibility.Visible;
            }
            else if (value is DateTime)
            {
                DateTime date = (DateTime)value;
                if(date < DateTime.Now)
                {
                    Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                    return Visibility.Visible;
                }
               
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskType taskType)
            {
                Debug.WriteLine($"VisibilityConverterBACK: TaskType = {taskType}");
                return (taskType == TaskType.Task) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }
}
