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
    internal class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //if (value is Task task)
            //{
            //    if (!task.IsCompleted)
            //    {
            //        if (value is DateTime)
            //        {
            //            DateTime date = (DateTime)value;
            //            if (date < DateTime.Now)
            //            {
            //                Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
            //                return Visibility.Visible;
            //            }
            //            else if (date == DateTime.Now.Date)
            //            {
            //                return "&#x1F7E2; Due Today"; // Task due today (green exclamation mark and "Due Today")
            //                //return Visibility.Visible;
            //            }
            //        }
            //    }

                if (value is DateTime)
                {
                    DateTime now = DateTime.Now;
                    DateTime todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);
                    DateTime date = (DateTime)value;

                    if (date > now && date <= todayEnd)
                    {
                        Debug.WriteLine($"TaskType is: {value.GetType().ToString()}");
                        return Visibility.Visible;
                    }
                    //else if (date == DateTime.Now.Date)
                    //{
                    //    return "&#x1F7E2; Due Today"; // Task due today (green exclamation mark and "Due Today")
                    //                                  //return Visibility.Visible;
                    //}
                }

                //        return "✅"; // Completed task (you can use a different emoji or text)
                //}
                //else if (task.DateDue.Date < DateTime.Now.Date)
                //{
                //    return "&#x26A0;&#xFE0F; Overdue"; // Overdue task (red exclamation mark and "Overdue")
                //}
                //else if (task.NonNullableDateDue.Date == DateTime.Now.Date)
                //{
                //    return "&#x1F7E2; Due Today"; // Task due today (green exclamation mark and "Due Today")
                //}
            //}

                return Visibility.Collapsed;
            }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
