using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private Folder currentFolder;



        public TasksPage()
        {
            this.InitializeComponent();
            FoldersListView.ItemsSource = Folder.AllFoldersList;
            //this.NavigationCacheMode = NavigationCacheMode.Disabled;
            
            _ = LoadData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshFolderList(); // Refresh folder list when navigating back
            if (e.Parameter is Folder selectedFolder)
            {
                // Use the selectedFolder to load tasks or perform other operations
                currentFolder = selectedFolder;
                RefreshTaskList(selectedFolder);
            }
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    base.OnNavigatedFrom(e);

        //    if (Frame.CanGoBack)
        //    {
        //        var lastEntry = Frame.BackStack.LastOrDefault();
        //        if (lastEntry != null)
        //        {
        //            lastEntry.Parameter = currentFolder;
        //        }
        //    }
        //}

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
