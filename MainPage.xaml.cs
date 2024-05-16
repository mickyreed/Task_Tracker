using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;

using System.ComponentModel.Design;
using Windows.Services.Maps.LocalSearch;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Runtime.CompilerServices;
using System.Globalization;
using Microsoft.Recognizers.Text.NumberWithUnit.Chinese;
using Microsoft.Data.Sqlite;
using static TaskList.Folder;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Microsoft.Recognizers.Text.Matcher;
using System.Xml.Linq;
using TaskList;
using Windows.Graphics;
using Windows.UI.Xaml.Controls.Primitives;

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
        // List that we can bind to for the UI
        public static ObservableCollection<Folder> FoldersList = new ObservableCollection<Folder>();
        public string SelectedFolderName { get; set; }
        private Folder currentFolder;
        bool dataLoaded = false;
        
        /// <summary>
        /// The Apps Main Page which has an aside with a folder listview in it, 
        /// and a main section with an input box which takes a task List input by user
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            if (!dataLoaded)
            {
                LoadData();
                
                //If there is no existing folders or data then load default values
                if (Folder.AllFoldersList.Count == 0 && Tasks.AllTasksList.Count == 0)
                {
                    DefaultDataInitialiser.LoadDefaultData();
                }

            }
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshFolderList();  // Refresh folder list when navigating back

            if (e.Parameter is Folder returnedFolder)
            {
                UpdateUIWithReturnedFolder(returnedFolder);
            }
        }

        private void UpdateUIWithReturnedFolder(Folder folder)
        {
            // Update the UI based on the returned folder
            // For example, select the folder in the ListView
            FoldersListView.SelectedItem = folder;
            SelectedFolderName = folder.Name;
            // Update the TextBox text with the selected folder name
            UpdateFolderNameTextBox();
        }

        /// <summary>
        /// Method to call the Loading of  Task and Folder Data from storage
        /// </summary>
        private async void LoadData()
        {
            dataLoaded = true;

            await TaskDataManager.LoadDataAsync();
            await FolderDataManager.LoadDataAsync();
            await TaskDataManagerSQL.InitialiseDatabase();
            await FolderDataManagerSQL.InitialiseDatabase();
            await TaskDataManagerSQL.LoadDataBaseAsync();
            await FolderDataManagerSQL.LoadDataBaseAsync();

            FoldersList.Clear(); // clear the existing folder UI list
           
            foreach (var folder in Folder.AllFoldersList)
            {
                //Debug.WriteLine(folder.Name);
                FoldersList.Add(folder);
                Debug.WriteLine(folder.Name);
                //Debug.WriteLine($"***!!!***     {folder.Name}       ***!!!***");
            }
            FoldersListView.ItemsSource = FoldersList;
            //FoldersListView.ItemsSource = Folder.AllFoldersList;
            // Call this method after data loading is completed
            DataLoaded();
            
            await Task.Delay(100);

        }

        private void RefreshFolderList()
        {
            // Clear existing list
            //FoldersListView.ItemsSource = Folder.AllFoldersList; ;
            // Repopulate list with latest data
            
            FoldersListView.ItemsSource = FoldersList;
        }

        /// <summary>
        /// Function called after data is loaded runs input Tests and create Task from input Methods
        /// </summary>
        private async void DataLoaded()
        {
            RefreshFolderList();
        }

        /// <summary>
        /// Functiomn to save data from Tasks and Files to file in Binary format
        /// </summary>
        private async void SaveData()
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
            SaveData();
            UpdateTaskIndexes();
            //await DisplayAllFolders();
            //await Task.Delay(300);
        }

        /// <summary>
        /// Function to call updates for Task Indexes
        /// </summary>
        private void UpdateTaskIndexes()
        {
            Tasks.UpdateTasksIndexes();
        }

        /// <summary>
        /// Function to display the Sorted Indexes
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Function to Display all Tasks
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Function to display all Folders in debug console
        /// </summary>
        /// <returns></returns>
        private async Task DisplayAllFolders()
        {
            //FoldersList.Clear(); // clear the existing folder UI list
            RefreshFolderList();
            // DISPLAY FOLDER INFORMATION IN CONSOLE
            //Debug.WriteLine("");
            //Debug.WriteLine("...");
            //Debug.WriteLine("DISPLAY ALL FOLDERS FROM ALLFOLDERS LIST...");

            //Debug.WriteLine("");
            await Task.Delay(100);
        }


        /// <summary>
        /// Function to Verify that tasks were added to the folders then display them to the Debug Console
        /// </summary>
        /// <returns></returns>
        private async Task DisplayTasksInFolders()
        {
            //Debug.WriteLine(Folder.AllFoldersList.Count());
            foreach (var folder in Folder.AllFoldersList)
            {
                Debug.WriteLine($"\n FOLDER: {folder.Name}");
                Debug.WriteLine($"Displaying Tasks in folder {folder.Name} = {folder.taskId.Count} : \n");
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

        /// <summary>
        /// Function to Search Tasks for a given description
        /// </summary>
        public void SearchByDescription()
        {
            Debug.WriteLine("*******************************************************");
            Debug.WriteLine("SEARCHING BY DESCRIPTION");
            var searchedTasksByDescription = Tasks.SearchByDescription("Exercise");
            foreach (var task in searchedTasksByDescription)
            {
                // Display the sorted tasks in order highest to lowest
                Debug.WriteLine($"{task.description} \n {task.dateDue}");
            };
        }

        /// <summary>
        /// Function to Search for tasks with a due date of Today
        /// </summary>
        public void SearchByTodaysDate()
        {
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

        /// <summary>
        /// Function to Search for tasks with a given due date
        /// </summary>
        /// <param name="date"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SearchByDate(string date)
        {
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

        /// <summary>
        /// Function to Add a Task
        /// </summary>
        /// <returns></returns>
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

            //Delete Task from SQL Database
            _ = TaskDataManagerSQL.AddTaskAsync(taskAdded);

            Guid idToLookUp = taskAdded.id;
            Debug.WriteLine($"\n Added Task '{taskAdded.description}' with ID: {idToLookUp}");
            Debug.WriteLine($"*************************************************");
            return idToLookUp;
        }

        /// <summary>
        /// Function to delete a task
        /// </summary>
        /// <param name="idToLookup"></param>
        public async void TestRemoveTask(Guid idToLookup)
        {
            Debug.WriteLine($"*************************************************");
            Guid id = idToLookup;

            Tasks.RemoveTask(id);

            //Delete Task from SQL Database
            await TaskDataManagerSQL.DeleteTaskByIdAsync(id);


            Debug.WriteLine($"\n Removed Task with ID: {id}");
            Debug.WriteLine($"*************************************************");
        }

        #region Create Tasks from Input
        /// <summary>
        /// Method to parse the input, and clean it checking for dates, ordinals and specific words or chars
        /// Using Microsoft.Recognizers Nuget packages, and regex.
        /// References:
        /// https://github.com/microsoft/Recognizers-Text/tree/master/.NET/Samples
        /// https://starbeamrainbowlabs.com/blog/article.php?article=posts%2F325-AI-Microsoft-Text-Recognizers.html
        /// https://stackoverflow.com/questions/52593835/parsing-timex-expressions-in-net-core
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<(string, DateTime?)> CreateTaskDataFromInput(string input)
        {
            //Debug.WriteLine("1. In Task");
            var culture = Culture.English;
            var results = DateTimeRecognizer.RecognizeDateTime(input, culture);

            // Create a new trimmed string and remove all the detected date/time, ordinal and Specific Joining Words or char entities
            var cleanedInput = input.Trim();
            foreach (var entity in results)
            {
                cleanedInput = Regex.Replace(cleanedInput, Regex.Escape(entity.Text), "", RegexOptions.IgnoreCase);
            }

            // Solution help from https://github.com/microsoft/Recognizers-Text/issues/2680
            // Check if there are no results or if there are no valid dateTimes
            if (results.Count <= 0 || !results.First().TypeName.StartsWith("datetimeV2"))
            {
                await Task.Delay(200);
                Debug.WriteLine("No DateTimes found!");
                return (cleanedInput, null); ;
            }

            var first = results.First();
            var resolutionValues = (IList<Dictionary<string, string>>)first.Resolution["values"];
            var subType = first.TypeName.Split('.').Last();
            DateTime currentDate = DateTime.Now;

            // if the DateTime contains a date (not a range)
            if (subType.Contains("date") && !subType.Contains("range"))
            {
                // try to parse in the date and catch it if the date is invalid or out of range
                try
                {
                    //Debug.WriteLine("2. In SubType date");
                    // a date (or date & time) or multiple 
                    DateTime moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();

                    DateTime checkedMoment = CheckAndAdjustMoment(input, moment, currentDate);

                    Debug.WriteLine($"*************************************************");
                    Debug.WriteLine($"Input = {input}");
                    Debug.WriteLine($"Cleaned Input = {cleanedInput}");
                    Debug.WriteLine($"Adjusted DateTime = {checkedMoment}");

                    return (cleanedInput, checkedMoment);
                }
                catch (FormatException)
                {
                    Debug.WriteLine("Handled invalid Date");
                    return (cleanedInput, null);
                }
            }

            // if the date contains a time only, not a recognised date
            else if (subType.Contains("time"))
            {
                //Debug.WriteLine("3. In Sub Type Time");
                try
                {
                    var moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();

                    DateTime checkedMoment = AdjustMomentForEvent(input, moment);

                    moment = checkedMoment;

                    return (cleanedInput, moment);
                }
                catch (FormatException)
                {
                    //Debug.WriteLine("4. In Catch");
                    Debug.WriteLine("Handled invalid Time");
                    return (cleanedInput, currentDate);
                }
            }

            await Task.Delay(100);
            Debug.WriteLine("reached end of function return current date");
            return (cleanedInput, currentDate);
        }

        /// <summary>
        /// This function adjusts the Task datetime, to make sure it is not in a previous year,
        /// It will add the difference in years to make it this year
        /// and then recheck and make sure the adjusted date is not earlier in the year, 
        /// if it is it and the date is greater than 7 days earlier,
        /// It will add a year to make it next years event
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        private DateTime CheckAndAdjustForYear(DateTime moment, DateTime currentDate)
        {
            //Debug.WriteLine("3.5. In CheckAndAdjustYear");

            DateTime adjustedMoment = moment;

            if (moment.Year <= currentDate.Year)
            {
                int currentYear = currentDate.Year;
                int momentYear = moment.Year;
                int yearDifference = currentYear - momentYear;

                //Debug.WriteLine($"....yearDifference = {yearDifference}");

                // Check if the date is last year?
                if (yearDifference > 0)
                {

                    // if it is add the difference to get it to the current year
                    // update the moment as a new DateTime ( as DateTime is immutable)
                    adjustedMoment = adjustedMoment.AddYears(yearDifference);
                    //Debug.WriteLine($"....adjustedMoment = {adjustedMoment}");
                }

                // ReCheck if the date is more than 7 days ago by checking the difference in days
                int dateDifferenceInDays = (currentDate.Date - adjustedMoment.Date).Days;
                //Debug.WriteLine($"....DifferenceinDays = {dateDifferenceInDays}");

                if (dateDifferenceInDays > 7)
                {
                    // if it more than 7 days assume date is in the future
                    // update the moment as a new DateTime ( as DateTime is immutable)
                    adjustedMoment = adjustedMoment.AddYears(1);
                    //Debug.WriteLine(adjustedMoment.ToString());

                }
            }
            return adjustedMoment;
        }

        /// <summary>
        /// This function adjusts a Task date for if a user inputs "fortnight"
        /// NOTE: "fortnight at 6pm" will not work, it needs to reference a day
        /// such as tomorrow fortnight at 6 or similar
        /// </summary>
        /// <param name="input"></param>
        /// <param name="moment"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        private DateTime CheckAndAdjustForFortnight(string input, DateTime moment, DateTime currentDate)
        {
            if (moment.Date < currentDate.AddDays(-3) && input.ToLower().Contains("fortnight"))
            {
                // add 21 days
                moment = moment.AddDays(14);
                return moment;
            }
            else if (moment.Date >= currentDate.AddDays(-3) && input.ToLower().Contains("fortnight"))
            {
                // add 14 days
                moment = moment.AddDays(14);
            }
            return moment;
        }

        /// <summary>
        /// Function to check the DateTime moment and adjust it in reference to our current date
        /// this function makes calls to other functions to adjust for Year, Fortnight, and events
        /// </summary>
        /// <param name="input"></param>
        /// <param name="moment"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        private DateTime CheckAndAdjustMoment(string input, DateTime moment, DateTime currentDate)
        {
            DateTime checkedYear = CheckAndAdjustForYear(moment, currentDate);
            DateTime checkedFortnight = CheckAndAdjustForFortnight(input, checkedYear, currentDate);
            DateTime adjustedMoment = checkedFortnight;

            //if the date is less than the current date(excluding time) then make it next week
            if (!input.ToLower().Contains("fortnight") && !input.ToLower().Contains("next"))
            {
                if (adjustedMoment.Date < currentDate.Date &&
                input.ToLower().Contains("monday") ||
                input.ToLower().Contains("tuesday") ||
                input.ToLower().Contains("wednesday") ||
                input.ToLower().Contains("thursday") ||
                input.ToLower().Contains("friday") ||
                input.ToLower().Contains("saturday") ||
                input.ToLower().Contains("sunday"))
                {
                    adjustedMoment = adjustedMoment.AddDays(7);
                }
            }

            DateTime adjustedEvent = AdjustMomentForEvent(input, adjustedMoment);
            moment = adjustedEvent;
            return moment;
        }

        /// <summary>
        /// Function to adjust the dateTime and Time based on the Event input
        /// ie breakfast, Lunch, DInner, Afternoon, Evening etc
        /// </summary>
        /// <param name="input"></param>
        /// <param name="moment"></param>
        /// <returns></returns>
        private static DateTime AdjustMomentForEvent(string input, DateTime moment)
        {
            //Debug.WriteLine("4. In AdjustMomentForEvent");
            DateTime adjustedMoment = moment;
            //Debug.WriteLine("moment is: " + moment.Hour.ToString());
            //if the input if for dinner
            if (moment.Hour > 4 && moment.Hour < 12)
            {
                if (input.ToLower().Contains("dinner") || input.ToLower().Contains("tonight"))
                {
                    //Debug.WriteLine("DINNER MOMENT");
                    adjustedMoment = CheckAndUpdateTime(moment);
                    return adjustedMoment;
                }
            }

            // if the moment Time is in the morning and they want lunch
            if (moment.Hour >= 0 & moment.Hour < 5)
            {
                if (input.ToLower().Contains("lunch"))
                {
                    //Debug.WriteLine($"LUNCH ADD 12 hrs:   {moment.Date}");
                    //adjustedMoment = moment.AddHours(12);
                    adjustedMoment = CheckAndUpdateTime(moment);
                    return adjustedMoment;
                }
            }

            // if the current Time is past the morning and they want breakfast
            if (DateTime.Now > DateTime.Today.AddHours(12) && moment.Date == DateTime.Today)
            //if (moment.Hour > 0 && moment.Hour < 12)
            {
                if (input.ToLower().Contains("breakfast") && (moment.Hour > 0 && moment.Hour < 12) && (!input.ToLower().Contains("tomorrow")))
                {
                    //Debug.WriteLine($"BREAKFAST TOO LATE ADD 24 hrs:   {moment.Date}");
                    //adjustedMoment = moment.AddHours(12);
                    adjustedMoment = moment.AddDays(1);
                    //adjustedMoment = CheckAndUpdateTime(moment);
                    return adjustedMoment;
                }
                else if (input.ToLower().Contains("breakfast") && (!input.ToLower().Contains("tomorrow")))
                {
                    adjustedMoment = moment.AddHours(12);
                    //adjustedMoment = CheckAndUpdateTime(moment);
                    return adjustedMoment;
                }
            }

            // else if moment if for afternoon
            if ((moment.Hour > 0 && moment.Hour < 7)
                && (input.ToLower().Contains("afternoon")))
            {
                //Debug.WriteLine("afternoon check");
                adjustedMoment = CheckAndUpdateTime(moment);
                return adjustedMoment;
            }

            else if (moment.TimeOfDay < DateTime.Now.TimeOfDay)
            {
                //Debug.WriteLine("time of day check");
                adjustedMoment = CheckAndUpdateTime(moment);
                return adjustedMoment;
            }

            //Debug.WriteLine("end of adjusted moment for event");
            return adjustedMoment;
        }

        /// <summary>
        /// A function to update any default time values by adding 12 hours to adjust for AM/PM
        /// and then rechecking if it is still prior to the current time, and if it is add 1 day
        /// to effectively make it tomorrows event
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        private static DateTime CheckAndUpdateTime(DateTime moment)
        {
            //Add 12 hours to the moment Time
            DateTime adjustedMoment = moment.AddHours(12);

            // check if the moment time is still before the current time
            if (adjustedMoment < DateTime.Now)
            {
                //Debug.WriteLine($"ADDING 24 hrs:   {adjustedMoment.Date}");
                // Time is in the past add 24hrs
                adjustedMoment = adjustedMoment.AddDays(1);

            }
            return adjustedMoment;
        }

        /// <summary>
        /// Function which checks the input for various keywords: 
        /// morning, breakfast, lunch, dinner, afternoon, evening, tomorrow
        /// which can affect the am or pm and makes a judgement call on when the user meant the tme to be
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        //private static DateTime AdjustDateTime(string input, DateTime dateTime)
        //{
        //    int yearAdjustment = dateTime.Year - DateTime.Now.Year;
        //    // Parse the time from the input string using regular expression
        //    Match match = Regex.Match(input, @"\b(\d{1,2})\b");
        //    if (!match.Success)
        //    {
        //        // If no time is found, return original dateTime
        //        return dateTime;
        //    }

        //    int hour = int.Parse(match.Groups[1].Value);

        //    // Check for keywords indicating specific times of the day
        //    if (input.ToLower().Contains("morning") || input.ToLower().Contains("breakfast"))
        //    {
        //        if ((DateTime.Now.AddYears(yearAdjustment) <= dateTime))
        //        {
        //            // If time is after the specified datetime keep it as it is
        //            return dateTime;
        //        }
        //        else
        //        {
        //            // Otherwise, set the time to the next morning
        //            return dateTime.AddHours(24);
        //        }
        //    }
        //    else if (input.ToLower().Contains("lunch"))
        //    {
        //        if (DateTime.Now.AddYears(yearAdjustment) <= dateTime && dateTime.Hour >= 10.30 && dateTime.Hour <= 2.30)
        //        {
        //            // If time is currently before the set time keep it on same afternoon
        //            return dateTime.AddHours(12);
        //        }
        //        else
        //        {
        //            // Otherwise, set the time to the afternoon of next day (24 + 12)
        //            return dateTime.AddHours(24);
        //        }
        //    }
        //    //else if (input.ToLower().Contains("afternoon"))
        //    //{
        //    //    if (DateTime.Now <= dateTime && dateTime.Hour >= 12.00 && dateTime.Hour < 6.00)
        //    //    {
        //    //        // If time is currently before the set time keep it on same afternoon
        //    //        return dateTime.AddHours(12);
        //    //    }
        //    //    else
        //    //    {
        //    //        // Otherwise, set the time to the afternoon of next day (24 + 12)
        //    //        return dateTime.AddHours(24);
        //    //    }
        //    //}
        //    else if (input.ToLower().Contains("evening") || input.ToLower().Contains("dinner"))
        //    {
        //        if (DateTime.Now < dateTime.AddHours(12))
        //        {
        //            // If the current evening time is less than the input time
        //            return dateTime.AddHours(12);
        //        }
        //        else
        //        {
        //            // Otherwise, set the time to the following evening (12 + 24)
        //            return dateTime.AddHours(36);
        //        }
        //    }
        //    //else if (input.ToLower().Contains("tomorrow") || input.ToLower().Contains("tomorow"))
        //    //{
        //    //    if (DateTime.Now.Day == dateTime.Day)
        //    //    {
        //    //        // If the current day is less than tomorrow add 24hrs
        //    //        return dateTime.AddHours(24);
        //    //    }
        //    //    else
        //    //    {
        //    //        // Otherwise, return the DateTime as is
        //    //        return dateTime;
        //    //    }
        //    //}
        //    else
        //    {
        //        //if the dateTime.Now means that the next available occurence is in the evening add 12hrs
        //        if (DateTime.Now < dateTime.AddHours(12) && DateTime.Now > dateTime)
        //        {
        //            return dateTime.AddHours(12);
        //        }
        //        //if the dateTime.Now means that the next available occurence is tomorrow, add 24hrs 
        //        else if (DateTime.Now > dateTime.AddHours(12))
        //        {
        //            return dateTime.AddHours(24);
        //        }
        //        // By default, consider times mentioned without keywords as next available occurence
        //        if (DateTime.Now < dateTime)
        //        {
        //            return dateTime;
        //        }
        //    }

        //    return dateTime;
        //}

        /// <summary>
        /// This Method splits the input string into tokens using regex
        /// removes any stop words (common words like "a", "the", "with", etc. as required) 
        /// Then joins the remaining tokens back into a string.
        /// https://www.geeksforgeeks.org/write-regular-expressions/#google_vignette
        /// https://www.tutorialspoint.com/Initializing-HashSet-in-Chash
        /// https://www.geeksforgeeks.org/hashset-in-c-sharp-with-examples/
        /// https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.split?view=net-8.0
        /// https://www.dotnetperls.com/regex-split
        /// https://thedeveloperblog.com/c-sharp/regex-split
        /// </summary>
        /// <param name="cleanedInput"></param>
        /// <returns></returns>
        private static string GetDescription(string cleanedInput)
        {
            // create a hash set with some stop words we need to remove
            var stopWords = new HashSet<string> { "a", "at", "an", "the", "for", "on", "in", "to", "at" }; //"with", "and",  (maybe keep this for say "meeting with Bob"
            // split the input and remove the stop words, then re-join these in a string for use as the description
            var tokens = Regex.Split(cleanedInput, @"\W+")
                .Where(token => !stopWords.Contains(token.ToLower())).ToList();
            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Function to get a string tuple of the cleaned description,
        /// and the date as a string and display it in the UI
        /// </summary>
        /// <param name="userInput"></param>
        public async Task<string> CheckUserInput(string userInput)
        {
            (string cleanedInput, DateTime? dateTime) = await CreateTaskDataFromInput(userInput);

            cleanedInput = cleanedInput.Trim(); //trim the input
            cleanedInput = GetDescription(cleanedInput); // call to remove any unwanted stop words from description
            string dateTimeToDisplay;
            cleanedInput = GetDescription(cleanedInput); // second clean for extra words (resolves extra "at"

            CreateTaskFromTaskData(cleanedInput, userInput, dateTime);

            if (dateTime == null)
            {
                dateTimeToDisplay = "None";
            }
            else
            {
                dateTimeToDisplay = dateTime.ToString();
            }
            //Debug.WriteLine("... " + userInput + " returns: \n" + cleanedInput + "\n" + dateTime);
            string output = ($"TASK: {cleanedInput} \nNOTES: {userInput} \nDUE DATE: {dateTimeToDisplay}");


            return output;
        }

        /// <summary>
        /// A function which will take the adjusted DateTime and input Data and create a Task Object
        /// </summary>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <param name="dateDue"></param>
        public void CreateTaskFromTaskData(string description, string notes, DateTime? dateDue)
        {
            string format = "dd/MM/yyyy h:mm:ss tt"; // Specify the exact format of dueDate parameter string to convert to new DateTime
            DateTime? convertedDateTime = null;

            if (dateDue != null)
            {

                string dateString = dateDue.Value.ToString(format);
                try
                {
                    convertedDateTime = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    // Handle the parsing of incomplete or null date formats
                    // these will occur if the user enters a Task without a date
                    Debug.WriteLine("Null or Invalid date format Handled.");
                }
            }

            // Add a task from the User Input
            //Debug.WriteLine($"*************************************************");
            var taskAdded = new Tasks();
            taskAdded.description = description;
            taskAdded.notes = notes;
            taskAdded.dateDue = convertedDateTime;
            taskAdded.isCompleted = false;
            Tasks.AddTask(taskAdded);
            //Folder1.AddTask(taskAdded.id);
            Folder selectedFolder = CheckCurrentSelectedFolder(SelectedFolderName);
            if (selectedFolder != null)
            {
                selectedFolder.AddTask(taskAdded.id);
                Debug.WriteLine(taskAdded.description, selectedFolder.Name);
                DisplayTasksInFolders();
            }

            DisplayTasksInFolders();




            // ***   ADD TASK TO THE SQL DATABASE   ***
            _ = TaskDataManagerSQL.UpdateDataBaseAsync(taskAdded);

            Guid idToLookUp = taskAdded.id;
            Debug.WriteLine($"\nAdded Task " +
                $"\nDescription: {taskAdded.description} " +
                $"\nID: {idToLookUp}" +
                $"\nNotes: {taskAdded.notes} " +
                $"\nDate Due: {dateDue}");

            Debug.WriteLine($"*************************************************");

        }
        #endregion

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
                    string output = await CheckUserInput(userInput);

                    ResultTextBlock.Text = output;
                    InputTextBox.Text = string.Empty;
                }
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

        /// <summary>
        /// Event Handler to check for text changes in the Text Box and block any math symbols
        /// Reference: https://claude.ai/chat/ab0b5a8f-3b82-4249-bf1b-8677e5b5667a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            // Define a regular expression pattern to match math symbols
            string pattern = @"[+\-*/=^%]";
            Regex regex = new Regex(pattern);

            // Check if the new input text contains any math symbols
            if (regex.IsMatch(textBox.Text))
            {
                // Revert the text to the previous value
                textBox.Text = textBox.Text.TrimEnd(regex.Matches(textBox.Text).Last().Value.ToCharArray());
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        /// <summary>
        /// An event handler that checks if the hamburger icon is clicked to open the aside menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }



        private void FoldersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Retrieve the clicked folder
            SelectedFolderName = e.ClickedItem as string;

            // Update the TextBox text with the selected folder name
            UpdateFolderNameTextBox();

        }



        /// <summary>
        /// A function that Updates the TextBox text with the selected folder name
        /// </summary>
        private void UpdateFolderNameTextBox()
        {
            // Update the TextBox text with the selected folder name
            InputTextBox.PlaceholderText = "Add Task (to " + SelectedFolderName + ")";
        }

        /// <summary>
        /// A funcytion that Finds the Folder instance with the matching name
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private Folder CheckCurrentSelectedFolder(string folderName)
        {

            // Find the Folder instance with the matching name
            Folder selectedFolder = null;
            foreach (Folder folder in AllFoldersList)
            {
                if (folder.Name == SelectedFolderName)
                {
                    selectedFolder = folder;
                    return selectedFolder;
                    //break;
                }

            }
            return selectedFolder;
        }

        /// <summary>
        /// An event handler that checks for the listview items to change when a user selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Check if there is any selected item
            if (e.AddedItems.Count > 0)
            {
                // Get the selected folder (assuming each item in the ListView is a Folder instance)
                Folder selectedFolder = e.AddedItems[0] as Folder;

                if (selectedFolder != null)
                {
                    // Update the current folder
                    currentFolder = selectedFolder;

                    // Update UI or perform any other actions based on the selection
                    UpdateFolderNameTextBox(selectedFolder.Name);
                }
            }
        }

        /// <summary>
        /// A function that updates the placeholder text in the Enter a Task input Text box based on the folder selected
        /// </summary>
        /// <param name="selectedFolderName"></param>
        private void UpdateFolderNameTextBox(string selectedFolderName)
        {
            // Update the TextBox text with the selected folder name
            InputTextBox.PlaceholderText = "Add Task (to " + selectedFolderName + ")";
        }

        /// <summary>
        /// An event handler that opens up the TasksPage after a button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_View_Tasks(object sender, RoutedEventArgs e)
        {
            // Get the selected folder
            Button button = sender as Button;
            Folder selectedFolder = button?.Tag as Folder;

            if (selectedFolder != null)
            {
                //Folder selectedFolder = FoldersListView.SelectedItem as Folder;
                // Navigate to a new page passing the selected folder as a parameter
                Frame.Navigate(typeof(TasksPage), selectedFolder);
            }
        }


        private void CreateTaskButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }
        private void CreateTaskButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }
        private void MainSplitView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
        }
        


        /// <summary>
        /// An event handler that checks for a double click and opens up the TaskPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Folder selectedFolder = (sender as ListView).SelectedItem as Folder;
            if (selectedFolder != null)
            {
                // Pass the selected folder to the task view page
                Frame.Navigate(typeof(TasksPage), selectedFolder);
            }
        }

        /// <summary>
        /// Function that opens the Add Folder Pop Up window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            AddFolderPopup.IsOpen = true;
        }

        /// <summary>
        /// Function that deletes a folder if the user presses the Confrim button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFolderYes_Click(object sender, RoutedEventArgs e)
        {
            
            // Implement logic to delete the folder
            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("DELETING A FOLDER");
            //var folderToDelete = (Folder)((Button)sender).Tag;
            //Folder.RemoveFolder(folderToDelete);
            Folder.RemoveFolder(currentFolder);
            FoldersList.Remove(currentFolder);

            //Delete Folder from SQL Database
            _ = FolderDataManagerSQL.DeleteFolderByIdAsync(currentFolder.id);
            SaveData();

            // Close Popup after deletion
            DeleteFolderPopup.IsOpen = false;

        }

        /// <summary>
        /// An event handler that takes in a button press and adds a folder based on input name from the user
        /// then closes the dialogue box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FolderNameTextBox.Text;
            if (!string.IsNullOrEmpty(folderName))
            {
                var folderAdded = new Folder(folderName);
                Folder.AddFolder(folderAdded);
                FoldersList.Add(folderAdded);

                //Add Folder to SQL Database
                _ = FolderDataManagerSQL.AddFolderAsync(folderAdded);
                SaveData();
                //await UpdateData();

                // Close Popup after adding folder
                AddFolderPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// An event handler that takes in a button press and opens apopup window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Folder selectedFolder = button?.Tag as Folder;
            currentFolder = selectedFolder;
            DeleteFolderPopup.IsOpen = true;
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

/*
TODO:
Each task should have:
    a checkbox to mark whether the task is completed
    a small pencil icon which is visible only if the task has notes attached.
        Icon Added DONE
        Functionality - NOT TESTED YET
    Overdue tasks should be listed first and marked somehow. 
    Beneath them should be the tasks due today. 
    After that, list the remaining tasks. 
    Tapping on a task will take the user to a screen where they can view 
    and change all the details of the task. 
    The app should have some basic preferences, 
    They could be colour schemes, different ways of ordering the tasks, etc.
*/