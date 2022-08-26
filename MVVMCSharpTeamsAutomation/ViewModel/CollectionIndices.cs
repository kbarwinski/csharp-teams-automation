using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.ViewModel
{
    /// <summary>
    /// Class contains item selection logic for MainWindowViewModel object.
    /// Through storing selected indices of each list and PropertyChanged notifications, only one item from every list can be selected.
    /// Class main purpose is to simplify logic for deleting items.
    /// </summary>
    public class CollectionIndices : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int recipientsIndex = -1;
        private int messagesIndex = -1;
        private int tasksIndex = -1;
        public int RecipientsIndex
        {
            get { return recipientsIndex; }
            set
            {
                recipientsIndex = value;
                messagesIndex = -1;
                tasksIndex = -1;
                OnPropertyChanged("MessagesIndex");
                OnPropertyChanged("TasksIndex");
            }
        }

        public int MessagesIndex
        {
            get { return messagesIndex; }
            set
            {
                messagesIndex = value;
                recipientsIndex = -1;
                tasksIndex = -1;
                OnPropertyChanged("RecipientsIndex");
                OnPropertyChanged("TasksIndex");
            }
        }

        public int TasksIndex
        {
            get { return tasksIndex; }
            set
            {
                tasksIndex = value;
                recipientsIndex = -1;
                messagesIndex = -1;
                OnPropertyChanged("RecipientsIndex");
                OnPropertyChanged("MessagesIndex");
            }
        }

        public CollectionIndices()
        {

        }
    }
}
