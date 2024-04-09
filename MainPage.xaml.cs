using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
using Windows.Media.Capture;
using Windows.Media.Protection;
using System.Security.Principal;
using Windows.Storage;
using System.Text;
using File = System.IO.File;
using TaskList;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using Windows.UI.Xaml.Shapes;
using Windows.Graphics;
using Windows.Security.Cryptography.Core;

using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;


namespace TaskList
{
    /// <summary>
    /// A program to track and manage someone’s tasks - ie a “to do” list app.
    /// no save or load data(yet…) and no user interface. 
    /// The important part at this stage is the handling of the data.
    /// 
    /// Testing
    /// Create some tasks and folders, and write some code to test everything is working correctly
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadData();

            //If there is no existing folders or data then load default values
            if (Folder.AllFoldersList.Count == 0 && Tasks.AllTasksList.Count == 0)
            {
                DefaultDataInitialiser.LoadDefaultData();
            }
        }
        private async void LoadData()
        {
            await TaskDataManager.LoadDataAsync();
            await FolderDataManager.LoadDataAsync();

            // Call this method after data loading is completed
            DataLoaded();
        }
        private async Task RunTests()
        {
            #region EARLIER TESTS (Depricated)
            //// Removing a task from the folder
            //Debug.WriteLine("...");
            //Debug.WriteLine("DELETE A TASK FROM A FOLDER...");
            //folder1.RemoveTask(task1.id);
            //DisplayTasksInFolders();
            //Debug.WriteLine("");

            ////Check due dates
            //Debug.WriteLine("...");
            //Debug.WriteLine("DISPLAY PERSONAL FOLDER TASK DUE DATES & if Overdue...");
            //Debug.WriteLine("...");
            //foreach (var taskId in folder1.taskId)
            //{
            //    var task = Tasks.GetTaskById(taskId);
            //    Debug.WriteLine
            //        ($"TASK TYPE: {task.GetType().Name}: ");

            //    // Check if the task is a repeating task
            //    if (task is RepeatTask repeatingTask)
            //    {
            //        Debug.WriteLine($"TASK NAME: {repeatingTask.description}");
            //        Debug.WriteLine($"DUE DATE: {repeatingTask.dateDue.ToString()}");
            //        Debug.WriteLine($"FREQUENCY: {repeatingTask.frequency}");
            //        Debug.WriteLine($"Is Completed?: {repeatingTask.isCompleted}");
            //        Debug.WriteLine($"Is Task Overdue?: {repeatingTask.isOverdue}");

            //        Debug.WriteLine(".....");
            //    }
            //    // Check if the task is a habit tasks
            //    else if (task is Habit habits)
            //    {
            //        Debug.WriteLine($"TASK NAME: {habits.description}");
            //        Debug.WriteLine($"DUE DATE: {habits.dateDue.ToString()}");
            //        Debug.WriteLine($"FREQUENCY: {habits.frequency}");
            //        Debug.WriteLine($"STREAK: {habits.streak.ToString()}");
            //        Debug.WriteLine($"Is Completed?: {habits.isCompleted}");
            //        Debug.WriteLine($"Is Task Overdue?: {habits.isOverdue}");

            //        Debug.WriteLine(".....");
            //    }
            //    // Or if its not a habit or repeating task, it's just a Task
            //    else
            //    {
            //        Debug.WriteLine($"TASK NAME: {task.description}");
            //        Debug.WriteLine($"DUE DATE: {task.dateDue}");
            //        Debug.WriteLine($"Is Completed?: {task.isCompleted}");
            //        Debug.WriteLine($"Is Task Overdue?: {task.isOverdue}");
            //        Debug.WriteLine(".....");
            //    }
            //}

            ////
            //// THIS IS TO CHECK THE STREAK GETS ADDED WHEN A HABIT TASK IS COMPLETED
            ////
            //// Check the current due date & streak of the habit task Then mark it complete
            //Debug.WriteLine($"THIS IS TO CHECK THE STREAK GETS ADDED WHEN A HABIT TASK IS COMPLETED");

            //Debug.WriteLine($"Habit Task due date: {habitTask.dateDue}");
            //Debug.WriteLine($"Streak before completion: {habitTask.streak}");
            //Tasks.SetTaskCompletion(habitTask); // call the grand parent class method on the habit task to set completion
            //Debug.WriteLine($"existing habit task is Completed ? : {habitTask.isCompleted}");
            //Debug.WriteLine(".....");

            ////Move habit task to the next due date and check the streak count, then reset complete to not completed

            //habitTask.MoveToNextDueDate();
            //Debug.WriteLine($"Habit Task due date: {habitTask.dateDue}");
            //Debug.WriteLine($"Streak after completion: {habitTask.streak}");
            //Tasks.SetTaskCompletion(habitTask);
            //Debug.WriteLine($"new habit task is Completed ? : {habitTask.isCompleted}");
            //Debug.WriteLine(".....");

            ////
            //// THIS IS TO CHECK THE STREAK RESETS WHEN A HABIT TASK ISNT COMPLETED
            ////
            //Debug.WriteLine($"THIS IS TO CHECK THE STREAK GETS RESET WHEN A HABIT TASK IS NOT COMPLETED");

            //// Check the current due date & streak of the habit task #2 but do not complete it
            //Debug.WriteLine($"Habit Task #2 due date: {habitTask2.dateDue}");
            //Debug.WriteLine($"Streak before completion: {habitTask2.streak}");
            //Debug.WriteLine($"existing habit task #2 is Completed ? : {habitTask.isCompleted}");
            //Debug.WriteLine(".....");

            ////Move habit task to the next due date and check the streak count has reset
            //habitTask.MoveToNextDueDate();
            //Debug.WriteLine($"Habit Task #2 due date: {habitTask.dateDue}");
            //Debug.WriteLine($"Streak after not completing: {habitTask.streak}");
            //Debug.WriteLine($"new habit task #2 is Completed ? : {habitTask.isCompleted}");
            //Debug.WriteLine(".....");

            ////Check the number of incomplet tasks in Folder 1 - This is to test the calculated folder property
            //int numberOfIncompleteTasksInFolder1 = folder1.incompleteTasksTotal(folder1);
            //Debug.WriteLine($"Number of incomplete Tasks in Folder1 = {numberOfIncompleteTasksInFolder1}");
            #endregion
            #region TAKS2 METHOD CALLS (depricated)
            // DisplayAllTasks();
            // Debug.WriteLine($"Number of folders loaded: {Folder.AllFoldersList.Count}");
            // DisplayTasksInFolders();
            // SearchByDescription();
            // SearchByTodaysDate();
            // SearchByDate("2024/03/15"); //date format must be "2008/04/14";
            // SearchByDate("2024/03/12");
            #endregion

            #region TASK 3 TEST METHOD CALLS
            // TEST adding a folder
            Folder folderTest = TestAddFolder();
            await DisplayAllFolders();

            // TEST adding a task
            Guid testGuid = TestAddTask();
            await Task.Delay(200);
            await DisplayTasksInFolders();

            // TEST update the index and save file
            await UpdateData();
            await Task.Delay(200);
            await DisplaySortedIndex();

            //TEST remove the Task
            TestRemoveTask(testGuid);
            await Task.Delay(200);
            await DisplayTasksInFolders();

            // TEST update the index and save file
            await UpdateData();
            await Task.Delay(200);
            await DisplaySortedIndex();

            // TEST: remove the folder
            TestRemoveFolder(folderTest);
            await DisplayAllFolders();

            // TEST update the index and save file
            await UpdateData();
            #endregion

        }
        private async void DataLoaded()
        {
            // Method calls to carry out after data loading is completed
            //await UpdateData();
            //Debug.WriteLine("................................");
            //Debug.WriteLine(".... Folders & Tasks Loaded ....");
            //await DisplayTasksInFolders();
            //await DisplayAllFolders();
            //await DisplayAllTasks();
            //await DisplaySortedIndex();
            Debug.WriteLine("................................");
            Debug.WriteLine("");
            //await RunTests();
            String input0 = "Meeting 30th"; // BUG! this comes up as 30th jan not curretn month
            String input1 = "Meeting 30th of December"; // BUG! this comes up as a date in the past
            String input2 = "Meeting the 30th of December 2025 at 8pm for dinner";
            String input3 = "Meeting 3:32 in the afternoon tomorrow";
            String input4 = "Meeting at 8:15pm"; //BUG! this dateTime returns as null


            string datecheck0 = await CreateTaskFromInput(input0);
            Debug.WriteLine("dateCheck0 = " + datecheck0);
            Debug.WriteLine(GetDescription(input0));

            string datecheck1 = await CreateTaskFromInput(input1);
            // 8/4 ** BUG ** PRIORITY: LOW
            // this date format is 1st of december
            // change this to suit australian date format or can we use user system location?
            Debug.WriteLine("dateCheck1 = " + datecheck1);
            Debug.WriteLine(GetDescription(input1));

            var datecheck2 = await CreateTaskFromInput(input2); // sets this as 8am
            Debug.WriteLine("dateCheck2 = " + datecheck2);
            Debug.WriteLine(GetDescription(input2));

            var datecheck3 = await CreateTaskFromInput(input3); //Meeting on the 29th of this month 3:32 in the afternoon
            Debug.WriteLine("dateCheck3 = " + datecheck3);
            Debug.WriteLine(GetDescription(input3));


            var datecheck4 = await CreateTaskFromInput(input4); // needs a valid day // time returns as am, not as next occerence
            Debug.WriteLine("dateCheck4 = " + datecheck4);
            Debug.WriteLine(GetDescription(input4));

            // 8/4 ** BUG ** PRIORITY: HIGH - this would be critcial as it is how a user would think of the date (not always putting in current year
            // this returns as null as its recognised as being in the past??? but if i put Jnauary, it recognises it, and updates to the current month?
            // need to check if there is a valid year of else year = current year
            // update: 9/4 - date/day/year is being set to 1 if ther is null value, which interferes with a january or 1st query...
            // perhaps check if there is january or 1st mentioned using regex or query the input string,
            //   and if there is then we can deal with it seprately to if the user didnt put a value in
            // check these first and modify string before checking date.
            //   ie if there is an Ordinal number but no month then adjust string to include the current month 
            //   ie if there is a time, but no day or no month, assume it is today
            //   ie if there is a time but no am or pm, then adjust to next occurence of this time
            //   ie if the string includes 8 in the morning or 8 in the evening make sure this works
            //   ie if the string include 8 oclock or just 8 , and the words, breakfast, lunch, brekkie, breaky, dinner, supper then adjust time to next occureence of that time
            // if the string includes ordinal 1st or a 1 with the word January, then we can assume its january 1st next year
            // if the string includes january, but nother ordinal or number then set the day to this number and the month to january,
            //   check if the current date is before this january date and if it is set the due Date to this year or else set it to next tear
            
        }
        private async void SaveData()
        {
            await TaskDataManager.SaveDataAsync();
            await FolderDataManager.SaveDataAsync();
        }
        private async Task UpdateData()
        {
            // this function will update the data after each change and save to the file
            SaveData();
            UpdateTaskIndexes();
            await Task.Delay(300);
        }
        private void UpdateTaskIndexes()
        {
            Tasks.UpdateTasksIndexes();
        }
        private async Task DisplaySortedIndex()
        {
            Debug.WriteLine("");
            Debug.WriteLine("...");
            Debug.WriteLine("DISPLAY ALL SORTED TASKS (by description) INDEX...");
            foreach (var task in Tasks.AllTasksList)
            {
                Tasks.TasksByDescriptionIndex.Add(task);
                Debug.WriteLine($"Description: {task.description}, Due Date: {task.dateDue}");
            }
            Debug.WriteLine("");
            await Task.Delay(100);
        }
        private async Task DisplayAllTasks()
        {
            // DISPLAY TASK INFORMATION IN CONSOLE
            Debug.WriteLine("");
            Debug.WriteLine("...");
            Debug.WriteLine("DISPLAY ALL TASKS FROM ALLTASKS LIST...");
            foreach (var task in Tasks.AllTasksList)
            {
                Debug.WriteLine(task.description);
            }
            Debug.WriteLine("");
            await Task.Delay(100);
        }
        private async Task DisplayAllFolders()
        {
            // DISPLAY FOLDER INFORMATION IN CONSOLE
            Debug.WriteLine("");
            Debug.WriteLine("...");
            Debug.WriteLine("DISPLAY ALL FOLDERS FROM ALLFOLDERS LIST...");

            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine(folder.name);
            }
            Debug.WriteLine("");
            await Task.Delay(100);
        }
        private async Task DisplayTasksInFolders()
        {
            // Verify that tasks were added to the folders then display them to the Debug Console
            //Debug.WriteLine(Folder.AllFoldersList.Count());
            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine($"\n FOLDER: {folder.name}");
                Debug.WriteLine($"Displaying Tasks in folder {folder.name} = {folder.taskId.Count} : \n");
                foreach (var taskId in folder.taskId)
                {
                    var task = Tasks.GetTaskById(taskId);
                    Debug.WriteLine($"- {task.description}");
                    //Debug.WriteLine($"- {task.id}");
                    //Debug.WriteLine($"- {task.dateDue}");
                    Debug.WriteLine($"- {task.notes}");
                }
            }
            Debug.WriteLine("");
            await Task.Delay(100);

        }
        public void SearchByDescription()
        {
            // Search Tasks for a given description
            Debug.WriteLine("*******************************************************");
            Debug.WriteLine("SEARCHING BY DESCRIPTION");
            var searchedTasksByDescription = Tasks.SearchByDescription("Exercise");
            foreach (var task in searchedTasksByDescription)
            {
                // Display the sorted tasks in order highest to lowest
                Debug.WriteLine($"{task.description} \n {task.dateDue}");
            };
        }
        public void SearchByTodaysDate()
        {
            // Search for tasks with a due date of Today
            Debug.WriteLine("*******************************************************");
            Debug.WriteLine("SEARCHING BY TODAYS DATE");

            DateTime date = DateTime.Today;
            List<Tasks> searchedTasksByTodaysDate = Tasks.SearchByDueDate(date);
            Debug.WriteLine("search 1 ... tasks for TODAY...");
            foreach (var task in searchedTasksByTodaysDate)
            {
                // Display the sorted tasks in order highest to lowest
                Debug.WriteLine($"{task.description} \n {task.dateDue}");
            };
        }
        public void SearchByDate(string date)
        {
            // Search for tasks with a given due date
            Debug.WriteLine("*******************************************************");
            Debug.WriteLine("SEARCHING BY DATE");
            DateTime _date;
            var isValidDate = DateTime.TryParse(date, out _date);
            if (isValidDate)
            {
                //Debug.WriteLine(_date);
                List<Tasks> searchedTasksByDate = Tasks.SearchByDueDate(_date); // pass in search date
                Debug.WriteLine($"search by date ...{date}");
                foreach (var task in searchedTasksByDate)
                {
                    // Display the sorted tasks in order highest to lowest
                    Debug.WriteLine($"{task.description} \n {task.dateDue}");
                };
            }

            else
            {
                Console.WriteLine($"{date} is not a valid date string");
                throw new NotImplementedException();
            }
        }
        public Guid TestAddTask()
        {
            // Add a task
            Debug.WriteLine($"*************************************************");
            var taskAdded = new Tasks();
            taskAdded.description = "Added Task";
            taskAdded.notes = "notes...";
            taskAdded.dateDue = DateTime.Now;
            taskAdded.isCompleted = false;
            Tasks.AddTask(taskAdded);

            Guid idToLookUp = taskAdded.id;
            Debug.WriteLine($"\n Added Task '{taskAdded.description}' with ID: {idToLookUp}");
            Debug.WriteLine($"*************************************************");
            return idToLookUp;
        }
        public void TestRemoveTask(Guid idToLookup)
        {
            // delete a task
            Debug.WriteLine($"*************************************************");
            Guid id = idToLookup;

            Tasks.RemoveTask(id);
            Debug.WriteLine($"\n Removed Task with ID: {id}");
            Debug.WriteLine($"*************************************************");
        }
        public Folder TestAddFolder()
        {
            // Test adding a folder
            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("TEST: ADDING A FOLDER");

            var folderAdded = new Folder("Added Folder");
            Folder.AddFolder(folderAdded);
            return folderAdded;
        }
        public void TestRemoveFolder(Folder folderAdded)
        {
            // delete a folder
            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("TEST: REMOVING A FOLDER");

            Folder.RemoveFolder(folderAdded);
        }


        /// <summary>
        /// Method to take input from the user, parse the input and check for dates, names, places and a description
        /// TODO: check for date and save it if there is one else date = null
        /// TODO: split the input into tokens and remove words like if when as etc and return the description
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CreateTaskFromInput(string input)
        {
            // TEST CODE: using Microsoft Recognizers nuget packages, and regex.
            // Research:
            // https://github.com/microsoft/Recognizers-Text/tree/master/.NET/Samples
            // https://github.com/microsoft/Recognizers-Text/blob/master/.NET/Samples/SimpleConsole/Program.cs
            // https://csharp.hotexamples.com/examples/-/DateTimeRecognizer/-/php-datetimerecognizer-class-examples.html#google_vignette
            // https://starbeamrainbowlabs.com/blog/article.php?article=posts%2F325-AI-Microsoft-Text-Recognizers.html
            // https://stackoverflow.com/questions/52593835/parsing-timex-expressions-in-net-core

            // NOTES:
            // *** Ordinal number recognizer will find any ordinal number
            //          E.g "eleventh" will return "11".

            // *** Datetime recognizer will find any Date even if its write in coloquial language 
            //          E.g "I'll go back 8pm today" will return "2017-10-04 20:00:00"

            var culture = Culture.English;
            var results = DateTimeRecognizer.RecognizeDateTime(input, culture);

            // Solution help from https://github.com/microsoft/Recognizers-Text/issues/2680
            // Check if there are no results or if there are no valid dateTimes
            if (results.Count <= 0 || !results.First().TypeName.StartsWith("datetimeV2"))
            {
                await Task.Delay(200);
                Debug.WriteLine("No DateTimes found!");
                return null;
            }

            // The DateTime model can return several resolution types
            // https://github.com/Microsoft/Recognizers-Text/blob/master/.NET/Microsoft.Recognizers.Text.DateTime/Constants.cs#L7-L14
            // We only want those with a date, date and time, or date time period: 

            var first = results.First();
            var resolutionValues = (IList<Dictionary<string, string>>)first.Resolution["values"];
            var subType = first.TypeName.Split('.').Last();
            DateTime currentDate = DateTime.Now;
            DateTime midYear = new DateTime(currentDate.Year, 6, 1);

            if (subType.Contains("date") && !subType.Contains("range"))
            {
                // a date (or date & time) or multiple 
                var moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();

                // If the year is not specified, use the current year
                if (moment.Year == 1)
                    // And if the input does not contain jan or january
                    if (!input.ToLower().Contains("jan") && !input.ToLower().Contains("January"))
                    {
                        moment = new DateTime(currentDate.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, moment.Second);
                    }
                    else
                    {
                        // Set the year nto next year
                        moment = new DateTime(currentDate.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, moment.Second);
                        moment.AddYears(1);
                    }

                // If the month is not specified, use the current month
                if (moment.Month == 1)
                    // And if the input does not contain jan or january
                    if (!input.ToLower().Contains("jan") && !input.ToLower().Contains("January"))
                    {
                        //  set month to current month
                        moment = new DateTime(moment.Year, currentDate.Month, moment.Day, moment.Hour, moment.Minute, moment.Second);
                    }

                // If the day is not specified, use the current day
                if (moment.Day == 1)
                {
                    moment = new DateTime(moment.Year, moment.Month, DateTime.Now.Day, moment.Hour, moment.Minute, moment.Second);
                }

                // If only the time is specified, use the current date
                // *** BUG 10/4 this does not work as intended
                if (moment.Day == 1 && moment.Month == 1)
                {
                    moment = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, moment.Hour, moment.Minute, moment.Second);
                }

                if (moment < DateTime.Now)
                {
                    // a future moment is valid past moment is not 
                    await Task.Delay(200);
                    Debug.WriteLine("Exception! Cant use a date from the past!");
                    return null;
                }
                return moment.ToString();
            }

            await Task.Delay(100);
            Debug.WriteLine("end of function");
            return null;
        }


        /// <summary>
        /// This Method splits the input string into tokens using regex
        /// removes any stop words (common words like "a", "the", "with", etc.) 
        /// Then joins the remaining tokens back into a string.
        /// https://www.geeksforgeeks.org/write-regular-expressions/#google_vignette
        /// https://www.tutorialspoint.com/Initializing-HashSet-in-Chash
        /// https://www.geeksforgeeks.org/hashset-in-c-sharp-with-examples/
        /// https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.split?view=net-8.0
        /// https://www.dotnetperls.com/regex-split
        /// https://thedeveloperblog.com/c-sharp/regex-split
        /// file:///C:/Users/xcal1/Downloads/Regular%20expressions%20quick%20reference.pdf
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetDescription(string input)
        {
            // create a hashset with some stop words we need to remove
            var stopWords = new HashSet<string> { "a", "an", "and", "the", "at", "with", "for", "on", "in", "to" };
            // split the input and remove the stop words, then re-join these in a string for use as the description
            var tokens = Regex.Split(input, @"\W+")
                .Where(token => !stopWords.Contains(token.ToLower())).ToList();
            return string.Join(" ", tokens);
        }
    }
}


