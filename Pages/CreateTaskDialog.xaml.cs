﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    public sealed partial class CreateTaskDialog : ContentDialog
    {
        public TaskViewModel ViewModel { get; set; }

        public List<TaskType> TaskTypes { get; } = new List<TaskType> { TaskType.Task, TaskType.RepeatingTask, TaskType.Habit };
        public List<Frequency> Frequencies { get; } = new List<Frequency> { Frequency.Daily, Frequency.Weekly };

        public CreateTaskDialog(TaskViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
            DateTime MinDate = DateTime.Today;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Validate time to ensure it is not in the past
            DateTime selectedDateTime = ViewModel.NonNullableDateDue.Add(ViewModel.NonNullableTimeDue);
            if (selectedDateTime < DateTime.Now)
            {
                args.Cancel = true;
                // Show a message to the user
                var dialog = new ContentDialog
                {
                    Title = "Invalid Time",
                    Content = "The selected time cannot be in the past.",
                    CloseButtonText = "Ok"
                };
                _ = dialog.ShowAsync();
            }
            else
            {
                // Logic to create the task using ViewModel properties
                Debug.WriteLine("Created Task will happen here...");
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Logic to cancel task creation
            Debug.WriteLine("CANCEL Create Task will happen here...");
        }
    }
}