using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskList
{
    public class TaskViewModel : INotifyPropertyChanged
    {

        private string _description;
        private string _notes;
        private DateTime? _dateDue;
        private TimeSpan _dueTime;
        private TaskType _taskType;
        private Frequency? _frequency;

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public DateTime? DateDue
        {
            get => _dateDue ?? DateTime.Now.Date;
            set { _dateDue = value; OnPropertyChanged(); }
        }

        public TimeSpan DueTime
        {
            get => _dueTime == default ? new TimeSpan(23, 59, 0) : _dueTime;
            set { _dueTime = value; OnPropertyChanged(); }
        }

        public TaskType TaskType
        {
            get => _taskType;
            set { _taskType = value; OnPropertyChanged(); }
        }

        public Frequency? Frequency
        {
            get => _frequency;
            set { _frequency = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
                
        }
    }

    public enum TaskType
    {
        Task,
        RepeatingTask,
        Habit
    }

    public enum Frequency
    {
        Daily,
        Weekly
    }
}

