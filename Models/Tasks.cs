using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking;
using Windows.UI.Xaml.Controls;

namespace TaskList
{
    /// <summary>
    /// Tasks
    /// A class to store the details of a task.It should have the following properties…

    /// /// The Database
    /// Add a static list that stores all the tasks in the database 
    /// You will also need static methods to do the following…
    /// • Add a new item to the static list.
    /// • Remove an item from the static list, given its GUID.
    /// 
    /// </summary>
    [Serializable]
    public class Tasks : IComparable<Tasks>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Guid id { get; set; } // need to have a setter to set id when existing files are loading in
        public string description { get; set; }
        // Description (eg. “Plan India trip”)
        public string notes { get; set; }
        // Notes(eg. “Singapore is pretty close to mid way.Probably the best spot for a layover.”)
        private bool _isCompleted;
        public DateTime? dateDue { get; set; } // nullable types, so using ? the date time can be optional
                                               //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types
                                               //Constructor
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }


        public Tasks()
        {
            this.id = Guid.NewGuid();
            IsCompleted = false;
            //this.description = string.Empty;
            //this.notes = string.Empty;
            //this.dateCompleted = DateTime.MinValue;

            // NOTE: description, notes and other properties above not required during initialisation
        }

        public virtual bool isOverdue
        {
            /// <summary>
            /// A calculated property that returns true if the task is overdue
            /// and false if it it either not overdue or there is no date of completion set.
            /// </summary>

            get
            {
                if (!dateDue.HasValue || dateDue.Value < DateTime.Now.Date) // check if date completed is null or in the past (NOTE: date only dont use time value
                {
                    if (!IsCompleted) // check is if task is not completed
                    {
                        return true;
                    }
                }
                return false;
            }

            set {; }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Static list to hold all of the tasks
        /// </summary>
        public static List<Tasks> AllTasksList = new List<Tasks>();
        /// <summary>
        /// Static list to all tasks sorted by date
        /// </summary>
        public static List<Tasks> TasksByDateIndex = new List<Tasks>();
        /// <summary>
        /// Static class to hold all tasks sorted by description
        /// </summary>
        public static List<Tasks> TasksByDescriptionIndex = new List<Tasks>();

        /// <summary>
        /// A static method to add new tasks to the static list
        /// </summary>
        /// <param name="task"></param>
        public static void AddTask(Tasks task)
        {
            AllTasksList.Add(task);
            UpdateTasksIndexes();
        }

        /// <summary>
        /// A static method to remove a task from the static list using its GUID
        /// </summary>
        /// <param name="taskId"></param>
        public static void RemoveTask(Guid taskId)
        {
            AllTasksList.RemoveAll(t => t.id == taskId);
            // removes all elements from the list where the id matches the taskId.
            // https://www.geeksforgeeks.org/lambda-expressions-in-c-sharp/
            UpdateTasksIndexes();
        }

        /// <summary>
        /// Static method to get a task by its ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static Tasks GetTaskById(Guid taskId)
        {
            return AllTasksList.Find(task => task.id == taskId);
        }

        // A static method to reset the value of isCompleted to the opposite of what it currently is
        //
        //// Can static class be overriden? NO
        //// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members
        //// FUTURE NOTE: possible issues as this develops... this perhaps should be virtual?,
        //// so we can override in sub classes ie if i need to change this method to set completion on a repraet task,
        //// but then instantiate a new repeat task on the next due date set by its frequqncy,
        //// otherwise the isCompleted will need to be set to completed, then the new date set to not completed,
        //// but then this means the previous due date the task doesnt hold the value of completed? or does this not matter
        //// OR... Perhaps I just make another method that calls on this and then instantiates a new task on the next due date might be easier.
        public static void SetTaskCompletion(Tasks task)
        {
            if (task.IsCompleted)
            {
                task.IsCompleted = false;
            }
            else
            {
                task.IsCompleted = true;
            }
        }

        public int CompareTo(Tasks other)
        {

            if (other == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                //Compare by description
                if (description.CompareTo(other.description) != 0)
                {
                    return this.description.CompareTo(other.description);
                }
                // if required then compare by if it is completed
                else if (IsCompleted.CompareTo(other.IsCompleted) != 0)
                {
                    return this.IsCompleted.CompareTo(other.IsCompleted);
                }
                // if required then compare by due date
                else if (dateDue.Value < other.dateDue.Value)
                {
                    return this.dateDue.Value.CompareTo(other.dateDue.Value);
                }
                else if (notes.CompareTo(other.notes) != 0)
                {
                    return this.notes.CompareTo(other.notes);
                }
                // Finally if required Compare by id
                return this.id.CompareTo(other.id);
            }


        }

        /// <summary>
        /// Search tasks with a given description
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<Tasks> SearchByDescription(string keyword)
        {
            List<Tasks> searchedTasks = new List<Tasks>();

            foreach (var folder in Folder.AllFoldersList)
            {
                foreach (var taskId in folder.taskId)
                {
                    var task = GetTaskById(taskId);
                    if (task.description.Contains(keyword))
                    {
                        searchedTasks.Add(task);
                    }
                }
            }

            searchedTasks.Sort();

            return searchedTasks;
        }

        /// <summary>
        /// Search tasks with a given due date
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public static List<Tasks> SearchByDueDate(DateTime? dueDate)
        {
            List<Tasks> searchedTasks = new List<Tasks>();

            foreach (var folder in Folder.AllFoldersList)
            {
                foreach (var taskId in folder.taskId)
                {
                    var task = GetTaskById(taskId);

                    if (task.dateDue.HasValue && task.dateDue.Value.Date == dueDate?.Date)
                    {
                        searchedTasks.Add(task);
                    }
                }
            }

            // compare the due dates of a given task1 and task 2 (using lambda expression)
            searchedTasks.Sort((x, y) => x.dateDue.GetValueOrDefault().CompareTo(y.dateDue.GetValueOrDefault()));
            // https://chat.openai.com/share/9ccf1a2f-4576-4e3f-8353-d605adb5705f

            //searchedTasks.Sort((task1, task2) =>
            //{
            //    DateTime date1 = task1.dateDue.GetValueOrDefault(DateTime.MinValue); // Get the due date of task1, use DateTime.MinValue if null
            //    DateTime date2 = task2.dateDue.GetValueOrDefault(DateTime.MinValue); //  the due date of task2, use DateTime.MinValue if null

            //    return date1.CompareTo(date2); // Compare the due dates
            //});

            return searchedTasks;
        }

        /// <summary>
        /// Sort the tasks by date
        /// </summary>
        private static void SortTasksByDate()
        {
            AllTasksList.Sort((x, y) => x.dateDue.GetValueOrDefault().CompareTo(y.dateDue.GetValueOrDefault()));
            TasksByDateIndex.Clear();
            Debug.WriteLine("Sorted Tasks by Date:");
            foreach (var task in AllTasksList)
            {
                TasksByDateIndex.Add(task);
                Debug.WriteLine($"Description: {task.description}, Due Date: {task.dateDue}");
            }
        }

        /// <summary>
        /// Sort tasks by description
        /// </summary>
        private static void SortTasksByDescription()
        {
            AllTasksList.Sort((x, y) => x.description.CompareTo(y.description));
            TasksByDescriptionIndex.Clear();

        }

        /// <summary>
        /// Method to update tasks 
        /// </summary>& indexes when changes are made
        public static void UpdateTasksIndexes()
        {
            //SortTasksByDate();
            SortTasksByDescription();

        }

        /// <summary>
        /// Method that displays a Task description, notes and dateDue as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Description: {description}\nContent: {notes}\nDue Date: {dateDue?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}";
        }
    }
}
