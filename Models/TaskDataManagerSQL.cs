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
using Windows.Media.Core;
using System.Reflection.Metadata.Ecma335;


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
            Debug.WriteLine($"*******************************************************************************************" +
                $"********************************{fullFilePath} ***********************************" +
                $"**********************************************************************************");
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
        public static async Task LoadDataBaseAsync()
        {
            bool addRecordsToDatabase = true;

            try
            {
                //String createTableQuery = "CREATE TABLE tasks(id VARCHAR(32), description VARCHAR(50), notes VARCHAR(150), isCompleted BOOL, dateDue DATETIME, streak INT, frequency CHAR(12); ";
                String createTableQuery = "CREATE TABLE Tasks(id VARCHAR(32), folder VARCHAR(32), type VARCHAR(12), description VARCHAR(50), notes VARCHAR(150), isCompleted BOOL, dateDue DATETIME, streak INT, frequency VARCHAR(12));";

                SqliteCommand createTable = new SqliteCommand(createTableQuery, database);
                createTable.ExecuteReader();
            }

            catch (Exception ex)
            {
                addRecordsToDatabase = false;
                Debug.WriteLine($"ERROR: TABLE EXISTS!.... \n {ex.GetType().Name}: {ex.Message}");
            }
            if (addRecordsToDatabase == true)
            {
                foreach (var task in Tasks.AllTasksList)
                {
                    UpdateDataBaseAsync(task);
                }
            }
        }

        public static async Task UpdateDataBaseAsync (Tasks task)
        {
        //    // Check if task ID already exists in the database
        //    string checkExistingQuery = "SELECT COUNT(*) FROM Tasks WHERE id = @id";
        //    SqliteCommand checkExistingCommand = new SqliteCommand(checkExistingQuery, database);
        //    checkExistingCommand.Parameters.AddWithValue("@id", task.id.ToString());
        //    int existingCount = Convert.ToInt32(checkExistingCommand.ExecuteScalar());

        //    // If there's an existing task with the same ID, skip inserting
        //    if (existingCount > 0)
        //    {
        //        Debug.WriteLine($"Task with ID {task.id} already exists in the database. Skipping insertion.");
        //        continue; // Skip out to the next iteration of the for each loop
        //    }

            if (task is Habit habit)
            {
                string id = task.id.ToString();
                string folder = "";
                string type = task.GetType().Name.ToString();
                string description = task.description; // Write task type
                string notes = task.notes;
                DateTime? dateDue = task.dateDue;
                bool isCompleted = task.isCompleted;
                int streak = habit.streak;
                string frequency = (Enum.GetName(typeof(Habit.Frequency), habit.frequency));

                string insertDataQuery = "INSERT INTO Tasks (id, folder, type, description, notes, dateDue, isCompleted, frequency, streak) " +
                    "VALUES (@id, @folder, @type, @description, @notes, @dateDue, @isCompleted, @frequency, @streak)";

                SqliteCommand command = new SqliteCommand(insertDataQuery, database);

                // Add parameters
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@folder", folder);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@notes", notes);
                command.Parameters.AddWithValue("@dateDue", dateDue);
                command.Parameters.AddWithValue("@isCompleted", isCompleted);
                command.Parameters.AddWithValue("@frequency", frequency);
                command.Parameters.AddWithValue("@streak", streak);

                // Execute the command
                command.ExecuteNonQuery();
            }
            else if (task is RepeatTask repeatTask)
            {
                string id = task.id.ToString();
                string folder = "";
                string type = task.GetType().Name.ToString();
                string description = task.description; // Write task type
                string notes = task.notes;
                DateTime? dateDue = task.dateDue;
                bool isCompleted = task.isCompleted;
                int streak = 0;
                string frequency = (Enum.GetName(typeof(RepeatTask.Frequency), repeatTask.frequency));

                string insertDataQuery = "INSERT INTO Tasks (id, folder, type, description, notes, dateDue, isCompleted, frequency, streak) " +
                    "VALUES (@id, @folder, @type, @description, @notes, @dateDue, @isCompleted, @frequency, @streak)";

                SqliteCommand command = new SqliteCommand(insertDataQuery, database);

                // Add parameters
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@folder", folder);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@notes", notes);
                command.Parameters.AddWithValue("@dateDue", dateDue);
                command.Parameters.AddWithValue("@isCompleted", isCompleted);
                command.Parameters.AddWithValue("@frequency", frequency);
                command.Parameters.AddWithValue("@streak", streak);

                // Execute the command
                command.ExecuteNonQuery();
            }
            else if (task is Tasks baseTask)
                    {
                        string id = task.id.ToString();
                        string folder = "";
                        string type = task.GetType().Name.ToString();
                        string description = task.description; // Write task type
                        string notes = task.notes;
                        DateTime? dateDue = task.dateDue;
                        bool isCompleted = task.isCompleted;
                        int streak = 0;
                        string frequency = "";

                        string insertDataQuery = "INSERT INTO Tasks (id, folder, type, description, notes, dateDue, isCompleted, frequency, streak) " +
                         "VALUES (@id, @folder, @type, @description, @notes, @dateDue, @isCompleted, @frequency, @streak)";

                        SqliteCommand command = new SqliteCommand(insertDataQuery, database);

                        // Add parameters
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@folder", folder);
                        command.Parameters.AddWithValue("@type", type);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@notes", notes);
                        command.Parameters.AddWithValue("@dateDue", dateDue);
                        command.Parameters.AddWithValue("@isCompleted", isCompleted);
                        command.Parameters.AddWithValue("@frequency", frequency);
                        command.Parameters.AddWithValue("@streak", streak);

                        // Execute the command
                        command.ExecuteNonQuery();
                    }
            
        }
        public static async Task AddTaskAsync(Tasks task)
        {
            try
            {
                UpdateDataBaseAsync(task);
                Debug.WriteLine("Task added to the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Couldn't update Database!.... \n {ex.GetType().Name}: {ex.Message}");
            }
        }
        public static async Task DeleteTaskByIdAsync(Guid taskId)
        {
            try
            {
                string sql = "DELETE FROM Tasks WHERE id = @taskId";
                SqliteCommand command = new SqliteCommand(sql, database);
                command.Parameters.AddWithValue("@taskId", taskId.ToString());
                command.ExecuteNonQuery(); 
                Debug.WriteLine($"Task with ID {taskId} deleted from database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Cannot delete task with ID: {taskId}! {ex.GetType().Name}: {ex.Message}");
            }
        }

    }
}
