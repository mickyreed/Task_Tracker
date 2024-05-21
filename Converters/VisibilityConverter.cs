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
                //(ViewModel.TaskType == TaskType.Habit)
            {
                Debug.WriteLine($"VisibilityConverter1: TaskType = {taskType}");
                if (value is TaskType.Habit || value is TaskType.RepeatTask)
                {
                    Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                    return Visibility.Visible;
                }
                else
                {
                    Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                    return Visibility.Collapsed;
                }

            }
            // if the Task has notes or if notes is an empty String
            else if (value is string notes && !string.IsNullOrEmpty(notes))
            {
                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                return Visibility.Visible;
            }

            //Catch any other
            else
            {
                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                return Visibility.Collapsed;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskType taskType)
            {
                Debug.WriteLine($"VisibilityConverter2: TaskType = {taskType}");
                return taskType != TaskType.RepeatTask
                    || taskType != TaskType.Habit ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        //public object ConvertBack(object value, Type targetType, object parameter, string language)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
