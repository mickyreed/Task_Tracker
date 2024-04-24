using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskList;
using Windows.Globalization;
using Windows.Media.Capture;
using Windows.Storage;
using static TaskList.RepeatTask;
using Microsoft.Data.Sqlite;


namespace TaskList
{
    /// <summary>
    /// A class which initialises  the database
    /// </summary>
    public class TaskDataManagerSQL
    {
        public static SqliteConnection database;
        public static String filename = "myDatabase.db";
        

        /// <summary>
        /// A methiod which initialises the database "open" ready for access
        /// </summary>
        /// <returns></returns>
        public static async Task InitialiseDatabase()
        {
            // Create the database file if it doesn’t exist 
            var appLocalFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine(appLocalFolder.Path);
            await appLocalFolder.CreateFileAsync(filename,
             CreationCollisionOption.OpenIfExists);
            // Open the database

            string fullFilePath = Path.Combine(appLocalFolder.Path,
             filename);
            database = new SqliteConnection("Filename=" + fullFilePath);
            database.Open();
        }

        public static async Task SaveDataSQLAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.CreateFileAsync("myTaskFile.bin",
                    CreationCollisionOption.ReplaceExisting);

                using (var stream = File.Open(file.Path, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        //writer.Write(Tasks.allTasksList.Count

                        foreach (var task in Tasks.AllTasksList)
                        {
                            if (task is Habit habit)
                            //if (task is Habit habit)
                            {
                                writer.Write(task.GetType().Name); // Write task type
                                writer.Write(task.id.ToByteArray()); // Write Guid as byte array
                                writer.Write(task.description);
                                writer.Write(task.notes);
                                ////writer.Write(task.dateDue?.ToString()); // Write nullable DateTime as string
                                if (task.dateDue.HasValue)
                                {
                                    writer.Write(true); // Check & Flag the value as not null
                                    writer.Write(task.dateDue.Value.Ticks); // Write ticks representing the DateTime value
                                                                            //    //https://learn.microsoft.com/en-us/dotnet/api/system.datetime.ticks?view=net-6.0
                                }
                                else
                                {
                                    writer.Write(false); // Write a flag indicating the value is null
                                }

                                writer.Write(task.isCompleted);
                                writer.Write(Enum.GetName(typeof(Habit.Frequency), habit.frequency));
                                //Debug.WriteLine(habit.frequency.ToString());
                                int streak = habit.streak;
                                writer.Write(streak);
                                //Debug.WriteLine(streak);
                            }
                            //int streak = habit.streak;

                            else if (task is RepeatTask repeatTask)
                            {
                                writer.Write(task.GetType().Name); // Write task type
                                writer.Write(task.id.ToByteArray()); // Write Guid as byte array
                                writer.Write(task.description);
                                writer.Write(task.notes);
                                ////writer.Write(task.dateDue?.ToString()); // Write nullable DateTime as string
                                if (task.dateDue.HasValue)
                                {
                                    writer.Write(true); // Check & Flag the value as not null
                                    writer.Write(task.dateDue.Value.Ticks); // Write ticks representing the DateTime value
                                                                            //    //https://learn.microsoft.com/en-us/dotnet/api/system.datetime.ticks?view=net-6.0
                                }
                                else
                                {
                                    writer.Write(false); // Write a flag indicating the value is null
                                }

                                writer.Write(task.isCompleted);
                                writer.Write(Enum.GetName(typeof(RepeatTask.Frequency), repeatTask.frequency));
                            }

                            else if (task is Tasks baseTask)
                            {
                                writer.Write(task.GetType().Name); // Write task type
                                writer.Write(task.id.ToByteArray()); // Write Guid as byte array
                                writer.Write(task.description);
                                writer.Write(task.notes);
                                ////writer.Write(task.dateDue?.ToString()); // Write nullable DateTime as string
                                if (task.dateDue.HasValue)
                                {
                                    writer.Write(true); // Check & Flag the value as not null
                                    writer.Write(task.dateDue.Value.Ticks); // Write ticks representing the DateTime value
                                                                            //    //https://learn.microsoft.com/en-us/dotnet/api/system.datetime.ticks?view=net-6.0
                                }
                                else
                                {
                                    writer.Write(false); // Write a flag indicating the value is null
                                }

                                writer.Write(task.isCompleted);
                            }
                        }
                        await Task.Delay(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: UNABLE TO SAVE TASKS!... {ex.GetType().Name}: {ex.Message}");
            }
        }

        public static async Task LoadDataAsync()
        {
            bool addSampleRecordsToDatabase = true;

            try
            {
                String createTableQuery = "CREATE TABLE TASKS(ID CHAR(32), description VARCHAR(50), notes VARCHAR(150), isCompleted BOOL, dateDue DATETIME); ";
                SqliteCommand createTable = new SqliteCommand(createTableQuery, database);
                createTable.ExecuteReader();
            }

            catch (Exception ex)
            {
                addSampleRecordsToDatabase = false;
                Debug.WriteLine($"ERROR: TABLE EXISTS!.... \n {ex.GetType().Name}: {ex.Message}");
            }
            if (addSampleRecordsToDatabase == true)
            {
                /// Put code to create sample data here
                foreach (var task in Tasks.AllTasksList)
                {
                    if (task is Habit habit)
                    {
                        string ID = task.id.ToString();
                        string description = task.GetType().Name; // Write task type
                        string notes = task.notes.ToString();
                        DateTime? dateTime = task.dateDue;
                        bool isCompleted = task.isCompleted;
                        int streak = habit.streak;
                        string frequency = (Enum.GetName(typeof(Habit.Frequency), habit.frequency));


                        String insertDataQuery =
                        "INSERT INTO TASKS (ID, description, notes, dateTime, isCompleted) " +
                        "VALUES (" + ID + ", \"" + description + ", \"" + notes + ", \"" + dateTime + 
                        ", \"" + isCompleted + ", \"" + frequency + ", \"" + streak + "\");";

                        Debug.WriteLine(insertDataQuery);
                        SqliteCommand insertData = new SqliteCommand(insertDataQuery, database);
                        insertData.ExecuteReader();
                    }
                    else if (task is RepeatTask repeatTask)
                    {
                        string ID = task.id.ToString();
                        string description = task.GetType().Name; // Write task type
                        string notes = task.notes.ToString();
                        DateTime? dateTime = task.dateDue;
                        bool isCompleted = task.isCompleted;
                        string frequency = (Enum.GetName(typeof(RepeatTask.Frequency), repeatTask.frequency));

                        String insertDataQuery =
                        "INSERT INTO TASKS (ID, description, notes, dateTime, isCompleted) " +
                        "VALUES (" + ID + ", \"" + description + ", \"" + notes + ", \"" + dateTime +
                        ", \"" + isCompleted + ", \"" + frequency + "\");";

                        Debug.WriteLine(insertDataQuery);
                        SqliteCommand insertData = new SqliteCommand(insertDataQuery, database);
                        insertData.ExecuteReader();
                    }

                    else if (task is Tasks baseTask)
                    {
                        string ID = task.id.ToString();
                        string description = task.GetType().Name; // Write task type
                        string notes = task.notes.ToString();
                        DateTime? dateTime = task.dateDue;
                        bool isCompleted = task.isCompleted;

                        String insertDataQuery =
                        "INSERT INTO TASKS (ID, description, notes, dateTime, isCompleted) " +
                        "VALUES (" + ID + ", \"" + description + ", \"" + notes + ", \"" + dateTime +
                        ", \"" + isCompleted + "\");";

                        Debug.WriteLine(insertDataQuery);
                        SqliteCommand insertData = new SqliteCommand(insertDataQuery, database);
                        insertData.ExecuteReader();
                    }
                }  
            }
        }
    }
}
