using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.Model
{
    public enum TaskType{
        OneTime,
        Cyclical
    }

    /// <summary>
    /// Class representing tasks scheduled for completion.
    /// </summary>
    public class ScheduledTask : INotifyPropertyChanged
    {
        private Guid id;
        private string name;
        private Guid recipientsId;
        private Guid messageId;
        private DateTime scheduledDate;

        private TaskType taskType;
        private int daysToRepeat;

        public Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }

        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public Guid RecipientsId
        {
            get { return recipientsId; }
            set 
            { 
                recipientsId = value;
                OnPropertyChanged("RecipientsId");
            }
        }

        public Guid MessageId
        {
            get { return messageId; }
            set
            {
                messageId = value;
                OnPropertyChanged("MessageId");
            }
        }

        public DateTime ScheduledDate
        {
            get { return scheduledDate; }
            set
            {
                scheduledDate = value;
                OnPropertyChanged("ScheduledDate");
                OnPropertyChanged("StrRepresentation");
            }
        }

        public TaskType TaskType
        {
            get { return taskType; }
            set 
            { 
                taskType = value;
                OnPropertyChanged("TaskType");
                OnPropertyChanged("StrRepresentation");
            }
        }

        public int DaysToRepeat
        {
            get { return daysToRepeat; }
            set 
            { 
                daysToRepeat = value;
                OnPropertyChanged("DaysToRepeat");
                OnPropertyChanged("StrRepresentation");
            }
        }

        private string strRepresentation;
        public string StrRepresentation
        {
            get
            {
                string strRepresentation = this.name + " " + scheduledDate.ToString("MM/dd/yyyy HH:mm");
                if (this.TaskType == TaskType.Cyclical)
                    strRepresentation += " Co " + this.daysToRepeat.ToString() + " dni";
                return strRepresentation;
            }
            set
            {
                strRepresentation = value;
                OnPropertyChanged("StrRepresentation");
            }
        }

        private ScheduledTask()  
        {
        }

        public ScheduledTask(String name, Guid recipientsId, Guid messageId, DateTime scheduledDate, TaskType taskType, int daysToRepeat = -1)
        {
            this.id = Guid.NewGuid();
            this.name = name;
            this.recipientsId = recipientsId;
            this.messageId = messageId;
            this.scheduledDate = scheduledDate;
            this.taskType = taskType;
            this.daysToRepeat = daysToRepeat;


        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
