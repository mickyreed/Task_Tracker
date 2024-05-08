using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList;
using Windows.Storage;
using static TaskList.RepeatTask;

namespace TaskList
{
    /// <summary>
    /// A class to manage folder data to an sql database
    /// </summary>
    public class FolderDataManagerSQL
    {
        /// <summary>
        /// Sqlite database connection
        /// </summary>
        public static SqliteConnection database;

        /// <summary>
        /// declares the name of the database
        /// </summary>
        public static String filename = "myDatabase.db";

        /// <summary>
        /// A method which initialises the database "open" ready for access
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
        /// A Task to load the database and or create a table if not existing
        /// </summary>
        /// <returns></returns>
        public static async Task LoadDataBaseAsync()
        {
            bool addRecordsToDatabase = true;

            try
            {
                String createTableQuery = "CREATE TABLE Folder(id VARCHAR(32), name VARCHAR(32));";

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
                foreach (var folder in Folder.AllFoldersList)
                {
                    await UpdateDataBaseAsync(folder);
                }
            }
        }

        /// <summary>
        /// A Task to update the database with a folder if it is added
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static async Task UpdateDataBaseAsync(Folder folder)
        {
            string insertDataQuery;

            string id = folder.id.ToString();
            string name = folder.Name;

            insertDataQuery = "INSERT INTO Folder (id, name) VALUES (@id, @name)";

            SqliteCommand command = new SqliteCommand(insertDataQuery, database);

            // Add parameters
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", name);

            // Execute the command
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Helper task to call the UpdateFolderDataBaseAsync method with the folder
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task AddFolderAsync(Folder folder)
        {
            try
            {
                UpdateDataBaseAsync(folder);
                Debug.WriteLine("Folder added to the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Couldn't add folder to Database!.... \n {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Task to delete a task from the database given its id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static async Task DeleteFolderByIdAsync(Guid folderId)
        {
            try
            {
                string sql = "DELETE FROM Folder WHERE id = @folderId";
                SqliteCommand command = new SqliteCommand(sql, database);
                command.Parameters.AddWithValue("@folderId", folderId.ToString());
                command.ExecuteNonQuery();
                Debug.WriteLine($"Folder with ID {folderId} deleted from database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Cannot delete folder with ID: {folderId}! {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
