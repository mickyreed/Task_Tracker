using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TaskList
{
    public static class TaskCreator
    {
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
        public static async Task<(string, DateTime?)> CreateTaskDataFromInput(string input)
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

            //await Task.Delay(100);
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
        private static DateTime CheckAndAdjustForYear(DateTime moment, DateTime currentDate)
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
        private static DateTime CheckAndAdjustForFortnight(string input, DateTime moment, DateTime currentDate)
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
        private static DateTime CheckAndAdjustMoment(string input, DateTime moment, DateTime currentDate)
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
            if (DateTime.Now > DateTime.Today.AddHours(12) && moment.Date == DateTime.Today
                && input.ToLower().Contains("breakfast") || input.ToLower().Contains("tomorrow"))
            //if (moment.Hour > 0 && moment.Hour < 12)
            {
                Debug.WriteLine($"moment.Date: {moment.Date}");
                Debug.WriteLine($"moment.TimeOfDay: {moment.TimeOfDay}");
                Debug.WriteLine($"DateTime.Now: {DateTime.Now}");
                Debug.WriteLine($"DateTime.Today: {DateTime.Today}");
                Debug.WriteLine($"DateTime.Today+12: {DateTime.Today.AddHours(12)}");
                Debug.WriteLine($"DateTime.Today+24: {DateTime.Today.AddHours(24)}");
                Debug.WriteLine($"DateTime.Today+1Day: {DateTime.Today.AddDays(1)}");

                adjustedMoment = moment.AddHours(24);
                Debug.Assert(DateTime.Now > DateTime.Today.AddHours(12));
                Debug.Assert(moment.Date == DateTime.Today);
                return adjustedMoment;
            }

            // if the task contains breakfast and does not contain tomorrow and the chsoen time is less than 12pm add a day
            //if (input.ToLower().Contains("breakfast") && (moment.Hour <12) && (input.ToLower().Contains("tomorrow")))
            else if (DateTime.Now < DateTime.Today.AddHours(12) && moment.Date == DateTime.Today
                && input.ToLower().Contains("breakfast") || input.ToLower().Contains("tomorrow"))
            {
                Debug.Assert(DateTime.Now < DateTime.Today.AddHours(12));
                Debug.Assert(moment.Date == DateTime.Today);
                adjustedMoment = moment.AddHours(24);

                return adjustedMoment;
            }



                //if (input.ToLower().Contains("breakfast") || (input.ToLower().Contains("tomorrow") &&
                //    (moment.Date >= DateTime.Now && moment.Date <= DateTime.Today.AddHours(12))))
                //{
                //    //Trace.Assert(moment.Date >= DateTime.Now);
                //    //Trace.Assert(moment.Date <= DateTime.Today.AddHours(12));
                //    Debug.WriteLine($"Breakfast is Today:   {moment.Date}");
                //    //adjustedMoment = moment.AddHours(12);
                //    adjustedMoment = moment.AddHours(24);
                //    //adjustedMoment = CheckAndUpdateTime(moment);
                //    return adjustedMoment;
                //}
                ////else if (input.ToLower().Contains("breakfast") && (moment.Hour > 12) && (input.ToLower().Contains("tomorrow")))
                //else if (input.ToLower().Contains("breakfast") || (input.ToLower().Contains("tomorrow") &&
                //    (moment.Date <= DateTime.Now && moment.Date <= DateTime.Today.AddHours(12))))
                //{
                //    Debug.WriteLine($"Breakfast is tomorrow:   {moment.Date}");
                //    adjustedMoment.AddDays(0);
                //    return adjustedMoment;
                //}
                //// else if task contains breakfast and current time is after 12pm
                //else if (!input.ToLower().Contains("breakfast") || (input.ToLower().Contains("tomorrow") &&
                //    (moment.Date > DateTime.Today.AddHours(12))))
                //{
                //    Debug.WriteLine($"After 12 Breakfast is tomorrow:   {moment.Date}");
                //    adjustedMoment = moment.AddHours(12);
                //    //adjustedMoment = CheckAndUpdateTime(moment);
                //    return adjustedMoment;
                //}
            //}

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
        public static async Task <(string, string, DateTime?)> CheckUserInput(string userInput)
        {
            (string cleanedInput, DateTime? dateTime)  = await CreateTaskDataFromInput(userInput);

            cleanedInput = cleanedInput.Trim(); //trim the input
            cleanedInput = GetDescription(cleanedInput); // call to remove any unwanted stop words from description
            //string dateTimeToDisplay;
            //cleanedInput = GetDescription(cleanedInput); // second clean for extra words (resolves extra "at"
           
            return (cleanedInput, userInput, dateTime);
            
            //CreateTaskFromTaskData(cleanedInput, userInput, dateTime);

            //if (dateTime == null)
            //{
            //    dateTimeToDisplay = "None";
            //}
            //else
            //{
            //    dateTimeToDisplay = dateTime.ToString();
            //}
            ////Debug.WriteLine("... " + userInput + " returns: \n" + cleanedInput + "\n" + dateTime);
            //string output = ($"TASK: {cleanedInput} \nNOTES: {userInput} \nDUE DATE: {dateTimeToDisplay}");


            //return output;
        }

        /// <summary>
        /// A function which will take the adjusted DateTime and input Data and create a Task Object
        /// </summary>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <param name="dateDue"></param>
        public static async void CreateTaskFromTaskData(string description, string notes, DateTime? dateDue)
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
            taskAdded.IsCompleted = false;
            Tasks.AddTask(taskAdded);
            
            //********************* THIS NEEDS TO BE DONE IN THE TASKVIEW PAGE OR SEPARATE HELPER *******************************

            //Folder1.AddTask(taskAdded.id);
            //Folder selectedFolder = CheckCurrentSelectedFolder(SelectedFolderName);
            //if (selectedFolder != null)
            //{
            //    selectedFolder.AddTask(taskAdded.id);
            //    Debug.WriteLine(taskAdded.description, selectedFolder.Name);
            //    DisplayTasksInFolders();
            //}

            //DisplayTasksInFolders();




            // ***   ADD TASK TO THE SQL DATABASE   ***
            _ = TaskDataManagerSQL.UpdateDataBaseAsync(taskAdded);
            // ***   ADD TASK TO BINARY FILE   ***
            await TaskDataManager.SaveDataAsync();

            Guid idToLookUp = taskAdded.id;
            Debug.WriteLine($"\nAdded Task " +
                $"\nDescription: {taskAdded.description} " +
                $"\nID: {idToLookUp}" +
                $"\nNotes: {taskAdded.notes} " +
                $"\nDate Due: {dateDue}");

            Debug.WriteLine($"*************************************************");

        }
        #endregion
    }
}
