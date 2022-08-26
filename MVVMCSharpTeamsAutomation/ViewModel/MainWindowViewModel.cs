using MVVMCSharpTeamsAutomation.Model;
using MVVMCSharpTeamsAutomation.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace MVVMCSharpTeamsAutomation.ViewModel
{
    /// <summary>
    /// Responsible for maintaining program's navigation logic. 
    /// </summary>
    public class MainWindowViewModel
    {

        private ExtendedObservableCollection<TaskItem> recipients;
        private ExtendedObservableCollection<TaskItem> messages;
        private ExtendedObservableCollection<ScheduledTask> tasks;
        public ExtendedObservableCollection<TaskItem> Recipients
        {
            get { return recipients; }
        }

        public ExtendedObservableCollection<TaskItem> Messages
        {
            get { return messages; }
        }

        public ExtendedObservableCollection<ScheduledTask> Tasks
        {
            get { return tasks; }
        }

        private CollectionIndices collectionIndices;

        public CollectionIndices CollectionIndices
        {
            get { return collectionIndices; }
        }

        private SeleniumTaskManager seleniumTaskManager;

        public event EventHandler<WindowEventArgs> OnRequestOpenForm;

        /// <summary>
        /// Custom event arguments class. Allows view to discern what kind of form to open and possibly pass the selected item to it.
        /// </summary>
        public class WindowEventArgs : EventArgs
        {
            private string title;
            private object payload;
            public string Title
            {
                get { return title; }
                set { title = value; }
            }

            public object Payload
            {
                get { return payload; }
                set { payload = value; }
            }

            public WindowEventArgs(string title, object payload)
            {
                this.title = title;
                this.payload = payload;
            }
        }

        private RelayCommand openFormCommand;

        /// <summary>
        /// Command responsible for firing events to the view, so it can open a form corresponding to it's controls.
        /// </summary>
        public RelayCommand OpenFormCommand
        {
            get
            {
                return openFormCommand ??
                    (openFormCommand = new RelayCommand(obj =>
                    {
                        string btnType = obj as string;
                        switch (btnType)
                        {
                            case "newRecipients":
                                OnRequestOpenForm(this, new WindowEventArgs("newRecipients",null));
                                break;
                            case "newMessage":
                                OnRequestOpenForm(this, new WindowEventArgs("newMessage",null));
                                break;
                            case "editRecipients":
                                if(this.collectionIndices.RecipientsIndex != -1)
                                    OnRequestOpenForm(this, new WindowEventArgs("editRecipients", Recipients.ElementAt(this.collectionIndices.RecipientsIndex)));
                                break;
                            case "editMessage":
                                if(this.collectionIndices.MessagesIndex != -1)
                                    OnRequestOpenForm(this, new WindowEventArgs("editMessage", Messages.ElementAt(this.collectionIndices.MessagesIndex)));
                                break;
                            case "newTask":
                                OnRequestOpenForm(this, new WindowEventArgs("newTask", null));
                                break;
                            case "editTask":
                                if (this.collectionIndices.TasksIndex != -1)
                                    OnRequestOpenForm(this, new WindowEventArgs("editTask", Tasks.ElementAt(this.collectionIndices.TasksIndex)));
                                break;
                            case "credentials":
                                OnRequestOpenForm(this, new WindowEventArgs("credentials", DataModelSingleton.getInstance().ModelSet.Credentials));
                                break;
                            default:
                                break;
                        }
                    }));
            }
        }

        private RelayCommand deleteItemCommand;

        /// <summary>
        /// Command responsible for deletion of selected item from collection.
        /// </summary>
        public RelayCommand DeleteItemCommand
        {
            get
            {
                return deleteItemCommand ??
                    (deleteItemCommand = new RelayCommand(obj =>
                    {
                        if(this.collectionIndices.RecipientsIndex != -1)
                        {
                            Guid deleteGuid = Recipients.ElementAt(this.collectionIndices.RecipientsIndex).Id;
                            for(int i=Tasks.Count-1; i>=0; i--)
                            {
                                if (Tasks.ElementAt(i).RecipientsId == deleteGuid)
                                    ObservableCollectionCRUD<ScheduledTask>.DeleteFromCollection(Tasks, i);
                            }
                            ObservableCollectionCRUD<TaskItem>.DeleteFromCollection(Recipients, this.collectionIndices.RecipientsIndex);
                        }
                        if (this.collectionIndices.MessagesIndex != -1)
                        {
                            Guid deleteGuid = Messages.ElementAt(this.collectionIndices.MessagesIndex).Id;
                            for (int i = Tasks.Count-1; i >= 0; i--)
                            {
                                if (Tasks.ElementAt(i).MessageId == deleteGuid)
                                    ObservableCollectionCRUD<ScheduledTask>.DeleteFromCollection(Tasks, i);
                            }
                            ObservableCollectionCRUD<TaskItem>.DeleteFromCollection(Messages, this.collectionIndices.MessagesIndex);
                        }
                        if (this.collectionIndices.TasksIndex != -1)
                            ObservableCollectionCRUD<ScheduledTask>.DeleteFromCollection(Tasks,this.collectionIndices.TasksIndex);
                    }));
            }
        }

        public MainWindowViewModel()
        {
            this.collectionIndices = new CollectionIndices();
            DataModelSingleton dataModelSingleton = DataModelSingleton.getInstance();
            this.recipients = dataModelSingleton.ModelSet.Recipients;
            this.messages = dataModelSingleton.ModelSet.Messages;
            this.tasks = dataModelSingleton.ModelSet.Tasks;
        }
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Zapisać dane?", "Alert", System.Windows.MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DataModelSingleton.getInstance().SaveToXML();
            }
            seleniumTaskManager.Dispose();
        }

        public void OnWindowLoaded(object sender, EventArgs e)
        {
            try
            {
                DataModelSingleton.getInstance().LoadFromXML();
            }
            catch (XmlException)
            {
                OnRequestOpenForm(this, new WindowEventArgs("credentials", DataModelSingleton.getInstance().ModelSet.Credentials));
            }
            this.seleniumTaskManager = new SeleniumTaskManager();
            this.seleniumTaskManager.InitTaskChecking();
        }

    }
}
