using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TaskList;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    public sealed partial class EditTaskDialog : ContentDialog
    {

        public List<TaskType> TaskTypes { get; } = new List<TaskType> { TaskType.Task, TaskType.RepeatTask, TaskType.Habit };
        public List<Frequency> Frequencies { get; } = new List<Frequency> { Frequency.Daily, Frequency.Weekly };


        public EditTaskDialog(Tasks taskToEdit)
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            ViewModel = new TaskViewModel
            {
                Description = taskToEdit.description,
                Notes = taskToEdit.notes,
                IsCompleted = taskToEdit.IsCompleted,
                DateDue = taskToEdit.dateDue,
                //TaskType = taskToEdit.TaskType,
                //Frequency = taskToEdit.Frequency,
                TimeDue = taskToEdit.dateDue?.TimeOfDay ?? TimeSpan.Zero,

                // Initialize other properties as needed
            };
        }


        public TaskViewModel ViewModel { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //taskToEdit.Description = ViewModel.Description;
            //taskToEdit.Notes = ViewModel.Notes;
            //taskToEdit.IsCompleted = ViewModel.IsCompleted;
            //taskToEdit.DateDue = ViewModel.DateDue?.Date + ViewModel.DueTimeOfDay;
            //taskToEdit.TaskType = ViewModel.TaskType;
            //taskToEdit.Frequency = ViewModel.Frequency;

            // Raise the PropertyChanged event for the updated properties
            //taskToEdit.OnPropertyChanged(nameof(Task.Description));
            //taskToEdit.OnPropertyChanged(nameof(Task.Notes));
            //taskToEdit.OnPropertyChanged(nameof(Task.IsCompleted));
            //taskToEdit.OnPropertyChanged(nameof(Task.DateDue));
            //taskToEdit.OnPropertyChanged(nameof(Task.TaskType));
            //taskToEdit.OnPropertyChanged(nameof(Task.Frequency));

            // Close the dialog
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Close the dialog without making any changes
            this.Hide();
        }
    }
}
