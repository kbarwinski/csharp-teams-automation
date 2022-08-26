using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace MVVMCSharpTeamsAutomation.Model
{
    /// <summary>
    /// Singleton class containing instance of selenium web driver.
    /// Implements IDisposable interface for proper closing of driver when needed.
    /// Contains methods for handling driver tasks.
    /// </summary>
    public class SeleniumSingleton : IDisposable
    {
        private static SeleniumSingleton _instance;
        private WebDriverWait newPageWait;
        private WebDriverWait clickWait;


        private IWebDriver driver;
        public IWebDriver Driver
        {
            get { return driver; }
            set { driver = value; }
        }
        public SeleniumSingleton()
        {
            this.driver = new ChromeDriver();
            this.newPageWait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(30));
            this.clickWait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(10));
        }

        public static SeleniumSingleton getInstance()
        {
            if (_instance == null)
                _instance = new SeleniumSingleton();
            return _instance;
        }


        /// <summary>
        /// Sends Teams message through interaction with it's web app.
        /// </summary>
        /// <param name="recipient">Recipient of message.</param>
        /// <param name="message">Contents of message to send.</param>
        /// <exception cref="Exception">Catches and throws further any undefined problems.</exception>
        public void SendMessage(string recipient, string message)
        {

                Actions keySender = new Actions(driver);
                //Navigating to new chat
                driver.Navigate().GoToUrl("https://teams.live.com/_#/conversations/newchat?ctx=chat");
                Thread.Sleep(5000);

                //Searching for recipient
                keySender.SendKeys(recipient).Perform();
                Thread.Sleep(2000);

                //Navigating to message field
                keySender.SendKeys(Keys.Enter).Perform();
                Thread.Sleep(2000);
                keySender.SendKeys(Keys.Tab).Perform();
                Thread.Sleep(1500);

                //Sending message
                keySender.SendKeys(message).Perform();
                Thread.Sleep(1500);
                keySender.SendKeys(Keys.Enter).Perform();
                Thread.Sleep(1000);

        }

        /// <summary>
        /// Logs in to teams web app through web driver interactions.
        /// </summary>
        /// <param name="credentials">Contains e-mail and password of the user to log-in.</param>
        /// <exception cref="Exception">Catches and throws further any undefined problems.</exception>
        public void TeamsLogin(Credentials credentials)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.microsoft.com/microsoft-teams/log-in");
                driver.Manage().Window.Maximize();
                //Navigating to the login page
                IWebElement currElement = newPageWait.Until(e => e.FindElement(By.XPath("/html/body/section/div[2]/div/div/div[3]/section/div/div[2]/div[1]/a")));
                currElement.Click();
                driver.SwitchTo().Window(driver.WindowHandles[1]);
                Thread.Sleep(1500);

                string mail = credentials.Email;
                string password = credentials.Password;

                //Filling the email field
                currElement = clickWait.Until(e => e.FindElement(By.Id("i0116")));
                currElement.SendKeys(mail);

                //Navigating to the password field
                currElement = clickWait.Until(e => e.FindElement(By.Id("idSIButton9")));
                currElement.Click();
                Thread.Sleep(3000);

                //Filling the password field
                currElement = clickWait.Until(e => e.FindElement(By.Id("i0118")));
                currElement.SendKeys(password);


                //Logging in
                currElement = clickWait.Until(e => e.FindElement(By.Id("idSIButton9")));
                currElement.Click();
                currElement = clickWait.Until(e => e.FindElement(By.Id("idBtn_Back")));
                currElement.Click();
                Thread.Sleep(10000);

                //Navigating to Teams web app
                currElement = newPageWait.Until(e => e.FindElement(By.ClassName("use-app-lnk")));
                currElement.Click();
            }
            catch(Exception ex)
            {
                throw;
            }
   
            
        }

        public void Dispose()
        {
            this.driver.Close();
            this.driver.Quit();
        }
    }
}
