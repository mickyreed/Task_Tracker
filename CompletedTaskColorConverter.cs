using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

public class CompletedTaskColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TaskViewModel task)
        {
            if (task.IsCompleted)
            {
                return new SolidColorBrush(Colors.Gray); // Completed tasks are gray
            }
            else
            {
                if (task.DateDue < DateTime.Today)
                {
                    return new SolidColorBrush(Colors.Red); // Overdue tasks are red
                }
                else if (task.DateDue == DateTime.Today)
                {
                    return new SolidColorBrush(Colors.Green); // Due today tasks are green
                }
                else
                {
                    return new SolidColorBrush(Colors.Black); // Normal tasks are black
                }
            }
        }
        return new SolidColorBrush(Colors.Black); // Default color
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}