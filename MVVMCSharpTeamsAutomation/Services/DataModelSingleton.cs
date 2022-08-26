using MVVMCSharpTeamsAutomation.Model;
using MVVMCSharpTeamsAutomation.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace MVVMCSharpTeamsAutomation.Model
{
    public class DataModelSingleton
    {
        private static DataModelSingleton _instance;

        private DataModel modelSet;

        public DataModel ModelSet
        {
            get { return modelSet; }
            set { modelSet = value; }
        }

        public DataModelSingleton()
        {
            this.modelSet = new DataModel();
        }
        public static DataModelSingleton getInstance()
        {
            if(_instance == null )
                _instance = new DataModelSingleton();
            return _instance;
        }
        private void LoadToList<T>(ObservableCollection<T> sourceList, ObservableCollection<T> targetList)
        {
            foreach(T item in sourceList)
            {
                targetList.Add(item);
            }
        }
        public void LoadFromXML()
        {
            try
            {
                this.modelSet.Credentials = XMLObjectConverter<Credentials>.ConvertFromXML("credentials.xml","Credentials");
                LoadToList(XMLObjectConverter<ObservableCollection<TaskItem>>.ConvertFromXML("recipients.xml","ArrayOfTaskItem"), this.modelSet.Recipients);
                LoadToList(XMLObjectConverter<ObservableCollection<TaskItem>>.ConvertFromXML("messages.xml","ArrayOfTaskItem"), this.modelSet.Messages);
                LoadToList(XMLObjectConverter<ObservableCollection<ScheduledTask>>.ConvertFromXML("tasks.xml","ArrayOfScheduledTask"), this.modelSet.Tasks);
            }
            catch(XmlException ex)
            {
                throw new XmlException(ex.Message);
            }
        }

        public void SaveToXML()
        {
            XMLObjectConverter<Credentials>.ConvertToXML(this.modelSet.Credentials, "credentials.xml", "Credentials");
            XMLObjectConverter<ObservableCollection<TaskItem>>.ConvertToXML(this.modelSet.Recipients, "recipients.xml", "ArrayOfTaskItem");
            XMLObjectConverter<ObservableCollection<TaskItem>>.ConvertToXML(this.modelSet.Messages, "messages.xml", "ArrayOfTaskItem");
            XMLObjectConverter<ObservableCollection<ScheduledTask>>.ConvertToXML(this.modelSet.Tasks, "tasks.xml", "ArrayOfScheduledTask");
        }

        public TaskItem GetMessageById(Guid guid)
        {
            return this.modelSet.Messages.First(m => m.Id == guid);
        }

        public TaskItem GetRecipientsById(Guid guid)
        {
            return this.modelSet.Recipients.First(m => m.Id == guid);
        }

        private void PlaceholderValues()
        {
            TaskItem item1 = new TaskItem("test1", "kajetan.barwinski2@outlook.com\r\nkajetan.barwinski3@outlook.com");
            TaskItem item2 = new TaskItem("test2", "kajetan.barwinski2@outlook.com\r\nkajetan.barwinski@gmail.com");
            TaskItem item3 = new TaskItem("test3", "kajetan.barwinski3@outlook.com\r\nkajetan.barwinski@gmail.com");
            TaskItem item4 = new TaskItem("test4", "testtest4");
            TaskItem item5 = new TaskItem("test5", "testtest5");
            TaskItem item6 = new TaskItem("test6", "testtest6");

            ScheduledTask task1 = new ScheduledTask("test7", item1.Id, item4.Id, DateTime.Now, TaskType.OneTime);
            ScheduledTask task2 = new ScheduledTask("test8", item2.Id, item5.Id, DateTime.Now.AddMinutes(1), TaskType.OneTime);
            ScheduledTask task3 = new ScheduledTask("test9", item3.Id, item6.Id, DateTime.Now.AddMinutes(2), TaskType.Cyclical, 7);

            this.modelSet.Recipients.Add(item1);
            this.modelSet.Recipients.Add(item2);
            this.modelSet.Recipients.Add(item3);

            this.modelSet.Messages.Add(item4);
            this.modelSet.Messages.Add(item5);
            this.modelSet.Messages.Add(item6);

            this.modelSet.Tasks.Add(task1);
            this.modelSet.Tasks.Add(task2);
            this.modelSet.Tasks.Add(task3);

            this.modelSet.Credentials = new Credentials("kajetan.barwinski@outlook.com","RPKRPK123");
        }
    }
}
