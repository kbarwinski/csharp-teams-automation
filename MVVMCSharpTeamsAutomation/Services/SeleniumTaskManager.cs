using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System.Threading;
using System.Windows.Data;
using System.Windows.Threading;
using MVVMCSharpTeamsAutomation.Model.Helpers;
using MVVMCSharpTeamsAutomation.Services.Helpers;

namespace MVVMCSharpTeamsAutomation.Model
{

    /// <summary>
    /// Class which merges program's model and web driver to execute messaging tasks.
    /// Implements IDisposable for disposing web driver, timer and semaphore.
    /// </summary>
    public class SeleniumTaskManager : IDisposable
    {
        private DataModelSingleton settingsSingleton;
        private SeleniumSingleton seleniumSingleton;

        /// <summary>
        /// Assures only one timer thread is accessing Selenium web driver to avoid potential hazard.
        /// </summary>
        private readonly SemaphoreSlim messagesSemaphore;
        /// <summary>
        /// Runs the process of executing due scheduled tasks every 60 seconds on a seperate thread.
        /// </summary>
        private Timer checkTasksTimer;
        /// <summary>
        /// UI-related thread used for refreshing the view from a timer thread.
        /// </summary>
        private Dispatcher dispatcher;

        public SeleniumTaskManager()
        {
            this.settingsSingleton = DataModelSingleton.getInstance();
            this.seleniumSingleton = SeleniumSingleton.getInstance();
            this.messagesSemaphore = new SemaphoreSlim(1);

        }

        /// <summary>
        /// Initializes timer to check tasks for completion.
        /// </summary>
        public void InitTaskChecking()
        {
            this.seleniumSingleton.TeamsLogin(this.settingsSingleton.ModelSet.Credentials);
            AutoResetEvent timerEvent = new AutoResetEvent(true);
            this.checkTasksTimer = new Timer(this.CheckForCompletion, timerEvent, 10000, 60000);

            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Checks every task from collection for completion.
        /// If it's ready, Selenium attempts to complete a scheduled task.
        /// Task is deleted or updated for repetition on Dispatcher thread on success, on failure it's omitted.
        /// </summary>
        /// <param name="stateInfo"></param>
        private void CheckForCompletion(Object stateInfo)
        {
            messagesSemaphore.Wait();
            for (int i = settingsSingleton.ModelSet.Tasks.Count - 1; i >= 0; i--)
            {
                ScheduledTask? currentTask = settingsSingleton.ModelSet.Tasks.ElementAt(i);
                if (currentTask.ScheduledDate <= DateTime.Now)
                {
                    try
                    {
                        this.SendMessages(currentTask);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    this.dispatcher.Invoke(new Action(() => {
                        if (currentTask.TaskType == TaskType.OneTime)
                            settingsSingleton.ModelSet.Tasks.Remove(currentTask);
                        else
                            currentTask.ScheduledDate = DateTime.Now.AddDays(currentTask.DaysToRepeat);
                    }));

                }
            }
            messagesSemaphore.Release();
        }

        /// <summary>
        /// Separates each line of recipients object contents into an array and iterates over it.
        /// Each line is treated by Selenium as message recipient.
        /// </summary>
        /// <param name="scheduledTask">Task to execute.</param>
        /// 

        private string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        private void SendMessages(ScheduledTask scheduledTask)
        {

            String[] recipientsArr = settingsSingleton.GetRecipientsById(scheduledTask.RecipientsId).Content.Split(";", StringSplitOptions.RemoveEmptyEntries);
            string message = settingsSingleton.GetMessageById(scheduledTask.MessageId).Content;

            try
            {
                foreach (string recipient in recipientsArr)
                {
                    seleniumSingleton.SendMessage(RemoveWhitespace(recipient), message);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            this.messagesSemaphore.Dispose();
            this.checkTasksTimer.Dispose();
            this.seleniumSingleton.Dispose();
        }
    }

}
