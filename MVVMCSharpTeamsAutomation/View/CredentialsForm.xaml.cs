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
    /// Interaction logic for CredentialsForm.xaml
    /// </summary>
    public partial class CredentialsForm : Window
    {
        public CredentialsForm()
        {
            InitializeComponent();
        }
        public CredentialsForm(Credentials credentials)
        {
            CredentialsFormViewModel vm = new CredentialsFormViewModel(credentials);
            this.DataContext = vm;
            vm.OnRequestCloseForm += (s, e) =>
            {
                this.Close();
            };
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).PasswordText = ((PasswordBox)sender).Password; }
        }
    }
}
