using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// A class that represents the Dialog Window for Editing existing Tasks
    /// </summary>
    public sealed partial class EditTaskDialog : ContentDialog
    {

        //public List<TaskType> TaskTypes { get; } = new List<TaskType> { TaskType.Task, TaskType.RepeatTask, TaskType.Habit };
        //public List<Frequency> Frequencies { get; } = new List<Frequency> { Frequency.Daily, Frequency.Weekly };

        /// <summary>
        /// Gets or sets the task that is being edited.
        /// This property holds a reference to the task object that is currently
        /// being modified in the dialog, allowing for its details to be displayed
        /// and updated by the user.
        /// </summary>
        private Tasks TaskToEdit { get; set; }

        /// <summary>
        /// A public TaskViewModel used tio display and bind the Tasks in the TaskView List
        /// </summary>
        public TaskViewModel ViewModel { get; set; }

        /// <summary>
        /// A public string variable to hold the name of the Task Type
        /// </summary>
        public String TaskTypeName;

        /// <summary>
        /// Constructor that takes in a Task that we want to edit
        /// </summary>
        /// <param name="taskToEdit"></param>
        public EditTaskDialog(Tasks taskToEdit)
        {
            this.InitializeComponent();
            Loaded += MyDialogBox_Loaded;
            DataContext = ViewModel;
            TaskToEdit = taskToEdit;


            ViewModel = new TaskViewModel
            {
                Description = taskToEdit.description,
                Notes = taskToEdit.notes,
                IsCompleted = taskToEdit.IsCompleted,
                DateDue = taskToEdit.dateDue,
                TaskType = taskToEdit.TaskType,
                Frequency = taskToEdit.Frequency,
                TimeDue = taskToEdit.dateDue?.TimeOfDay ?? TimeSpan.Zero
            };
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// A function that runs when the dialog is loaded, and calls the function to to find and style the buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyDialogBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Find all buttons in the dialog and apply styling
            FindAndStyleButtons();
        }

        /// <summary>
        /// The function that handles the primary button event (save the edited task) and updates the saved data
        /// And will also call an error if date is in the past
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            TaskToEdit.description = ViewModel.Description;
            TaskToEdit.notes = ViewModel.Notes;
            TaskToEdit.IsCompleted = ViewModel.IsCompleted;
            TaskToEdit.dateDue = ViewModel.DateDue?.Date + ViewModel.TimeDue;
            TaskToEdit.TaskType = ViewModel.TaskType;
            TaskTypeName = TaskToEdit.TaskType.GetType().Name.ToString();

            if (TaskToEdit is Habit habit)
            {
                TaskTypeName = TaskType.Habit.ToString();
                habit.frequency = (RepeatTask.Frequency)ViewModel.Frequency;
            }
            else if (TaskToEdit is RepeatTask repeatTask)
            {
                TaskTypeName = TaskType.RepeatTask.ToString();
                repeatTask.frequency = (RepeatTask.Frequency)ViewModel.Frequency;
            }

            // Update the database with the changes
            //TaskDataManagerSQL.UpdateTaskAsync(TaskToEdit).ConfigureAwait(false);
            // Update the database with the changes
            try
            {
                await TaskDataManagerSQL.UpdateTaskAsync(TaskToEdit);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Could not update task. {ex.GetType().Name}: {ex.Message}");
            }

            // Close the dialog
            this.Hide();
        }

        /// <summary>
        /// Function that handles the cancel event and returns to the TaskPage view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Close the dialog without making any changes
            this.Hide();
        }

        /// <summary>
        /// Function that recursively finds all the buttons in the dialog window and styles them
        /// </summary>
        private void FindAndStyleButtons()
        {
            // Find all buttons within the dialog box
            FindButtonsRecursive(this);

            void FindButtonsRecursive(DependencyObject parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child is Button button)
                    {
                        // Apply the desired styling to the button
                        button.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                        button.CornerRadius = new CornerRadius(10);
                        button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);  // FromArgb(255, 62, 78, 77)); // #FF3E4D
                    }
                    else
                    {
                        // Recursively search for buttons in child elements
                        FindButtonsRecursive(child);
                    }
                }
            }
        }
    }
}
