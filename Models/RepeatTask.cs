using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.UserDataTasks.DataProvider;

namespace TaskList
{
    /// <summary>
    /// 
    /// • Repeating tasks.These tasks repeat every day or every week. When a task is “completed” it moves to the next date it needs to repeat
    /// So, if you complete a weekly task on the 1st of March, it will shift seven days to the next week (the 8th of March).
    //
    /// How you implement this is up to you.An enum might be useful and you may wish to write some methods(eg.
    /// setToWeekly() or somesuch).
    /// 
    /// </summary>

    [Serializable]
    public class RepeatTask : Tasks
    {
        public enum Frequency
        {
            Daily,
            Weekly
        }

        public Frequency frequency { get; set; }

        // Method to change the completion date to the next due date
        public virtual void MoveToNextDueDate() // need to make this virtual so habit class can override
        {
            int day = 1;
            int week = day * 7;
            //check if completion date has a value first
            if (dateDue.HasValue)
            {
                switch (frequency)
                {
                    case Frequency.Daily:
                        //dateCompleted = dateCompleted.Value.AddDays(1);
                        completeTask(day);
                        break;
                    case Frequency.Weekly:
                        //dateCompleted = dateCompleted.Value.AddDays(7);
                        completeTask(week);
                        break;
                }
            }
        }

        public override bool isOverdue
        {
            /// <summary>
            /// A calculated property that returns true if the task is overdue
            /// and false if it it either not overdue or there is no date of completion set.
            /// </summary>

            get
            {
                if (!dateDue.HasValue || (dateDue.Value) < (DateTime.Now.Date)) // check if date completed is null or in the past
                {
                    if (frequency == Frequency.Daily || dateDue.Value < DateTime.Now.AddDays(-2).Date)
                    {
                        if (!isCompleted) // check is if task is not completed
                        {
                            return true;
                        }
                    }
                    else if (frequency == Frequency.Weekly || dateDue.Value < DateTime.Now.AddDays(-8).Date)
                    {
                        if (!isCompleted) // check is if task is not completed
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public void completeTask(int days)
        {
            dateDue = dateDue.Value.AddDays(days);
        }

    }
}
