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
    /// Interaction logic for TaskForm.xaml.
    /// </summary>
    public partial class TaskForm : Window
    {
        /// <summary>
        /// Constructor initializes handling OnRequestCloseForms events on the view side fired by the viewmodel.
        /// Event is responsible for closing the form after submitting the data.
        /// </summary>
        /// <param name="task">If it's null, viewmodel creates a new task and adds it to the list. If it's not, viewmodel edits an existing task.</param>
        public TaskForm(ScheduledTask? task)
        {
            TaskFormViewModel vm = new TaskFormViewModel(task);
            this.DataContext = vm;
            vm.OnRequestCloseForm += (s, e) =>
            {
                this.Close();
            };
            InitializeComponent();
        }

    }
}
