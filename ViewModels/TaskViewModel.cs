using Microsoft.Recognizers.Text.Matcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using static TaskList.Tasks;
using static TaskList.RepeatTask;
using static TaskList.Habit;


namespace TaskList
{
    public class TaskViewModel : INotifyPropertyChanged
    {

        private string _description;
        private string _notes;
        private DateTime? _dateDue;
        private TimeSpan? _timeDue;
        private DateTime _minDate;
        
        private TaskType _taskType;
        private Frequency? _frequency;

        //Set the allowed minimum DateTime in relation to todays date
        //public DateTime MinDate => DateTime.Now.Date;
        public DateTime MinAllowedDate => new DateTime(DateTime.Now.Year, MinMonth, MinDay);
        public DateTime MinAllowedTime => new DateTime(DateTime.Now.Year, MinHour, MinMinute);
        public int MinMonth => DateTime.Now.Month;
        public int MinDay => DateTime.Now.Day;
        public int MinHour => DateTime.Now.Hour;
        public int MinMinute => DateTime.Now.Minute;

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateDue
        {
            get => _dateDue;
            set
            {
                if (_dateDue != value)
                {
                    _dateDue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NonNullableDateDue));
                }
            }
        }

        public DateTime NonNullableDateDue
        {
            get => _dateDue ?? DateTime.Now.Date;
            set
            {
                if (_dateDue != value)
                {
                    _dateDue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DateDue));
                }
            }
        }

        /// <summary>
        /// Datetime reference to make sure we cant pick a date in the past
        /// </summary>
        //public DateTime MinDate => DateTime.Now.Date;

        public TimeSpan? TimeDue
        {
            get => _timeDue;
            set
            {
                if (_timeDue != value)
                {
                    _timeDue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NonNullableTimeDue));
                }
            }
        }

        public TimeSpan NonNullableTimeDue
        {
            get => _timeDue ?? TimeSpan.Zero;
            set
            {
                if (_timeDue != value)
                {
                    _timeDue = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDue));
                }
            }
        }

        public DateTime MinDate
        {
            get { return _minDate; }
            set
            {
                if (_minDate != value)
                {
                    _minDate = value;
                    OnPropertyChanged(); // Implement INotifyPropertyChanged interface
                }
            }
        }

        public TaskType TaskType
        {
            get => _taskType;
            set
            {
                if (_taskType != value)
                {
                    _taskType = value;
                    OnPropertyChanged();
                }
            }
        }

        public Frequency? Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));       
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

