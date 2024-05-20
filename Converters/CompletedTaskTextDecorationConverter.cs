using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Text;
using Windows.UI.Xaml;

namespace TaskList
{
    public class CompletedTaskTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Check if the task is completed
            bool isCompleted = (bool)value;

            // If the task is completed, return strikethrough text decoration
            if (isCompleted)
                return TextDecorations.Strikethrough;
            else
                return null; // No text decoration if the task is not completed
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
