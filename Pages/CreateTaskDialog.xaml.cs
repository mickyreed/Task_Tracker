using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    /// <summary>
    /// A Class that represents the dialog window for creating a new Task
    /// </summary>
    public sealed partial class CreateTaskDialog : ContentDialog
    {
        private bool _isClosing;
        DateTime? MinDate = DateTime.Today.AddDays(+1).AddTicks(-1);

        

        public TaskViewModel ViewModel { get; set; }

        public List<TaskType> TaskTypes { get; } = new List<TaskType> { TaskType.Task, TaskType.RepeatTask, TaskType.Habit };
        public List<Frequency> Frequencies { get; } = new List<Frequency> { Frequency.Daily, Frequency.Weekly };

        /// <summary>
        /// Constructor taht takes in a TaskViewModel parameter
        /// </summary>
        /// <param name="viewModel"></param>
        public CreateTaskDialog(TaskViewModel viewModel)
        {
            this.InitializeComponent();
            // Find all buttons in the dialog and apply styling
            Loaded += MyDialogBox_Loaded;
            //ViewModel = new TaskViewModel();
            this.ViewModel = viewModel;
            this.DataContext = viewModel;
            
        }

        /// <summary>
        /// Function that runs when the dialog box loads, and calls the Button Style Functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyDialogBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Find all buttons in the dialog and apply styling
            FindAndStyleButtons();
        }

        /// <summary>
        /// Function that handles the primary button (create task) event
        /// And takes all the entered data and creates a Task, Habit or RepeatTask from it, or handles an error if its a date in the past
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Access the ViewModel
            var viewModel = (TaskViewModel)this.DataContext;

            // Extract the data from the ViewModel
            TaskType taskType = viewModel.TaskType;
            string description = viewModel.Description;
            string notes = viewModel.Notes;
            DateTime? dateDue = viewModel.DateDue;
            TimeSpan? timeDue = viewModel.NonNullableTimeDue;
            Frequency frequency = viewModel.Frequency;

            //// Create the Tasks object
            //Tasks newTask = new Tasks
            //{
            //    TaskType = taskType,
            //    description = description,
            //    notes = notes,
            //    DateDue = dateDue,
            //    //TimeDue = timeDue,
            //    Frequency = frequency
            //};

            // Validate time to ensure it is not in the past
            DateTime selectedDateTime = ViewModel.NonNullableDateDue.Add(ViewModel.NonNullableTimeDue);
            if (selectedDateTime < DateTime.Now)
            {
                // Store the state of the original dialog
                var dialogState = new DialogState
                {
                    Description = ViewModel.Description,
                    Notes = ViewModel.Notes,
                    TaskType = ViewModel.TaskType,
                    DateDue = MinDate,
                    TimeDue = MinDate.Value.TimeOfDay,
                    Frequency = ViewModel.Frequency
                };


                args.Cancel = true;
            _isClosing = true;

            this.Hide();

            Task.Delay(300);

            var errorDialog = new ContentDialog
            {
                Title = "Invalid Task Due Date",
                Content = "The selected Date & Time cannot be in the past.",
                CloseButtonText = "Ok"
            };

            await errorDialog.ShowAsync();

            // After closing the error dialog, reopen the original dialog
            if (_isClosing)
            {
                _isClosing = false;
            }
            }
            else
            {
                Debug.WriteLine("Created Task will happen here...");
                // Create a new Task based on the ViewModel properties

                var folder = Folder.AllFoldersList.FirstOrDefault(f => f.id == TasksPage.currentFolder.id);
                if (folder != null)
                {
                    switch (ViewModel.TaskType)
                    {
                        case TaskType.Task:
                            Debug.WriteLine("Task");
                            var Task = new Tasks
                            {
                                description = viewModel.Description,
                                notes = viewModel.Notes,
                                dateDue = viewModel.NonNullableDateDue + timeDue,
                            };

                            //Add the new task to the AllTaskList
                            Tasks.AddTask(Task);
                            TasksPage.currentFolder.AddTask(Task.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(Task);
                            await TaskDataManager.SaveDataAsync();
                            Debug.WriteLine($"saving Task: {Task.description}");
                            break;

                        case TaskType.RepeatTask:
                            Debug.WriteLine("RepeatTask");
                            var repeatTask = new RepeatTask
                            {
                                description = viewModel.Description,
                                notes = viewModel.Notes,
                                dateDue = viewModel.NonNullableDateDue + timeDue,
                                frequency = (RepeatTask.Frequency)viewModel.Frequency
                            };
                            Tasks.AddTask(repeatTask);
                            TasksPage.currentFolder.AddTask(repeatTask.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(repeatTask);
                            await TaskDataManager.SaveDataAsync();
                            break;

                        case TaskType.Habit:
                            Debug.WriteLine("Habit");
                            var habit = new Habit
                            {
                                description = viewModel.Description,
                                notes = viewModel.Notes,
                                dateDue = viewModel.NonNullableDateDue + timeDue,
                                frequency = (Habit.Frequency)viewModel.Frequency,
                                //streak = viewModel.Streak
                                streak = 0
                            };
                            Tasks.AddTask(habit);
                            TasksPage.currentFolder.AddTask(habit.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(habit);
                            await TaskDataManager.SaveDataAsync();
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// Functyion that hadnles the cancel task button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Logic to cancel task creation
            Debug.WriteLine("CANCELLED Task creation...");
        }

        /// <summary>
        /// A function that recursively finds any buttons inthe dialog and applies styles to them
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

    /// <summary>
    /// A function that saves the current state of the Dialog data so we can close it 
    /// as there can only be one dialog open at a time.
    /// and then we can display another dialog when the date is in the past
    /// Before re-opening the dialog with the saved information
    /// </summary>
    public class DialogState
    {
        public string Description { get; set; }
        public string Notes { get; set; }
        public TaskType TaskType { get; set; }
        public DateTime? DateDue { get; set; }
        public TimeSpan? TimeDue { get; set; }
        public Frequency Frequency { get; set; }
    }

}
