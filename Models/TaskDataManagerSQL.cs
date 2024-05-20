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
using System.Collections.Immutable;


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
            Debug.WriteLine($"********************************  PATH TO TASK DATABASE:  {fullFilePath}  ***********************************");
        }

        /// <summary>
        /// A Task that loads data in from the existing storage
        /// </summary>
        /// <returns></returns>
        public static async Task LoadDataBaseAsync()
        {
            bool addRecordsToDatabase = true;

            try
            {
                String createTableQuery = "CREATE TABLE Tasks(id VARCHAR(32), folder VARCHAR(32), type VARCHAR(12), " +
                    "description VARCHAR(50), notes VARCHAR(150), isCompleted BOOL, dateDue DATETIME, streak INT, frequency VARCHAR(12));";

                SqliteCommand createTable = new SqliteCommand(createTableQuery, database);
                createTable.ExecuteReader();
            }

            catch (Exception ex)
            {
                addRecordsToDatabase = false;
                Debug.WriteLine($"ERROR: TABLE EXISTS!.... \n {ex.GetType().Name}: {ex.Message}");
            }
            
            // if database is empty, update it with the current data
            if (addRecordsToDatabase == true)
            {
                foreach (var task in Tasks.AllTasksList)
                {
                    await UpdateDataBaseAsync(task);
                }
            }
        }
        
        /// <summary>
        /// A Task that takes a task and adds it the database
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
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

            string insertDataQuery;
            int streak = 0;
            string frequency ="";

            string id = task.id.ToString();
            string folder = "";
            string type = task.GetType().Name.ToString();
            string description = task.description; // Write task type
            string notes = task.notes;
            DateTime? dateDue = task.dateDue;
            bool isCompleted = task.IsCompleted;

            if (task is Habit habit)
            {
                streak = habit.streak;
                frequency = (Enum.GetName(typeof(Habit.Frequency), habit.frequency));
            }

            else if (task is RepeatTask repeatTask)
            {
                streak = -1;
                frequency = (Enum.GetName(typeof(RepeatTask.Frequency), repeatTask.frequency));
            }

            else if (task is Tasks baseTask)
            {
                streak = -1;
                frequency = String.Empty;
            }

            insertDataQuery = "INSERT INTO Tasks (id, folder, type, description, notes, dateDue, isCompleted, frequency, streak) " +
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

        /// <summary>
        /// Helper task to call the UpdateDataBaseAsync method with the Task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task AddTaskAsync(Tasks task)
        {
            try
            {
                UpdateDataBaseAsync(task);
                Debug.WriteLine("Task added to the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Couldn't add task to Database!.... \n {ex.GetType().Name}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Task to delete a task from the database given its id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
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
