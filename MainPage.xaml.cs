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
            #region TESTS - dipslaying, add, delete both Folders and Tasks
            // Method calls to carry out after data loading is completed
            //await UpdateData();
            //Debug.WriteLine("................................");
            //Debug.WriteLine(".... Folders & Tasks Loaded ....");
            //await DisplayTasksInFolders();
            //await DisplayAllFolders();
            //await DisplayAllTasks();
            //await DisplaySortedIndex();
            //Debug.WriteLine("................................");
            //Debug.WriteLine("");
            //await RunTests();
            #endregion

            #region TESTS FOR Testing Task Input Creation Method
            ////String input0 = "Meeting 30th"; // BUG! Does not return a date at all
            //String input0 = "Exercise at 9am on the 31 January Star Jumps and Leg Day"; // BUG! this comes up as error - added try parse FIXED 11/4
            //String input1 = "Meeting 30th of May with Bob and Mary at HighGate Restaurant"; // BUG! this comes up as a date in the past
            //String input2 = "Meet Jenny the 30th of December 2025 at 8pm for dinner";
            //String input3 = "Meeting 3:32 in the afternoon tomorrow 12th street";
            //String input4 = "Meeting at 8:15pm"; //BUG! this dateTime returns as null FIXED WITH subtype time fix

            //(string cleanedInput, string dateTime0) = await CreateTaskFromInput(input0);
            //Debug.WriteLine(input0 + " returns: \n" + GetDescription(cleanedInput) + "\n" + dateTime0);

            //(string cleanedInput1, string dateTime1) = await CreateTaskFromInput(input1);
            //Debug.WriteLine(input1 + " returns: \n" + GetDescription(cleanedInput1) + "\n" + dateTime1);

            //(string cleanedInput2, string dateTime2) = await CreateTaskFromInput(input2);
            //Debug.WriteLine(input2 + " returns: \n" + GetDescription(cleanedInput2) + "\n" + dateTime2);

            //(string cleanedInput3, string dateTime3) = await CreateTaskFromInput(input3);
            //Debug.WriteLine(input3 + " returns: \n" + GetDescription(cleanedInput3) + "\n" + dateTime3);

            //(string cleanedInput4, string dateTime4) = await CreateTaskFromInput(input4);
            //Debug.WriteLine(input4 + " returns: \n" + GetDescription(cleanedInput4) + "\n" + dateTime4);

            //// 8/4 ** BUG ** PRIORITY: HIGH - this would be critcial as it is how a user would think of the date (not always putting in current year
            //// this returns as null as its recognised as being in the past??? but if i put Jnauary, it recognises it, and updates to the current month?
            //// RESOLVED: 10/4need to check if there is a valid year of else year = current year
            //// RESOLVED: 10/4 update: 9/4 - date/day/year is being set to 1 if ther is null value, which interferes with a january or 1st query...
            //// perhaps check if there is january or 1st mentioned using regex or query the input string,
            ////   and if there is then we can deal with it seprately to if the user didnt put a value in
            //// check these first and modify string before checking date.
            //// RESOLVED: 10/4  ie if there is an Ordinal number but no month then adjust string to include the current month 
            //// RESOLVED: 10/4  ie if there is a time, but no day or no month, assume it is today
            ////   ie if there is a time but no am or pm, then adjust to next occurence of this time
            ////   ie if the string includes 8 in the morning or 8 in the evening make sure this works
            ////   ie if the string include 8 oclock or just 8 , and the words, breakfast, lunch, brekkie, breaky, dinner, supper then adjust time to next occureence of that time
            //// if the string includes ordinal 1st or a 1 with the word January, then we can assume its january 1st next year
            //// if the string includes january, but nother ordinal or number then set the day to this number and the month to january,
            ////   check if the current date is before this january date and if it is set the due Date to this year or else set it to next tear
            #endregion

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
        public async Task<(string, string)> CreateTaskFromInput(string input)
        {
            // using Microsoft Recognizers nuget packages, and regex.
            // Research:
            // https://github.com/microsoft/Recognizers-Text/tree/master/.NET/Samples
            // https://github.com/microsoft/Recognizers-Text/blob/master/.NET/Samples/SimpleConsole/Program.cs
            // https://csharp.hotexamples.com/examples/-/DateTimeRecognizer/-/php-datetimerecognizer-class-examples.html#google_vignette
            // https://starbeamrainbowlabs.com/blog/article.php?article=posts%2F325-AI-Microsoft-Text-Recognizers.html
            // https://stackoverflow.com/questions/52593835/parsing-timex-expressions-in-net-core

            var culture = Culture.English;
            var results = DateTimeRecognizer.RecognizeDateTime(input, culture);

            // Check for and remove "at" if it preceeds a Time ** NOT BEING USED IN THIS VERSION
            // RemoveAtFromTime(input);

            // Create a new string with all the detected date/time and ordinal entities removed, and Trim it
            var cleanedInput = input.Trim();
            
            //var cleanedInput = results.Aggregate(input, (current, entity) => current.Replace(entity.Text, ""));
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
                return (cleanedInput, results.ToString()); ;
            }

            // The DateTime model can return several resolution types
            // https://github.com/Microsoft/Recognizers-Text/blob/master/.NET/Microsoft.Recognizers.Text.DateTime/Constants.cs#L7-L14
            // We only want those with a date, date and time, or date time period: 

            var first = results.First();
            var resolutionValues = (IList<Dictionary<string, string>>)first.Resolution["values"];
            var subType = first.TypeName.Split('.').Last();
            DateTime currentDate = DateTime.Now;



            if (subType.Contains("date") && !subType.Contains("range"))
            {
                // try to parse in the date and catch it if the date is invalid or out of range
                try
                {
                    // a date (or date & time) or multiple 
                    var moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();

                    Debug.WriteLine("--------------------------");
                    Debug.WriteLine("moment=:" + moment.Year.ToString()
                        + ", " + moment.Month.ToString()
                        + ", " + moment.Day.ToString()
                        + ", " + moment.TimeOfDay.ToString());
                    Debug.WriteLine("--------------------------");

                    // If the year is not specified, use the current year
                    if (moment.Year < currentDate.Year)
                    {
                        moment = new DateTime(currentDate.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, moment.Second);
                    }

                    if (moment.Month < currentDate.Month)
                    {
                        // If the month is 4 months less than the prior month assume its in future
                        if (moment.Date < currentDate.AddMonths(-2))
                        {
                            moment = moment.AddYears(1);
                        }
                        // If the month is not specified, use the current month
                        if (moment.Month == 1)
                        {
                            // And if the input does not contain jan or january
                            if (!input.ToLower().Contains("jan") && !input.ToLower().Contains("January"))
                            {
                                //  set month to current month
                                moment = new DateTime(moment.Year, currentDate.Month, moment.Day, moment.Hour, moment.Minute, moment.Second);
                            }
                        }
                    }

                    // If the day is not specified, use the current day
                    if (moment.Day == 1)
                    {
                        moment = new DateTime(moment.Year, moment.Month, DateTime.Today.Day, moment.Hour, moment.Minute, moment.Second);
                    }

                    // If only the time is specified, use the current date
                    if (moment.Day == 1 && moment.Month == 1)
                    {
                        moment = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, moment.Hour, moment.Minute, moment.Second);
                    }

                    if (moment < DateTime.Now)
                    {
                        // a future moment is valid past moment is not 
                        await Task.Delay(200);
                        Debug.WriteLine("Exception! Cant use a date from the past!");
                        return (cleanedInput, null);
                    }
                    return (cleanedInput, moment.ToString());

                }
                catch
                {
                    Debug.WriteLine("THIS DATE IS INVALID, please enter a new date");
                    return (cleanedInput, "No Due Date");
                }
            }

            else if (subType.Contains("time"))
            {
                var moment = resolutionValues.Select(v => DateTime.Parse(v["value"])).FirstOrDefault();
                moment = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, moment.Hour, moment.Minute, moment.Second);
                return (cleanedInput, moment.ToString());

            }
            await Task.Delay(100);
            Debug.WriteLine("end of function");
            return (cleanedInput, "No Due Date");
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
        private static string GetDescription(string cleanedInput)
        {
            // create a hashset with some stop words we need to remove
            var stopWords = new HashSet<string> { "a", "at", "an", "the", "for", "on", "in", "to" }; //"with", "and",  (maybe keep this for say "meeting with Bob"
            // split the input and remove the stop words, then re-join these in a string for use as the description
            var tokens = Regex.Split(cleanedInput, @"\W+")
                .Where(token => !stopWords.Contains(token.ToLower())).ToList();
            return string.Join(" ", tokens);
        }

        public async void CheckUserInput(string userInput)
        {
            (string cleanedInput, string dateTime) = await CreateTaskFromInput(userInput);
            Debug.WriteLine("... " + userInput + " returns: \n" + GetDescription(cleanedInput) + "\n" + dateTime);
            string output = "TASK: " + cleanedInput + "\nNOTES: " + userInput + "\nDUE DATE: " + dateTime;
            ResultTextBlock.Text = output;
        }

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
                    //ResultTextBlock.Text = "HELLO!!!";
                    CheckUserInput(userInput);
                    InputTextBox.Text = string.Empty;
                }
            }
        }

        public async void ErrorMessage()
        {
            var dialog = new MessageDialog("Error: You must enter a Task");
            await dialog.ShowAsync();
            InputTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;
        }

        //private string RemoveAtFromTime(string input)
        //{
        //    //BUG! THis is not working at the moment Priority is Very Low - optional only
        //    // Check if the substring before the DateTime value contains the word "at" and remove it
        //    int indexOfTime = input.IndexOf("time", StringComparison.OrdinalIgnoreCase);
        //    if (indexOfTime > 0 && input.Substring(0, indexOfTime).Trim().EndsWith("at", StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Remove "at" and the preceding part from the original string
        //        input = input.Remove(indexOfTime - 2); // -2 to remove "at" and the preceding space
        //    }

        //    // Return the updated input
        //    return input;
        //}
    }
}


