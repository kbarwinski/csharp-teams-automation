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
    /// Implements recipients and messages form logic.
    /// </summary>
    public class ItemFormViewModel
    {
        private bool isMessage;
        private TaskItem taskItem;

        private string nameText;
        private string contentText;

        public string NameText
        {
            get { return nameText; }
            set { nameText = value; }
        }

        public string ContentText
        {
            get { return contentText; }
            set { contentText = value; }
        }


        public ItemFormViewModel(TaskItem? task, bool isMessage)
        {
            if(task!=null)
            {
                this.nameText = task.Name;
                this.contentText = task.Content;
            }
            this.taskItem = task;
            this.isMessage = isMessage;
        }

        public event EventHandler<EventArgs> OnRequestCloseForm;

        private RelayCommand saveResultsCommand;


        private bool IsFormValid()
        {
            if (string.IsNullOrEmpty(nameText) || string.IsNullOrEmpty(contentText))
            {
                MessageBox.Show("Wypełnij puste pola.", "Błąd");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Command saves the results of filling a form into corresponding collection.
        /// </summary>
        /// 
        public RelayCommand SaveResultsCommand
        {
            get
            {
                return saveResultsCommand ??
                    (saveResultsCommand = new RelayCommand(obj =>
                    {
                        if (!IsFormValid())
                            return;

                        TaskItem newTask = new TaskItem(this.nameText, this.contentText);
                        if (this.taskItem == null)
                        {
                            if (isMessage)
                                ObservableCollectionCRUD<TaskItem>.AddToCollection(newTask, 
                                    DataModelSingleton.getInstance().ModelSet.Messages);
                            else
                                ObservableCollectionCRUD<TaskItem>.AddToCollection(newTask,
                                    DataModelSingleton.getInstance().ModelSet.Recipients);
                        }
                        else
                        {
                            ObservableCollectionCRUD<TaskItem>.UpdateCollection(newTask, this.taskItem); 
                        }
                        OnRequestCloseForm(this, new EventArgs());
                    }));
            }
        }
    }
}
