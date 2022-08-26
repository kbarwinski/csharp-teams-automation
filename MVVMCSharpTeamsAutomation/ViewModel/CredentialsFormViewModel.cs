using MVVMCSharpTeamsAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMCSharpTeamsAutomation.ViewModel
{
    /// <summary>
    /// Implements credentials form logic.
    /// </summary>
    public class CredentialsFormViewModel
    {
        private Credentials credentials;

        private string emailText;
        private string passwordText;
        public string EmailText { get => emailText; set => emailText = value; }
        public string PasswordText { get => passwordText; set => passwordText = value; }

        public CredentialsFormViewModel(Credentials credentials)
        {
            this.credentials = credentials;
        }

        public event EventHandler<EventArgs> OnRequestCloseForm;

        private RelayCommand saveResultsCommand;


        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private bool IsFormValid()
        {
            if(string.IsNullOrEmpty(emailText) || string.IsNullOrEmpty(passwordText))
            {
                MessageBox.Show("Wypełnij puste pola.", "Błąd");
                return false;
            }

            if (IsValidEmail(emailText))
            {
                MessageBox.Show("Wpisany email nie jest poprawny", "Błąd");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Command saves the results of filling a form into credentials object.
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
                        this.credentials.Email = emailText;
                        this.credentials.Password = passwordText;
                        OnRequestCloseForm(this, new EventArgs());
                    }));
            }
        }


    }
}

