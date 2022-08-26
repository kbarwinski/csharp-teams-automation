using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.Model
{
    /// <summary>
    /// Class represents a list of recipients or a message to be sent.
    /// </summary>
    public class TaskItem : INotifyPropertyChanged
    {
        private Guid id;
        private string name;
        private string content;
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

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged("Content");
            }
        }

        private TaskItem() 
        {
        }

        public TaskItem(string name, string content)
        {
            this.id= Guid.NewGuid();
            this.name = name;
            this.content = content;
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
