using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using MVVMCSharpTeamsAutomation.Model;
using MVVMCSharpTeamsAutomation.ViewModel;

namespace MVVMCSharpTeamsAutomation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor initializes handling OnRequestOpenForms events on the view side fired by the viewmodel.
        /// Event is responsible for opening various forms required for program's functionality.
        /// </summary>
        public MainWindow()
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            this.DataContext = vm;
            vm.OnRequestOpenForm += (s, e) =>
            {
                ItemForm itemForm;
                switch (e.Title)
                {
                    case "newRecipients":
                        new ItemForm(null, false).ShowDialog();
                        break;
                    case "newMessage":
                        new ItemForm(null, true).ShowDialog();
                        break;
                    case "editRecipients":
                        new ItemForm((TaskItem?)e.Payload, false).ShowDialog();
                        break;
                    case "editMessage":
                        new ItemForm((TaskItem?)e.Payload, true).ShowDialog();
                        break;
                    case "newTask":
                        new TaskForm(null).ShowDialog();
                        break;
                    case "editTask":
                        new TaskForm((ScheduledTask?)e.Payload).ShowDialog();
                        break;
                    case "credentials":
                        new CredentialsForm((Credentials)e.Payload).ShowDialog();
                        break;
                    case null:
                        break;
                }
            };

            Closing += vm.OnWindowClosing;
            Loaded += vm.OnWindowLoaded;
            InitializeComponent();
        }
    }
}
