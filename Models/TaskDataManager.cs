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


namespace TaskList
{
    public class TaskDataManager
    {
        public static async Task SaveDataAsync()
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
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                using (var stream = File.Open(storageFolder.Path + "\\myTaskFile.bin", FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        //Debug.WriteLine($"***************  DISPLAYING TASKS... ****************");
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            var taskType = reader.ReadString();

                            Tasks task = null;

                            if (taskType == nameof(Habit))
                            //else if (taskType == nameof(Habit))
                            {

                                Guid id = new Guid(reader.ReadBytes(16)); // Read task id as byte array
                                //Debug.WriteLine($"{taskType} {id}");
                                string description = reader.ReadString();
                                //Debug.WriteLine(description);
                                string notes = reader.ReadString();
                                //Debug.WriteLine(notes);
                                //DateTime? dateDue = string.IsNullOrEmpty(reader.ReadString()) ? : null DateTime.Parse(reader.ReadString());
                                DateTime? dateDue = null;
                                //Debug.WriteLine(dateDue.ToString());
                                if (reader.ReadBoolean()) // Read the flag
                                {
                                    long ticks = reader.ReadInt64(); // Read ticks representing the DateTime value
                                    dateDue = new DateTime(ticks); // Convert ticks to DateTime
                                    //Debug.WriteLine(dateDue.ToString());
                                }

                                bool isCompleted = reader.ReadBoolean();
                                //Debug.WriteLine(isCompleted.ToString());

                                string frequencyString = reader.ReadString();
                                Frequency _frequency = (Habit.Frequency)Enum.Parse(typeof(Habit.Frequency), frequencyString);
                                //Debug.WriteLine($"Frequency: {frequencyString}");
                                //int _streak = reader.ReadInt32();

                                var streak = reader.ReadInt32();
                                //var streak = 5;

                                //Debug.WriteLine(streak);

                                task = new Habit
                                {
                                    id = id,
                                    description = description,
                                    notes = notes,
                                    dateDue = dateDue,
                                    isCompleted = isCompleted,
                                    frequency = _frequency,
                                    streak = streak
                                };
                            }

                            if (taskType == nameof(RepeatTask))
                            {
                                Guid id = new Guid(reader.ReadBytes(16)); // Read task id as byte array
                                //Debug.WriteLine($"{taskType} {id}");
                                string description = reader.ReadString();
                                //Debug.WriteLine(description);
                                string notes = reader.ReadString();
                                //Debug.WriteLine(notes);
                                ////DateTime? dateDue = string.IsNullOrEmpty(reader.ReadString()) ? : null DateTime.Parse(reader.ReadString());
                                DateTime? dateDue = null;
                                ////Debug.WriteLine(dateDue.ToString());
                                if (reader.ReadBoolean()) // Read the flag
                                {
                                    long ticks = reader.ReadInt64(); // Read ticks representing the DateTime value
                                    dateDue = new DateTime(ticks); // Convert ticks to DateTime
                                    //Debug.WriteLine(dateDue.ToString());
                                }

                                bool isCompleted = reader.ReadBoolean();
                                //Debug.WriteLine(isCompleted.ToString());
                                string frequencyString = reader.ReadString();
                                RepeatTask.Frequency _frequency = (RepeatTask.Frequency)Enum.Parse(typeof(RepeatTask.Frequency), frequencyString);
                                //Debug.WriteLine($"Frequency: {frequencyString}");

                                task = new RepeatTask
                                {
                                    id = id,
                                    description = description,
                                    notes = notes,
                                    dateDue = dateDue,
                                    isCompleted = isCompleted,
                                    frequency = _frequency
                                };
                            }

                            if (taskType == nameof(Tasks))
                            {
                                Guid id = new Guid(reader.ReadBytes(16)); // Read task id as byte array
                                //Debug.WriteLine($"{taskType} {id}");
                                string description = reader.ReadString();
                                //Debug.WriteLine(description);
                                string notes = reader.ReadString();
                                //Debug.WriteLine(notes);
                                ////DateTime? dateDue = string.IsNullOrEmpty(reader.ReadString()) ? : null DateTime.Parse(reader.ReadString());
                                DateTime? dateDue = null;
                                ////Debug.WriteLine(dateDue.ToString());
                                if (reader.ReadBoolean()) // Read the flag
                                {
                                    long ticks = reader.ReadInt64(); // Read ticks representing the DateTime value
                                    dateDue = new DateTime(ticks); // Convert ticks to DateTime
                                    //Debug.WriteLine(dateDue.ToString());
                                }

                                bool isCompleted = reader.ReadBoolean();
                                //Debug.WriteLine(isCompleted.ToString());

                                task = new Tasks
                                {
                                    id = id,
                                    description = description,
                                    notes = notes,
                                    dateDue = dateDue,
                                    isCompleted = isCompleted
                                };
                            }

                            Tasks.AllTasksList.Add(task);
                        }
                    }
                    await Task.Delay(3);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WARNING: NO EXISTING TASKS FOUND!.... \n {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
