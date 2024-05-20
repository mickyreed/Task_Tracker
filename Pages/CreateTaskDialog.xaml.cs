using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    public sealed partial class CreateTaskDialog : ContentDialog
    {
        private bool _isClosing;

        public TaskViewModel ViewModel { get; set; }

        public List<TaskType> TaskTypes { get; } = new List<TaskType> { TaskType.Task, TaskType.RepeatingTask, TaskType.Habit };
        public List<Frequency> Frequencies { get; } = new List<Frequency> { Frequency.Daily, Frequency.Weekly };

        public CreateTaskDialog(TaskViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
            DateTime MinDate = DateTime.Today;
        }

        public CreateTaskDialog(DialogState state)
        {
            this.InitializeComponent();
            ViewModel = new TaskViewModel
            {
                Description = state.Description,
                Notes = state.Notes,
                TaskType = state.TaskType,
                DateDue = state.DateDue,
                TimeDue = state.TimeDue,
                Frequency = state.Frequency
            };
            DataContext = ViewModel;
        }

            private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
                    DateDue = ViewModel.NonNullableDateDue,
                    TimeDue = ViewModel.NonNullableTimeDue,
                    Frequency = ViewModel.Frequency
                };

                args.Cancel = true;
                _isClosing = true;
                
                this.Hide();
                
                Task.Delay(300);


                // Show a message to the user
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
                    var reopenedDialog = new CreateTaskDialog(dialogState);
                    await reopenedDialog.ShowAsync();
                }
            }
            else
            {
                // Logic to create the task using ViewModel properties
                Debug.WriteLine("Created Task will happen here...");
                // User pressed "Yes"
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
                                description = ViewModel.Description,
                                notes = ViewModel.Notes,
                                dateDue = ViewModel.DateDue,
                            };

                            //Add the new task to the AllTaskList
                            Tasks.AddTask(Task);
                            TasksPage.currentFolder.AddTask(Task.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(Task);
                            await TaskDataManager.SaveDataAsync();
                            Debug.WriteLine($"saving Task: {Task.description}");
                            break;

                        case TaskType.RepeatingTask:
                            Debug.WriteLine("RepeatingTask");
                            var repeatTask = new RepeatTask
                            {
                                description = ViewModel.Description,
                                notes = ViewModel.Notes,
                                dateDue = ViewModel.DateDue,
                                frequency = (RepeatTask.Frequency)ViewModel.Frequency
                            };
                            Tasks.AddTask(repeatTask);
                            TasksPage.currentFolder.AddTask(repeatTask.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(repeatTask);
                            await TaskDataManager.SaveDataAsync();
                            TasksPage.TasksList.Clear();
                            foreach (var task in Tasks.AllTasksList)
                            {
                                TasksPage.TasksList.Add(task);
                            }
                            break;

                        case TaskType.Habit:
                            Debug.WriteLine("Habit");
                            var habit = new Habit
                            {
                                description = ViewModel.Description,
                                notes = ViewModel.Notes,
                                dateDue = ViewModel.DateDue,
                                frequency = (Habit.Frequency)ViewModel.Frequency,
                                streak = ViewModel.Streak
                            };
                            Tasks.AddTask(habit);
                            TasksPage.currentFolder.AddTask(habit.id);
                            _ = TaskDataManagerSQL.AddTaskAsync(habit);
                            await TaskDataManager.SaveDataAsync();
                            TasksPage.TasksList.Clear();
                            foreach (var task in Tasks.AllTasksList)
                            {
                                TasksPage.TasksList.Add(task);
                            }

                            break;
                    }
                }
                
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Logic to cancel task creation
            Debug.WriteLine("CANCEL Create Task will happen here...");
        }
    }

    public class DialogState
    {
        public string Description { get; set; }
        public string Notes { get; set; }
        public TaskType TaskType { get; set; }
        public DateTime? DateDue { get; set; }
        public TimeSpan? TimeDue { get; set; }
        public Frequency? Frequency { get; set; }
    }
}
