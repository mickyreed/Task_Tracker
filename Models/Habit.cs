using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskList
{
    /// <summary>
    /// 
    /// • Habits.A habit is a sub-class of the repeating task which keeps track of how long you have successfully been
    /// completing the task.For example, if you have a habit task for exercise and you have exercised every day for five
    /// days, it would know you have a streak of five days. If you miss a day, the streak is broken and resets to zero.
    /// 
    /// </summary>

    [Serializable]
    public class Habit : RepeatTask
    {
        //tracks streaks for any habit tasks
        public int streak { get; set; }

        // overide method to check if habit is completed first before moving to next due date
        // resets streak to zero if you break the streak by not completing the task by due date
        public override void MoveToNextDueDate()
        {
            //check if streak is valid
            if (IsCompleted)
            {
                streak++;
            }
            else
            {
                ResetStreak();
            }

            base.MoveToNextDueDate(); // call base class to move to next due date
        }

        public void ResetStreak()
        {
            streak = 0; //reset to start streak again
        }
    }
}
