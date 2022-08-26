using MVVMCSharpTeamsAutomation.Model;
using MVVMCSharpTeamsAutomation.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMCSharpTeamsAutomation.ViewModel
{
    /// <summary>
    /// Represents 
    /// </summary>
    public class TaskFormViewModel
    {
        private ScheduledTask selectedTask;

        private string nameValue;
        private TaskItem selectedRecipients;
        private TaskItem selectedMessage;
        private DateTime selectedDate;
        private int selectedHours;
        private int selectedMinutes;
        private bool isOneTime = false;
        private bool isCyclical = false;
        private int selectedDays;

        private List<int> hoursRange = Enumerable.Range(0, 24).ToList();
        private List<int> minutesRange = Enumerable.Range(0, 60).ToList();
        private List<int> daysRange = new List<int>{1,2,3,7,14,30,60,90};

        public TaskFormViewModel(ScheduledTask? task)
        {
            if(task!=null)
            {
                this.nameValue = task.Name;
                this.selectedRecipients = DataModelSingleton.getInstance().GetRecipientsById(task.RecipientsId);
                this.selectedMessage = DataModelSingleton.getInstance().GetMessageById(task.MessageId);
                this.selectedDate = task.ScheduledDate.Date;
                this.selectedHours = task.ScheduledDate.Hour;
                this.selectedMinutes = task.ScheduledDate.Minute;
                if(task.TaskType==TaskType.OneTime)
                    this.isOneTime = true;
                else
                    this.isCyclical = true;
                this.selectedDays = task.DaysToRepeat;
            }
            else
            {
                DateTime now = DateTime.Now;
                this.selectedDate = now.Date;
                this.selectedHours = now.Hour;
                this.selectedMinutes = now.Minute;
            }
            this.selectedTask = task;
        }
        public ExtendedObservableCollection<TaskItem> Recipients
        {
            get { return DataModelSingleton.getInstance().ModelSet.Recipients; }
        }

        public ExtendedObservableCollection<TaskItem> Messages
        {
            get { return DataModelSingleton.getInstance().ModelSet.Messages; }
        }

        public string NameValue { get => nameValue; set => nameValue = value; }
        public TaskItem SelectedRecipients { get => selectedRecipients; set => selectedRecipients = value; }
        public TaskItem SelectedMessage { get => selectedMessage; set => selectedMessage = value; }
        public DateTime SelectedDate { get => selectedDate; set => selectedDate = value; }
        public int SelectedHours { get => selectedHours; set => selectedHours = value; }
        public int SelectedMinutes { get => selectedMinutes; set => selectedMinutes = value; }
        public int SelectedDays { get => selectedDays; set => selectedDays = value; }
        public bool IsOneTime { get => isOneTime; set => isOneTime = value; }
        public bool IsCyclical { get => isCyclical; set => isCyclical = value; }
        public List<int> HoursRange { get => hoursRange; set => hoursRange = value; }
        public List<int> MinutesRange { get => minutesRange; set => minutesRange = value; }
        public List<int> DaysRange { get => daysRange; set => daysRange = value; }


        public event EventHandler<EventArgs> OnRequestCloseForm;

        private RelayCommand saveResultsCommand;


        private bool IsFormValid()
        {
            if (string.IsNullOrEmpty(nameValue))
            {
                MessageBox.Show("Wypełnij puste pola.", "Błąd");
                return false;
            }

            if(selectedRecipients == null || selectedMessage == null)
            {
                MessageBox.Show("Wybierz odbiorców i/lub wiadomość.", "Błąd");
                return false;
            }

            if(!isOneTime && !isCyclical)
            {
                MessageBox.Show("Wybierz rodzaj zadania.", "Błąd");
                return false;
            }

            if(isCyclical && selectedDays == 0)
            {
                MessageBox.Show("Wypełnij liczbę dni do powtórzenia zadania.", "Błąd");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Command saves the results of filling a for m into tasks collection.
        /// </summary>
        public RelayCommand SaveResultsCommand
        {
            get
            {
                return saveResultsCommand ??
                    (saveResultsCommand = new RelayCommand(obj =>
                    {
                        if (!IsFormValid())
                            return;

                        TaskType taskType = isOneTime ? TaskType.OneTime : TaskType.Cyclical;
                        DateTime dateTime = this.selectedDate.AddHours(SelectedHours).AddMinutes(SelectedMinutes);
                        ScheduledTask newTask = new ScheduledTask(this.nameValue,this.selectedRecipients.Id,this.selectedMessage.Id,dateTime,taskType,this.selectedDays);

                        if (this.selectedTask == null)
                                ObservableCollectionCRUD<ScheduledTask>.AddToCollection(newTask,
                                    DataModelSingleton.getInstance().ModelSet.Tasks);
                        else
                            ObservableCollectionCRUD<ScheduledTask>.UpdateCollection(newTask, this.selectedTask);
                       
                        OnRequestCloseForm(this, new EventArgs());
                    }));
            }
        }


    }
}
