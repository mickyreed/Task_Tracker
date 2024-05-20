using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList;

namespace TaskList
{
    public class DefaultDataInitialiser
    {
        public static void LoadDefaultData()
        {
            Debug.WriteLine($"");
            Debug.WriteLine($"LOADING DEFAULT VALUES......");
            Debug.WriteLine($"");

            var folder1 = new Folder("Personal Tasks");
            folder1.Name = "Personal Tasks";
            Folder.AddFolder(folder1);

            var folder2 = new Folder("Work Tasks");
            folder2.Name = "Work Tasks";
            Folder.AddFolder(folder2);

            var task1 = new Tasks();
            //Debug.WriteLine(task1.id);
            task1.description = "Plan India trip";
            task1.notes = "research places to stay. make enquiries into flights. speak to Dad";
            task1.dateDue = DateTime.Now.AddDays(-1);
            task1.IsCompleted = false;
            Tasks.AddTask(task1);
            folder1.AddTask(task1.id);
            //Debug.WriteLine("TASK: " + task1.description);

            var task2 = new Tasks();
            //Debug.WriteLine(task2.id);
            task2.description = "Take the bin out";
            task2.notes = "";
            task2.dateDue = DateTime.Now.AddDays(+0); // this is due today NOT OVERDUE
            task2.IsCompleted = false;
            Tasks.AddTask(task2);
            folder1.AddTask(task2.id);
            //Debug.WriteLine("TASK: " + task2.description);

            var task3 = new Tasks();
            //Debug.WriteLine(task3.id);
            task3.description = "Plan India trip again";
            task3.notes = "Speak to mum this time";
            task3.dateDue = DateTime.Now.AddDays(-1); // this is overdue DUE 1 DAY AGO
            task3.IsCompleted = false;
            Tasks.AddTask(task3);
            folder1.AddTask(task3.id);
            //Debug.WriteLine("TASK: " + task3.description);

            var task4 = new Tasks();
            //Debug.WriteLine(task4.id);
            task4.description = "Read a novel";
            task4.notes = "Lord of the Rings again??";
            task4.dateDue = DateTime.Now.AddDays(-1); // this is past the due date ...
            task4.IsCompleted = true; // but has been completed !!
            Tasks.AddTask(task4);
            folder1.AddTask(task4.id);
            //Debug.WriteLine("TASK: " + task4.description);

            var task4a = new Tasks();
            //Debug.WriteLine(task4.id);
            task4a.description = "Read a novel";
            task4a.notes = "possibly The Marsian by Andy Wier, yes i know I've read this 20 times already :)";
            task4a.dateDue = DateTime.Now.AddDays(+2); // this is past the due date ...
            task4a.IsCompleted = false; // but has been completed !!
            Tasks.AddTask(task4a);
            folder1.AddTask(task4a.id);
            //Debug.WriteLine("TASK: " + task4.description);

            // ADD 1 task in work folder
            var task5 = new Tasks();
            task5.description = "Call Accountant";
            task5.notes = "Ring Paul about the business venture";
            task5.dateDue = DateTime.Now.AddDays(3); // this is due tomorrowin 1 day
            task5.IsCompleted = false;
            Tasks.AddTask(task5);
            folder2.AddTask(task5.id);
            //Debug.WriteLine("TASK: " + task5.description);

            //create a habit and add to the personal folder
            var habitTask = new Habit();
            habitTask.description = "Daily Exercise";
            habitTask.notes = "Leg Day!!";
            habitTask.dateDue = DateTime.Today;
            habitTask.IsCompleted = false;
            habitTask.frequency = Habit.Frequency.Daily; ;
            habitTask.streak = 5;
            Tasks.AddTask(habitTask);
            folder1.AddTask(habitTask.id);
            //Debug.WriteLine("TASK: " + habitTask.GetType().Name + " - " + habitTask.description);

            ////create a habit and add to the PERSONAL folder
            var habitTask2 = new Habit();
            habitTask2.description = "Daily Exercise";
            habitTask2.notes = "Chest and Arms,followed by yoga";
            habitTask2.dateDue = DateTime.Now.AddDays(-1);
            habitTask2.IsCompleted = true;
            habitTask2.frequency = Habit.Frequency.Daily;
            habitTask2.streak = 5;
            Tasks.AddTask(habitTask2);
            folder1.AddTask(habitTask2.id);
            //Debug.WriteLine("TASK: " + habitTask2.GetType().Name + " - " + habitTask2.description);

            ////create a repeat Task and add to personal folder
            var repeatTask = new RepeatTask();
            repeatTask.description = "Pick up Weekly Mail";
            repeatTask.notes = "";
            repeatTask.dateDue = DateTime.Now.AddDays(-3);  //this will be overdue
            repeatTask.IsCompleted = false;
            repeatTask.frequency = RepeatTask.Frequency.Weekly;
            Tasks.AddTask(repeatTask);
            folder1.AddTask(repeatTask.id);
            //Debug.WriteLine("TASK: " + repeatTask.GetType().Name + " - " + repeatTask.description);

            //////create a repeat Task and add to personal folder
            var repeatTask2 = new RepeatTask();
            repeatTask2.description = "Clean Bathroom Weekly";
            repeatTask2.notes = "";
            repeatTask2.dateDue = DateTime.Now.AddDays(+1);
            repeatTask2.IsCompleted = false;
            repeatTask2.frequency = RepeatTask.Frequency.Weekly; // this will be not overdue (due tomorrow)
            Tasks.AddTask(repeatTask2);
            folder1.AddTask(repeatTask2.id);
            ////Debug.WriteLine("TASK: " + repeatTask2.GetType().Name + " - " + repeatTask2.description);

            ////create a repeat Task and add to personal folder
            var repeatTask3 = new RepeatTask();
            repeatTask3.description = "Get out of Bed";
            repeatTask3.notes = "Reminder: GET MOTIVATED!!";
            repeatTask3.dateDue = DateTime.Now.AddDays(+0);
            repeatTask3.IsCompleted = false;
            repeatTask3.frequency = RepeatTask.Frequency.Daily; // this will be not overdue (due today)
            Tasks.AddTask(repeatTask3);
            folder1.AddTask(repeatTask3.id);
            ////Debug.WriteLine("TASK: " + repeatTask3.GetType().Name + " - " + repeatTask3.description);

            //UpdateData(); // Save the default data after loading
        }
    }
}
