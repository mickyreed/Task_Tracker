using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList;
using Windows.Storage;

namespace TaskList
{
    public class FolderDataManager
    {
        public static async Task SaveDataAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.CreateFileAsync("myFolderFile.bin",
                    CreationCollisionOption.ReplaceExisting);

                using (var stream = File.Open(file.Path, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                    {
                        //save all Tasks, and GUIDs
                        writer.Write(Folder.AllFoldersList.Count());
                        foreach (var folder in Folder.AllFoldersList)
                        {
                            writer.Write($"{folder.Name}");
                            writer.Write(folder.taskId.Count());
                            foreach (Guid id in folder.taskId)
                            {
                                //var task = Tasks.GetTaskById(id);
                                //writer.Write(id.ToString());
                                byte[] idBytes = id.ToByteArray();
                                writer.Write(idBytes.Length); // Write the length of the byte array
                                writer.Write(idBytes); // Write the byte array itself
                            }
                        }
                    }
                }
                await Task.Delay(2);
            }

            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: UNABLE TO SAVE FOLDERS!... \n {0} error while saving file", ex);
            }
        }

        public static async Task LoadDataAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                // use a try method here to check if file/folder is there and if not load some dummy data
                using (var stream = File.Open(storageFolder.Path + "\\myFolderFile.bin", FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        // read folder count, folder name, and all taskId's in folder for each folder
                        while (reader.PeekChar() != -1)
                        {
                            var folderCount = reader.ReadInt32();
                            int count = 0;

                            //Debug.WriteLine("");
                            //Debug.WriteLine($"DISPLAYING FOLDERS & Task Id's... there are {folderCount} folders in total");

                            while (count < folderCount)
                            {
                                var folderName = reader.ReadString();
                                Folder folder = new Folder(folderName);
                                Folder.AllFoldersList.Add(folder);
                                var folderTaskCount = reader.ReadInt32();

                                int n = 0;
                                while (n < folderTaskCount)
                                {
                                    int idLength = reader.ReadInt32(); // Read the length of the byte array
                                    byte[] idBytes = reader.ReadBytes(idLength); // Read the byte array
                                    Guid id = new Guid(idBytes); // Construct the Guid from the byte array

                                    // Print the GUID and folder name for debugging
                                    //Debug.WriteLine($" *************   Folder: {folderName}, TaskID: {id}");
                                    folder.AddTask(id);
                                    n++;
                                }
                                count++;
                            }
                        }
                    }
                }
                await Task.Delay(100);
            }

            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: NO EXISTING FOLDERS FOUND!... \n {0} Error while loading Data", ex);
            }

        }
    }
}
