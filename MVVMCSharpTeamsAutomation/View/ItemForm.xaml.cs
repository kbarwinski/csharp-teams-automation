using MVVMCSharpTeamsAutomation.Model;
using MVVMCSharpTeamsAutomation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MVVMCSharpTeamsAutomation
{
    /// <summary>
    /// Interaction logic for ItemForm.xaml
    /// </summary>
    /// 
    
    public partial class ItemForm : Window
    {
        /// <summary>
        /// Constructor initializes handling OnRequestCloseForms events on the view side fired by the viewmodel.
        /// Event is responsible for closing the form after submitting the data.
        /// </summary>
        /// <param name="taskItem">If it's null viewmodel creates a new task and adds it to the list. If it's not, viewmodel edits an existing task.</param>
        /// <param name="isMessage">If it's true, viewmodel operates on the list of messages. If it's false, it means a form relates to recipients.</param>
        public ItemForm(TaskItem? taskItem, bool isMessage)
        {
            ItemFormViewModel vm = new ItemFormViewModel(taskItem,isMessage);
            this.DataContext = vm;
            vm.OnRequestCloseForm += (s, e) =>
            {
                this.Close();
            };
            InitializeComponent();
        }
    }
}
