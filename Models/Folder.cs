using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Display.Core;
using Windows.UI.Xaml.Controls;

namespace TaskList
{/// <summary>
 /// 
 /// Folders
 /// Tasks are kept in folders.

 /// /// The Database
 /// Add a static list of all the folders in the database.
 /// You will also need static methods to do the following…
 /// • Add a new item to the static list.
 /// • Remove an item from the static list, given its GUID.
 /// 
 /// </summary>
    public class Folder
    {
        public Guid id { get; } // no setter
        public string name { get; set; }

        // • A list of tasks that the folder contains.However, we are not going to implement a list of tasks directly.Instead,
        // make a list of GUIDs and if a task is meant to be in that folder, then it’s ID will be in the list. 
        public List<Guid> taskId { get; set; }


        // • TODO: A calculated property that is the total number of incomplete tasks in the folder.
        // create a public int with a count property
        public int incompleteTasksTotal()
        {
            int count = 0; //count of total tasks in folder
            int incompletedCount = 0; // count of incomplete tasks

            foreach (var taskId in taskId)
            {
                // TOD DO: get the task id
                var task = Tasks.GetTaskById(taskId);

                // TO DO: if task id is not null & not completed
                if (task != null)
                {
                    if (!task.isCompleted)
                    {
                        incompletedCount++;
                    }
                }
                count++;
            }
            return incompletedCount;
        }

        // Constructor
        public Folder(string name)
        {
            this.name = name;
            this.id = Guid.NewGuid();
            this.taskId = new List<Guid>();

            // NOTE: name property above not required during initialisation
        }

        // A method that will add a task to the folder.
        public void AddTask(Guid id)
        {

            taskId.Add(id);
        }

        // A method that will remove a task from the folder, given it’s ID.
        public void RemoveTask(Guid id)
        {
            taskId.Remove(id);
        }

        // TODO: A static list to hold all of the folders
        public static List<Folder> AllFoldersList = new List<Folder>();

        // TODO: A static method to add new folders to the static list
        public static void AddFolder(Folder folder)
        {
            AllFoldersList.Add(folder);
        }

        // TODO: A static method to remove a folder from the static list using its GUID
        public static void RemoveFolder(Folder folder)
        {
            AllFoldersList.RemoveAll(f => f.name == folder.name);
            // removes all elements from the list where the id matches the folderId.
            // https://www.geeksforgeeks.org/lambda-expressions-in-c-sharp/
        }

        //public void DisplayAllTasksInFolders()
        //{
        //    if (AllFoldersList != null)
        //    {
        //        foreach (var folder in AllFoldersList)
        //            Debug.WriteLine($"FOLDER: {folder.name}");

        //        {
        //            //Debug.WriteLine($"Tasks in {Folder.name} = {folder.taskId.Count} :");
        //            foreach (var taskId in Folder.taskId)
        //            {
        //                var task = Tasks.GetTaskById(taskId);
        //                Debug.WriteLine($"- {task.description}");
        //                //Debug.WriteLine($"- {task.id}");
        //                //Debug.WriteLine($"- {task.dateDue}");
        //                Debug.WriteLine($"- {task.notes}");

        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Personal folder is null.");
        //    }

        //}
    }
}
