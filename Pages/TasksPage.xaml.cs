﻿using Microsoft.Recognizers.Text.Matcher;
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
        public static ObservableCollection<Folder> FoldersList = new ObservableCollection<Folder>();
        //public static ObservableCollection<Folder> TasksList = new ObservableCollection<Folder>();

        /// <summary>
        /// 
        /// </summary>
        public string SelectedFolderName { get; set; }
        public string taskInput {  get; set; }
        private Folder currentFolder;
        //public var result = (string description, string notes, DateTime? dateTime);
        




        public TasksPage()
        {
            this.InitializeComponent();
            FoldersListView.ItemsSource = Folder.AllFoldersList;
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
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

            RefreshTaskList(currentFolder);
        }

        private void RefreshTaskList(Folder selectedFolder)
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

                }

                // Set the ItemsSource of the ListView to the tasks collection
                TasksListView.ItemsSource = tasksCollection;
                // Set the header to the folder name
                FolderHeaderTextBlock.Text = selectedFolder.Name ;
            }
        }
        private async Task LoadData()
        {
            FoldersList.Clear(); // clear the existing folder UI list
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


            /* !!! TODO: 
            OPEN a dialogue box
            
            // so you can select what type of task and check details etc
            */
            await OpenPopupCreateTask(result.Item3, result.Item1, result.Item2);
        }

        //private void OpenPopupCreateTask(Tuple<DateTime?, string, string> taskParams)


        private async Task OpenPopupCreateTask(DateTime? dateTime, string description, string notes)
        {
            string dateTimeToDisplay;
            //await result = TaskCreator.CheckUserInput(task);
            Debug.WriteLine("TRYING TO OPEN POPUP");
            

            //var taskAdded = new Tasks
            //{
            //    description = description,
            //    notes = notes,
            //    dateDue = dateTime,
            //    isCompleted = false
            //};

            // Create an instance of TaskViewModel (populate with data as needed)
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
                // User pressed "Yes"
                // Add logic for creating a task
            }
            else
            {
                // User pressed "No" or closed the dialog
                // Add logic for canceling or closing
            }
            //var taskAdded = new Tasks
            //{
            //    description = result.;


            //}
            //taskAdded.description = description;
            //taskAdded.notes = notes;
            //taskAdded.dateDue = convertedDateTime;
            //taskAdded.isCompleted = false;
            //Tasks.AddTask(taskAdded);

            // Create and show the dialog
            //var dialog = new CreateTaskDialog(viewModel);
            //var result = await dialog.ShowAsync();

            //if (result == ContentDialogResult.Primary)
            //{
            //    // Create a new Task based on the ViewModel properties
            //    var newTask = new Tasks
            //    {
            //        description = viewModel.Description,
            //        notes = viewModel.Notes,
            //        dateDue = viewModel.DateDue?.Add(viewModel.DueTime),
            //        // Set other properties if needed
            //    };

            //    // Add the new task to your task list
            //    Tasks.AddTask(newTask);
            //}


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

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}