using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Windows.UI.Popups;
using System.ComponentModel.Design;
using Windows.Services.Maps.LocalSearch;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Runtime.CompilerServices;
using System.Globalization;
using Microsoft.Recognizers.Text.NumberWithUnit.Chinese;
using Microsoft.Data.Sqlite;



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
        
        /// <summary>
        /// Method to call the Loading of  Task and Folder Data from storage
        /// </summary>
        private async void LoadData()
        {
            await TaskDataManager.LoadDataAsync();
            await FolderDataManager.LoadDataAsync();
            await TaskDataManagerSQL.InitialiseDatabase();
            await FolderDataManagerSQL.InitialiseDatabase();
            await TaskDataManagerSQL.LoadDataBaseAsync();
            await FolderDataManagerSQL.LoadDataBaseAsync();

            // Call this method after data loading is completed
            DataLoaded();
        }

        /// <summary>
        /// Function that includes Test Methods used in Testing
        /// </summary>
        /// <returns></returns>
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
            #region TASK 2 METHOD CALLS (depricated)
            // DisplayAllTasks();
            // Debug.WriteLine($"Number of folders loaded: {Folder.AllFoldersList.Count}");
            // DisplayTasksInFolders();
            // SearchByDescription();
            // SearchByTodaysDate();
            // SearchByDate("2024/03/15"); //date format must be "2008/04/14";
            // SearchByDate("2024/03/12");
            #endregion
            #region TASK 3 TEST METHOD CALLS
            //// TEST adding a folder
            //Folder folderTest = TestAddFolder();
            //await DisplayAllFolders();

            //// TEST adding a task
            //Guid testGuid = TestAddTask();
            //await Task.Delay(200);
            //await DisplayTasksInFolders();

            //// TEST update the index and save file
            //await UpdateData();
            //await Task.Delay(200);
            //await DisplaySortedIndex();

            ////TEST remove the Task
            //TestRemoveTask(testGuid);
            //await Task.Delay(200);
            //await DisplayTasksInFolders();

            //// TEST update the index and save file
            //await UpdateData();
            //await Task.Delay(200);
            //await DisplaySortedIndex();

            //// TEST: remove the folder
            //TestRemoveFolder(folderTest);
            //await DisplayAllFolders();

            //// TEST update the index and save file
            //await UpdateData();
            #endregion

            #region TASK 6 TEST METHOD CALLS
            Debug.WriteLine("");
            Debug.WriteLine("TASK MANAGER PART #6" +
                "\nTESTS FOR ADDING AND DELETING ITEMS FROM TABLES IN DATABASE");
            Debug.WriteLine("");
            // TEST #1 adding a folder
            Folder newfolder = TestAddFolder();
            //display contents of db folder table
            await FolderDataManagerSQL.QueryAllFoldersByName("Added Folder"); // expecting a Folder returned

            // TEST #2 adding a task
            Guid testGuid2 = TestAddTask();
            await Task.Delay(200);
            await TaskDataManagerSQL.QueryAllTasksByName("Added Task"); // expecting a Task returned
            //display contents of db task table
            await UpdateData();

            ////TEST #3 remove the Task
            TestRemoveTask(testGuid2);
            await Task.Delay(200);
            TaskDataManagerSQL.QueryAllTasksByName("Added Task"); // expecting no Task returned
            //display contents of db task table
            await UpdateData();

            // TEST #4 remove the folder
            TestRemoveFolder(newfolder);
            //display contents of db folder table
            await FolderDataManagerSQL.QueryAllFoldersByName("Added Folder"); // expecting no Folder returned

            await UpdateData();
            #endregion
        }

        /// <summary>
        /// Function called after data is loaded runs input Tests and create Task from input Methods
        /// </summary>
        private async void DataLoaded()
        {
            #region TESTS - dipslaying, add, delete both Folders and Tasks
            ////Method calls to carry out after data loading is completed
            //await UpdateData();
            //Debug.WriteLine("................................");
            //Debug.WriteLine(".... Folders & Tasks Loaded ....");
            //await DisplayTasksInFolders();
            //await DisplayAllFolders();
            //await DisplayAllTasks();
            //await DisplaySortedIndex();
            //Debug.WriteLine("................................");
            //Debug.WriteLine("");
            await RunTests();
            #endregion

            #region TASK 5 TESTS CASES FOR Testing Task Input Creation Method
            //// TEST DATA
            //String input0 = "Exercise at 9am on the 1st January 2025 Star Jumps and Leg Day";
            //String input1 = "Meeting 1st of May with Bob and Mary at HighGate Restaurant";
            //String input2 = "Meet Jenny the 30th of December 2025 at 8pm for dinner";
            //String input3 = "Meeting 3:32 in the afternoon tomorrow 12th street";
            //String input4 = "Meeting at 8:15pm";

            //String input5 = "Call Rob on Wednesday at 3PM";
            //String input6 = "Call Rob at 3PM on Wednesday";
            //String input7 = "Call Rob";
            //String input8 = "Call Rob, 3PM, Thursday";
            //String input9 = "Call Rob, Wednesday, 3PM";
            //String input10 = "Call Rob 3PM Wednesday";
            //String input11 = "Call Rob Wednesday 3AM";
            ////String input12 = "1/1/2025";

            ////string output12 = await CheckUserInput(input12);
            //string output0 = await CheckUserInput(input0);
            ////Debug.WriteLine($"{output0} \n");
            //string output1 = await CheckUserInput(input1);
            ////Debug.WriteLine($"{output1} \n");
            //string output2 = await CheckUserInput(input2);
            ////Debug.WriteLine($"{output2} \n");
            //string output3 = await CheckUserInput(input3);
            ////Debug.WriteLine($"{output3} \n");
            //string output4 = await CheckUserInput(input4);
            ////Debug.WriteLine($"{output4} \n");
            //string output5 = await CheckUserInput(input5);
            ////Debug.WriteLine($"{output5} \n");
            //string output6 = await CheckUserInput(input6);
            ////Debug.WriteLine($"{output6} \n");
            //string output7 = await CheckUserInput(input7);
            ////Debug.WriteLine($"{output7} \n");
            //string output8 = await CheckUserInput(input8);
            ////Debug.WriteLine($"{output8} \n");
            //string output9 = await CheckUserInput(input9);
            ////Debug.WriteLine($"{output9} \n");
            //string output10 = await CheckUserInput(input10);
            ////Debug.WriteLine($"{output10} \n");
            //string output11 = await CheckUserInput(input11);
            ////Debug.WriteLine($"{output11} \n");
            #endregion

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
            await Task.Delay(300);
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

        /// <summary>
        /// Function to Verify that tasks were added to the folders then display them to the Debug Console
        /// </summary>
        /// <returns></returns>
        private async Task DisplayTasksInFolders()
        {
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

            //Add Task to SQL Database
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

        /// <summary>
        /// Function to Test adding a folder
        /// </summary>
        /// <returns></returns>
        public Folder TestAddFolder()
        {
            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("TEST: ADDING A FOLDER");

            var folderAdded = new Folder("Added Folder");
            Folder.AddFolder(folderAdded);

            //Add Folder from SQL Database
            _ = FolderDataManagerSQL.AddFolderAsync(folderAdded);

            return folderAdded;
        }

        /// <summary>
        /// Function to delete a folder
        /// </summary>
        /// <param name="folderAdded"></param>
        public void TestRemoveFolder(Folder folderAdded)
        {
            Debug.WriteLine($"*************************************************");
            Debug.WriteLine("TEST: REMOVING A FOLDER");

            Folder.RemoveFolder(folderAdded);

            //Delete Folder from SQL Database
            _ = FolderDataManagerSQL.DeleteFolderByIdAsync(folderAdded.id);
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
        /// Function to adjust the dateTime & Time based on the Event input
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

            else {
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
    }
}