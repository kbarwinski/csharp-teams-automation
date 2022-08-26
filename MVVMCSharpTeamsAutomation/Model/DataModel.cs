using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.Model
{
    public class DataModel
    { 

        private ExtendedObservableCollection<TaskItem> recipients;
        private ExtendedObservableCollection<TaskItem> messages;
        private ExtendedObservableCollection<ScheduledTask> tasks;
        private Credentials credentials;

        public ExtendedObservableCollection<TaskItem> Recipients
        {
            get { return recipients; }
            set { recipients = value; }
        }

        public ExtendedObservableCollection<TaskItem> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        public ExtendedObservableCollection<ScheduledTask> Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        public DataModel()
        {
            this.recipients = new ExtendedObservableCollection<TaskItem>();
            this.messages = new ExtendedObservableCollection<TaskItem>();
            this.tasks = new ExtendedObservableCollection<ScheduledTask>();
            this.credentials = new Credentials();
        }

    }
}
