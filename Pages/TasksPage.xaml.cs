using Microsoft.Recognizers.Text.Matcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TasksPage : Page
    {
        // List that we can bind to for the UI
        public static ObservableCollection<Folder> FoldersList { get; set; } // = new ObservableCollection<Folder>();
        public static ObservableCollection<Tasks> TasksList { get; set; } // = new ObservableCollection<Tasks>();

        /// <summary>
        /// 
        /// </summary>
        public string SelectedFolderName { get; set; }
        public string taskInput {  get; set; }
        public static Folder currentFolder;
        //public var result = (string description, string notes, DateTime? dateTime);
        




        public TasksPage()
        {
            this.InitializeComponent();
            FoldersListView.ItemsSource = Folder.AllFoldersList;
            TasksListView.ItemsSource = Tasks.AllTasksList;
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
            //this.DataContext = this;
            TasksListView.UpdateLayout();
            _ = LoadData();
        }

        /// <summary>
        /// an override function that takes in parameters passed in during the navigation sent from another page
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Tuple<Folder, string> navParams)
            {
                currentFolder = navParams.Item1;
                taskInput = navParams.Item2;
                CheckUserInput(taskInput);
            }
            else if (e.Parameter is string userInput)
            {
                taskInput = userInput;
                //var result = TaskCreator.CheckUserInput(taskInput); TODO

            }
            else if (e.Parameter is Folder selectedFolder)
            {
                currentFolder = selectedFolder;
            }

            //TasksListView.ItemsSource = Tasks.AllTasksList;
            RefreshTaskList(currentFolder);
        }

        public void RefreshTaskList(Folder selectedFolder)
        {
            if (selectedFolder != null)
            {
                // Update the current folder
                currentFolder = selectedFolder;

                // Update UI or perform any other actions based on the selection
                UpdateFolderNameTextBox(selectedFolder.Name);

                // Retrieve the list of task IDs from the selected folder
                List<Guid> taskIds = selectedFolder.taskId;

                // Create a collection to hold the tasks
                ObservableCollection<Tasks> tasksCollection = new ObservableCollection<Tasks>();
                // Iterate through the task IDs and retrieve the corresponding tasks
                foreach (var taskId in taskIds)
                {
                    // Retrieve the task using the taskId
                    Tasks task = Tasks.GetTaskById(taskId);
                    //var task = Tasks.GetTaskById(taskId);
                    if (task != null)
                    {
                        // Add the task to the collection
                        tasksCollection.Add(task);
                    }
                    else
                    {
                        tasksCollection.Clear();
                        TasksListView.UpdateLayout();
                    }

                    //}

                    //// Set the ItemsSource of the ListView to the tasks collection
                    TasksListView.ItemsSource = tasksCollection;
                    // Set the header to the folder name
                    FolderHeaderTextBlock.Text = selectedFolder.Name;
                }
            }
        }
        private async Task LoadData()
        {
            if(FoldersList != null)
            {
                FoldersList.Clear(); // clear the existing folder UI list
            }
            else
            {
                FoldersList = new ObservableCollection<Folder>(); // Initialize the FoldersList if it's null
            }

            // DISPLAY FOLDER INFORMATION IN CONSOLE
            //Debug.WriteLine("");
            //Debug.WriteLine("...");
            //Debug.WriteLine("DISPLAY ALL FOLDERS FROM ALLFOLDERS LIST...");

            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine(folder.Name);
                FoldersList.Add(folder);
                Debug.WriteLine($"***!!!***     {folder.Name}       ***!!!***");
            }
            //FoldersList = Folder.AllFoldersList;

            // Set the ItemsSource of FoldersListView to the list of current folders
            //FoldersListView.ItemsSource = FoldersList;
            FoldersListView.ItemsSource = FoldersList;

            //FoldersListView.ItemsSource = Folder.AllFoldersList;


            //Debug.WriteLine("");
            await Task.Delay(100);
        }


        /// <summary>
        /// Function to save data from Tasks and Files to file in Binary format
        /// </summary>
        private async Task SaveData()
        {
            await TaskDataManager.SaveDataAsync();
            await FolderDataManager.SaveDataAsync();
        }

        /// <summary>
        /// this function will update the data after each change and save to the file
        /// </summary>
        /// <returns></returns>
        private async Task UpdateData()
        {
            await SaveData();
            //UpdateTaskIndexes();
            RefreshTaskList(currentFolder);
            RefreshFolderList();
        }
        private async void RefreshFolderList()
        {
            FoldersList.Clear(); // clear the existing folder UI list
                                 //FoldersListView.ItemsSource = null;
                                 // Repopulate list with latest data

            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine(folder.Name);
                FoldersList.Add(folder);
                Debug.WriteLine($"***!!!***     {folder.Name}       ***!!!***");
            }

            //FoldersListView.ItemsSource = FoldersList;
        }

        private void FoldersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if there is any selected item
            if (e.AddedItems.Count > 0)
            {
                // Get the selected folder (assuming each item in the ListView is a Folder instance)
                Folder selectedFolder = e.AddedItems[0] as Folder;
                currentFolder = selectedFolder;
                RefreshTaskList(selectedFolder);
            }
        }

        private void UpdateFolderNameTextBox(string selectedFolderName)
        {
            // Update the TextBox text with the selected folder name
            InputTextBox.PlaceholderText = "Add Task (to " + selectedFolderName + ")";
        }

        /// <summary>
        /// Button CLick Event for recieving input from user and converting it to a Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                ErrorMessage();
            }

            else
            {
                string userInput = InputTextBox.Text.Trim(); // Get the text from the text box
                ResultTextBlock.Text = string.Empty;

                if (userInput != null || userInput != "Enter a value")
                {
                    CheckUserInput(userInput);
                    //await OpenPopupCreateTask(taskParams);
                }
                //ResultTextBlock.Text = output; //this to be removed in final version
                InputTextBox.Text = string.Empty; // if we nav to Task page this may be redundant
            }
        }

        public async void CheckUserInput(string userInput)
        {
            //var result = new Tuple<DateTime?, string, string>(result.Item3, result.Item1, result.Item2);
            //var result = new Tuple<string, string, DateTime?>( result.Item2, result.Item3);
            var result = await TaskCreator.CheckUserInput(userInput);
            Debug.WriteLine($"String 1: {result}");
            Debug.WriteLine($"String 2: {result.Item2}");
            Debug.WriteLine($"DateTime: {result.Item3}");

            await OpenPopupCreateTask(result.Item3, result.Item1, result.Item2);
        }

        private async Task OpenPopupCreateTask(DateTime? dateTime, string description, string notes)
        {
            string dateTimeToDisplay;
            //await result = TaskCreator.CheckUserInput(task);
            Debug.WriteLine("TRYING TO OPEN POPUP");
            
            // Create an instance of TaskViewModel
            TaskViewModel viewModel = new TaskViewModel()
            {
                Description = description,
                Notes = notes,
                DateDue = dateTime ?? DateTime.Now.Date,
                TimeDue = dateTime?.TimeOfDay ?? TimeSpan.Zero // Default to 12:00 AM if time is null
            };

            // Create an instance of CreateTaskDialog and pass the ViewModel
            CreateTaskDialog createTaskDialog = new CreateTaskDialog(viewModel);

            // Show the dialog and await the result
            ContentDialogResult result = await createTaskDialog.ShowAsync();

            // Handle the result
            if (result == ContentDialogResult.Primary)
            {
                await UpdateData();
                RefreshTaskList(currentFolder);
                Debug.WriteLine("Refreshing Task List");
            }
            else
            {
                // User pressed "No" or closed the dialog
                // Add logic for canceling or closing
            }
        }

        /// <summary>
        /// Error Message Function for invalid input
        /// </summary>
        public async void ErrorMessage()
        {
            var dialog = new MessageDialog("Error: You must enter a Task");
            await dialog.ShowAsync();
            InputTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the state of the SplitView pane
            TasksSplitView.IsPaneOpen = !TasksSplitView.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the MainPage
            Frame.GoBack();
        }

        private void CreateTaskButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //CreateTaskButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            //CreateTaskButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
        }

        private void CreateTaskButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //CreateTaskButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
            //CreateTaskButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
        }

        private void TasksSplitView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            TasksSplitView.IsPaneOpen = false;
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private async void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;

            // Get the task object from the Tag property
            Tasks task = button?.Tag as Tasks; // Assuming your task model is named TaskModel

            if (task != null)
            {
                // Get the ID of the task
                Guid taskId = task.id; // Assuming the task has a property named 'id' of type Guid

                // Call the method to remove the task
                RemoveTask(taskId);
            }
            else
            {
                Debug.WriteLine("Task is null or could not be casted from Tag.");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TasksListView.UpdateLayout();
            //UpdateData(); //causes glitch
        }

        /// <summary>
        /// Function to delete a task
        /// </summary>
        /// <param name="idToLookup"></param>
        public async void RemoveTask(Guid idToLookup)
        {
            Guid id = idToLookup;

            Tasks.RemoveTask(id);

            //Delete Task from SQL Database
            await TaskDataManagerSQL.DeleteTaskByIdAsync(id);
            Debug.WriteLine($"\n Removed Task with ID: {id}");
            TasksListView.UpdateLayout();
        }
    }
}
