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
    //public class VisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, string language)
    //    {
    //        if (value is TaskType taskType)
    //            //(ViewModel.TaskType == TaskType.Habit)
    //        {
    //            Debug.WriteLine($"VisibilityConverter1: TaskType = {taskType}");
    //            if (value is TaskType.Habit || value is TaskType.RepeatTask)
    //            {
    //                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
    //                return Visibility.Visible;
    //            }
    //            else
    //            {
    //                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
    //                return Visibility.Collapsed;
    //            }

    //        }
    //        // if the Task has notes or if notes is an empty String
    //        else if (value is string notes && !string.IsNullOrEmpty(notes))
    //        {
    //            Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
    //            return Visibility.Visible;
    //        }

    //        //Catch any other
    //        Debug.WriteLine($"Unhandled value type: {value?.GetType().ToString() ?? "null"}");
    //        return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, string language)
    //    {
    //        if (value is Visibility visibility)
    //        {
    //            // This is a very simplistic ConvertBack implementation and might not fit all use cases.
    //            return visibility == Visibility.Visible ? TaskType.Task : TaskType.Habit;
    //        }
    //        throw new NotImplementedException();
    //    }
    //}

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TaskType taskType)
            {
                Debug.WriteLine($"VisibilityConverterTO: TaskType = {taskType}");
                return (taskType == TaskType.Habit || taskType == TaskType.RepeatTask) ? Visibility.Visible : Visibility.Collapsed;
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
