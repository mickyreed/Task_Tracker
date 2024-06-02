using Microsoft.Recognizers.Text.Matcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
using static TaskList.MainPage;

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

        //public IOrderedEnumerable<Tasks> sortedTasks { get; set; }
        private IOrderedEnumerable<Tasks> sortedTasks;

        /// <summary>
        /// 
        /// </summary>
        public Folder selectedFolder { get; set; }
        public string SelectedFolderName { get; set; }
        public string taskInput { get; set; }
        public static Folder currentFolder;
        private Task currentTask;
        private Guid currentTaskGuid;
        //public var result = (string description, string notes, DateTime? dateTime);
        private bool isUpdating = false;
        private Dictionary<CheckBox, bool> checkBoxStates = new Dictionary<CheckBox, bool>();

        /// <summary>
        /// 
        /// </summary>
        public TasksPage()
        {
            this.InitializeComponent();
            MainPage.ThemeChanged += OnThemeChanged;
            ApplyCurrentTheme();
            FoldersListView.ItemsSource = Folder.AllFoldersList;
            TasksListView.ItemsSource = TasksList;

            TasksList = Tasks.AllTasksList;
            TasksListView.UpdateLayout();
            _ = LoadData();

            currentFolder = CheckCurrentSelectedFolder(SelectedFolderName);
            
            RefreshTaskList(currentFolder);
            TasksListView.ItemsSource = sortedTasks;
            _ = UpdateData();
        }

        /// <summary>
        /// an override function that takes in parameters passed in during the navigation sent from another page
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.WriteLine("Navigated to TasksPage");

            if (e.Parameter is Tuple<Folder, string> navigationParams)
            {
                selectedFolder = navigationParams.Item1;
                taskInput = navigationParams.Item2;
                CheckUserInput(taskInput);
                currentFolder = selectedFolder;
                Debug.WriteLine($" on Nav1 to: {currentFolder.Name}");
            }
            else if (e.Parameter is string userInput)
            {
                taskInput = userInput;
                Debug.WriteLine($" on Nav2");
            }
            else if (e.Parameter is Folder selectedFolder)
            {
                currentFolder = selectedFolder;
                Debug.WriteLine($" on Nav3 to: {currentFolder.Name}");
            }
            RefreshTaskList(currentFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private Folder CheckCurrentSelectedFolder(string folderName)
        {

            // Find the Folder instance with the matching name
            selectedFolder = null;
            foreach (Folder folder in Folder.AllFoldersList)
            {
                if (folder.Name == SelectedFolderName)
                {
                    selectedFolder = folder;
                    return selectedFolder;
                }

            }
            return selectedFolder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedFolder"></param>
        public void RefreshTaskList(Folder selectedFolder)
        {
            if (selectedFolder != null)
            {
                // Update the current folder
                currentFolder = selectedFolder;
                
                CheckCurrentSelectedFolder(currentFolder.Name);

                // Update UI or perform any other actions based on the selection
                UpdateFolderNameTextBox(selectedFolder.Name);

                List<Guid> taskIds = new List<Guid>();
                if (currentFolder.TaskCount > 0)
                {
                    foreach (var task in currentFolder.taskId)
                    {
                        taskIds.Add(task);
                    }
                }

                // Create a collection to hold the tasks
                ObservableCollection<Tasks> tasksCollection = new ObservableCollection<Tasks>();
                // Iterate through the task IDs and retrieve the corresponding tasks
                if (taskIds.Count > 0)
                {
                    foreach (var taskId in taskIds)
                    {
                        // Retrieve the task using the taskId
                        Tasks task = Tasks.GetTaskById(taskId);
                        if (task != null)
                        {
                            // Add the task to the collection
                            tasksCollection.Add(task);
                        }
                    };
                }
                else
                {
                    tasksCollection.Clear();
                }
                
                // Set the header to the folder name
                FolderHeaderTextBlock.Text = selectedFolder.Name;
                UpdateSortedTasks(tasksCollection);

            }
        }

        /// <summary>
        /// Function that creates an observable collection then orders the tasks by IsCompleted, then by due Date, then by decending
        /// </summary>
        /// <param name="currentTasksList"></param>
        public void UpdateSortedTasks(ObservableCollection<Tasks> currentTasksList)
        {
            if (currentTasksList == null)
            {
                Debug.WriteLine("1. currentTasksList is null.");
                return;
            }

            var sortedTasks = currentTasksList
                .OrderBy(t => t.IsCompleted) // order by incomplete tasks first
                .ThenBy(t => t.dateDue ?? DateTime.MaxValue)
                .ThenByDescending(t => t.dateDue ?? DateTime.MinValue);

            TasksListView.ItemsSource = sortedTasks;
        }

        private async Task LoadData()
        {
            if (FoldersList != null)
            {
                FoldersList.Clear(); // clear the existing folder UI list
            }
            else
            {
                FoldersList = new ObservableCollection<Folder>(); // Initialize the FoldersList if it's null
            }

            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine(folder.Name);
                FoldersList.Add(folder);
                Debug.WriteLine($"***!!!***     {folder.Name}       ***!!!***");
            }

            FoldersListView.ItemsSource = FoldersList;
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
            RefreshFolderList(currentFolder);
            RefreshTaskList(currentFolder);
        }

        /// <summary>
        /// A fcuntion which clears the current folder list, and then repopulates it based on the latest saved data
        /// </summary>
        /// <param name="currentFolder"></param>
        private async void RefreshFolderList(Folder currentFolder)
        {
            FoldersList.Clear(); // clear the existing folder UI list

            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine(folder.Name);
                FoldersList.Add(folder);
                Debug.WriteLine($"***!!!***     {folder.Name}       ***!!!***");
            }

        }

        /// <summary>
        /// A function which updates the folders listview based on if there is a selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if there is any selected item
            if (e.AddedItems.Count > 0)
            {
                // Get the selected folder
                selectedFolder = e.AddedItems[0] as Folder;
                currentFolder = selectedFolder;
                RefreshTaskList(selectedFolder);
            }
        }

        /// <summary>
        /// A function which updates the user Text box with the current folder name
        /// </summary>
        /// <param name="selectedFolderName"></param>
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
                }
                InputTextBox.Text = string.Empty; // if we nav to Task page this may be redundant
            }
        }

        /// <summary>
        /// A function which takes the users input and passes it in and has it evaluated and returned as 2 strings for desciption and notes and a dateTime?
        /// </summary>
        /// <param name="userInput"></param>
        public async void CheckUserInput(string userInput)
        {
            var result = await TaskCreator.CheckUserInput(userInput);
            Debug.WriteLine($"String 1: {result}");
            Debug.WriteLine($"String 2: {result.Item2}");
            Debug.WriteLine($"DateTime: {result.Item3}");
            if (result.Item3 == null)
            {
                result.Item3 = DateTime.Now;
            }

            await OpenPopupCreateTask(result.Item3, result.Item1, result.Item2);
        }

        /// <summary>
        /// A function that gets called and takes the user input and passes it in to open up a dialag with the task information
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        private async Task OpenPopupCreateTask(DateTime? dateTime, string description, string notes)
        {
            DateTime? nullableDateTime = dateTime;
            DateTime date;
            TimeSpan timeSpan = TimeSpan.Zero;

            TaskViewModel viewModel;
            
            // Try to extract a time of day as a timespan
            if (nullableDateTime.HasValue)
            {
                date = nullableDateTime.Value;

                // Extract the date component and time component
                if (date.TimeOfDay == TimeSpan.Zero && date.TimeOfDay < DateTime.Now.TimeOfDay)
                {
                    // The user specified only the date without the time.
                    timeSpan = new TimeSpan(23, 59, 59);
                    date = nullableDateTime.Value.Date;
                }
                else
                {
                    timeSpan = date.TimeOfDay;
                    date = nullableDateTime.Value.Date;
                }
            }

            else
            {
                // If there's no dateTime value, set to 11:59:59 PM of the current day
                date = DateTime.Today;
                timeSpan = new TimeSpan(23, 59, 59);
            }

            viewModel = new TaskViewModel()
            {
                Description = description,
                Notes = notes,
                DateDue = date,
                TimeDue = timeSpan,
                Frequency = Frequency.Daily,
                Streak = 0

            };

            // Create an instance of CreateTaskDialog and pass the ViewModel
            CreateTaskDialog createTaskDialog = new CreateTaskDialog(viewModel);

            // Show the dialog and await the result
            ContentDialogResult result = await createTaskDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await UpdateData();
                
                RefreshTaskList(currentFolder);
                Debug.WriteLine("Refreshing Task List");
            }
            else
            {
                // User pressed "No" or closed the dialog
            }
            await UpdateData();
            RefreshTaskList(currentFolder);
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

        /// <summary>
        /// A function that is called when the input text box text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //
        }

        /// <summary>
        ///  A function that opens the aside when the toggle menu is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the state of the SplitView pane
            TasksSplitView.IsPaneOpen = !TasksSplitView.IsPaneOpen;
        }

        /// <summary>
        /// A function that naviagtes back to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the MainPage
            Frame.GoBack();
        }

        //A function activated when the mouse pointer enters the create task button - unused in thuis version
        private void CreateTaskButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //CreateTaskButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            //CreateTaskButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
        }

        /// <summary>
        /// A function activated when the mouse pointer exits the create task button - unused in thuis version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateTaskButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //CreateTaskButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
            //CreateTaskButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
        }

        /// <summary>
        /// A function which closes the side pane when the user moves the mouse out of the aside
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TasksSplitView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            await Task.Delay(300);
            TasksSplitView.IsPaneOpen = false;
        }

        /// <summary>
        /// A function which opens a edit Task dialog when the users clicks on the edit task button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected Task object from the ListView
            Tasks selectedTask = (sender as Button)?.Tag as Tasks;

            if (selectedTask != null)
            {
                // Create an instance of the EditTaskDialog and pass the selected Task object
                EditTaskDialog editTaskDialog = new EditTaskDialog(selectedTask);

                // Show the dialog and handle the result
                ContentDialogResult result = await editTaskDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    RemoveTask(currentTaskGuid);
                    await UpdateData();
                    RefreshTaskList(currentFolder);
                }
                else
                {
                    // User pressed "No" or closed the dialog
                }
            }
        }

        /// <summary>
        /// A function which gets actioned from the delete task button action and opens a popup to confirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;

            // Get the task object from the Tag property
            Tasks tasks = button?.Tag as Tasks;

            if (tasks != null)
            {
                // Get the ID of the task
                Debug.WriteLine($"\n Getting Task ID: {currentTaskGuid}");
                currentTaskGuid = tasks.id;
                DeleteTaskPopup.IsOpen = true;

            }
            else
            {
                Debug.WriteLine("Task is null or could not be casted from Tag.");
            }
        }

        /// <summary>
        /// Function to delete a task
        /// </summary>
        /// <param name="idToLookup"></param>
        public async void RemoveTask(Guid idToLookup)
        {
            currentTaskGuid = idToLookup;

            Tasks.RemoveTask(currentTaskGuid);
            currentFolder.RemoveTask(currentTaskGuid);

            //Delete Task from SQL Database
            _ = TaskDataManagerSQL.DeleteTaskByIdAsync(currentTaskGuid);
            Debug.WriteLine($"\n Removed Task with ID: {currentTaskGuid}");

            await UpdateData();
            RefreshTaskList(currentFolder);
            
        }

        /// <summary>
        /// Function that deletes a folder if the user presses the Confrim button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteTaskYes_Click(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("DELETING A TASK");

            if (currentTaskGuid != null)
            {
                RemoveTask(currentTaskGuid);
            }

            // Close Popup after deletion
            DeleteTaskPopup.IsOpen = false;

        }

        /// <summary>
        /// Function that takes a checkbox ticked unyticked action and sets a bool which gets updated in the Task Class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            Tasks taskItem = checkBox?.Tag as Tasks;

            if (taskItem != null)
            {
                if (checkBox != null)
                {
                    bool isChecked = checkBox.IsChecked ?? false;
                    taskItem.IsCompleted = isChecked;
                }
            }
            await UpdateData();
            RefreshTaskList(currentFolder);
            TasksListView.UpdateLayout();
        }

        /// <summary>
        /// Function to check for a theme change and updates the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ApplyTheme(e.Theme);
        }

        /// <summary>
        /// A function that checks if there is a saved AppTheme in applicationData and calls the ApplyTheme function
        /// </summary>
        private void ApplyCurrentTheme()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("AppTheme"))
            {
                string savedTheme = localSettings.Values["AppTheme"].ToString();
                ApplyTheme(savedTheme);
            }
        }

        /// <summary>
        /// Function that gets the saved theme from preferences and applies it in the app
        /// </summary>
        /// <param name="theme"></param>
        private void ApplyTheme(string theme)
        {
            if (App.Current.Resources["AppBackgroundBrush"] is LinearGradientBrush backgroundBrush)
            {
                var gradientStops = backgroundBrush.GradientStops;
                if (theme == "Light")
                {
                    gradientStops[0].Color = Windows.UI.Colors.White;
                    gradientStops[1].Color = Windows.UI.Colors.LightGray;
                }
                else if (theme == "Dark")
                {
                    gradientStops[0].Color = Windows.UI.Colors.Black;
                    gradientStops[1].Color = Windows.UI.Colors.Gray;
                }
            }
        }

        /// <summary>
        /// A function that takes in a string value which is the name of an existing popup window,
        /// It finds the open pop up and closes it when a user presses the cancel button
        /// </summary>
        /// <param name="popupName"></param>
        private void ClosePopup(string popupName)
        {
            var popup = FindName(popupName) as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

        /// <summary>
        /// An event handler for finding the open pop up window name based on the Tag element from the button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var popupName = button.Tag as string;
                ClosePopup(popupName);
            }
        }

    }
}
