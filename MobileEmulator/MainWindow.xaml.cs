using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium.Support;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Remote;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = objAccountCredsBinding;         
        }



        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion

        #region Variables

        private int _RunningDeviceCount = 0;

        public int RunningDeviceCount
        {
            get
            {
                return _RunningDeviceCount;
            }
            set
            {
                _RunningDeviceCount = value;
                OnPropertyChanged("RunningDeviceCount");
            }
        }


        private AccountCreds _objAccountCredsBinding = new AccountCreds();

        public AccountCreds objAccountCredsBinding
        {
            get
            {
                return _objAccountCredsBinding;
            }
            set
            {
                _objAccountCredsBinding = value;
                OnPropertyChanged("objAccountCredsBinding");
            }
        }

        #endregion

        #region Method : RunEmulator

        #region Description

        // This method is used to run the mobile emulator to navigate to facebook page with their account creds
        // along with useragent, mobile devices and proxy settings

        #endregion

        public void RunEmulator(AccountCreds objAccountCreds)
        {
            try
            {
                ChromeOptions objChromeOptions = new ChromeOptions();

                //Set the mobile devices 
                 objChromeOptions.EnableMobileEmulation(objAccountCreds.MobileDeviceName);

                //Add the extensions
                objChromeOptions.AddExtension(@"D:\AndroidEmulator\Test\MobileEmulator\bin\Debug\SessionBox.crx");

                //Set the User agent
                objChromeOptions.AddArgument(objAccountCreds.Useragent);

                #region some other useragent

                //objChromeOptions.AddArgument(@"user-agent=""Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25""");

                //objChromeOptions.AddArgument(@"user-agent=""Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25""");

                //Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25

                //Mozilla/5.0 (Linux; U; Android 2.2.1; en-us; Nexus One Build/FRG83) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1
                           
                //@"user-agent=""YOUR_USER_AGENT"""
        
                // "--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25"
                // "--user-agent=Mozilla/5.0 (Linux; Android 6.0; HTC One M9 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36"

                #endregion

                //Set the proxy to the Chrome options
                #region Set Proxy

                if (!string.IsNullOrEmpty(objAccountCreds.ProxyAddress))
                {

                    var proxy = new Proxy();

                    proxy.Kind = ProxyKind.Manual;

                    proxy.IsAutoDetect = false;

                    proxy.HttpProxy = proxy.SslProxy = objAccountCreds.ProxyAddress + ":" + objAccountCreds.ProxyPort;

                    objChromeOptions.Proxy = proxy;

                    objChromeOptions.AddArgument("ignore-certificate-errors");

                   

                }

                #endregion


                // Connect with yours chrome driver (location of chrome driver)                
                ChromeDriverService objChromeDriverService = ChromeDriverService.CreateDefaultService();

                // set true for hide the command prompt window when chrome is initailized
                objChromeDriverService.HideCommandPromptWindow = true;




                #region Navigate to facebook with creds

                IWebDriver objChromeDriver = new ChromeDriver(objChromeDriverService, objChromeOptions);

                // Navigate to facebook page
                objChromeDriver.Navigate().GoToUrl("https://www.facebook.com");

                System.Threading.Thread.Sleep(2000);

                //check the page source of the facebook which contains input area for provide creds
                if (objChromeDriver.PageSource.Contains("id=\"email\""))
                {
                    try
                    {
                        // Clear the email input box
                        objChromeDriver.FindElement(By.Id("email")).Clear();
                        System.Threading.Thread.Sleep(500);

                        // Pass the email to the input box
                        objChromeDriver.FindElement(By.Id("email")).SendKeys(objAccountCreds.Username);
                        System.Threading.Thread.Sleep(500);

                        // Clear the password input box
                        objChromeDriver.FindElement(By.Id("pass")).Clear();
                        System.Threading.Thread.Sleep(500);

                        // Pass the password to the input box
                        objChromeDriver.FindElement(By.Id("pass")).SendKeys(objAccountCreds.Password);
                        System.Threading.Thread.Sleep(500);

                        // Click the login button
                        objChromeDriver.FindElement(By.XPath("//input[@value='Log In']")).Click();

                    }
                    catch (Exception)
                    {

                    }
                }

                #endregion

            }
            catch (Exception)
            {

            }
        }

    

        #endregion

        #region Start Button

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            AccountCreds objAccountCreds = new AccountCreds();
            objAccountCreds.Username = objAccountCredsBinding.Username;
            objAccountCreds.Password = objAccountCredsBinding.Password;
            objAccountCreds.ProxyAddress = objAccountCredsBinding.ProxyAddress;
            objAccountCreds.ProxyPort = objAccountCredsBinding.ProxyPort;
            objAccountCreds.ProxyUsername = objAccountCredsBinding.ProxyUsername;
            objAccountCreds.ProxyPassword = objAccountCredsBinding.ProxyPassword;
            objAccountCreds.MobileDeviceName = objAccountCredsBinding.MobileDeviceName;
            objAccountCreds.Useragent = objAccountCredsBinding.Useragent;
            RunningDeviceCount++;

            Thread objThread = new Thread(() => { RunEmulator(objAccountCreds); });
            objThread.Name = "Device " + RunningDeviceCount.ToString();
            objThread.IsBackground = true;
            objThread.SetApartmentState(ApartmentState.STA);
            objThread.Start();


            objAccountCredsBinding = new AccountCreds();
            this.DataContext = objAccountCredsBinding;
        }

        #endregion

    }

    #region Class : Account Creds

    public class AccountCreds
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion

        #region Fields


        private string _Username = string.Empty;

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                OnPropertyChanged("Username");
            }
        }


        private string _Password = string.Empty;

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }



        private string _ProxyAddress = string.Empty;

        public string ProxyAddress
        {
            get
            {
                return _ProxyAddress;
            }
            set
            {
                _ProxyAddress = value;
                OnPropertyChanged("ProxyAddress");
            }
        }


        private int _ProxyPort = 80;

        public int ProxyPort
        {
            get
            {
                return _ProxyPort;
            }
            set
            {
                _ProxyPort = value;
                OnPropertyChanged("ProxyPort");
            }
        }


        private string _ProxyUsername = string.Empty;

        public string ProxyUsername
        {
            get
            {
                return _ProxyUsername;
            }
            set
            {
                _ProxyUsername = value;
                OnPropertyChanged("ProxyUsername");
            }
        }


        private string _ProxyPassword = string.Empty;

        public string ProxyPassword
        {
            get
            {
                return _ProxyPassword;
            }
            set
            {
                _ProxyPassword = value;
                OnPropertyChanged("ProxyPassword");
            }
        }


        private string _MobileDeviceName = "Google Nexus 5";

        public string MobileDeviceName
        {
            get
            {
                return _MobileDeviceName;
            }
            set
            {
                _MobileDeviceName = value;
                OnPropertyChanged("MobileDeviceName");
            }
        }


        private string _Useragent = "Mozilla/5.0 (Linux; Android 4.2.1; en-us; Nexus 5 Build/JOP40D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19";

        public string Useragent
        {
            get
            {
                return _Useragent;
            }
            set
            {
                _Useragent = value;
                OnPropertyChanged("Useragent");
            }
        }


        #endregion

        #region Constructor

        public AccountCreds()
        {

        }

        public AccountCreds(string Username, string Password, string ProxyAddress, int ProxyPort, string ProxyUsername, string ProxyPassword, string MobileDeviceName, string Useragent) : this()
        {

            this.Username = Username;
            this.Password = Password;
            this.ProxyAddress = ProxyAddress;
            this.ProxyPort = ProxyPort;
            this.ProxyUsername = ProxyUsername;
            this.ProxyPassword = ProxyPassword;
            this.MobileDeviceName = MobileDeviceName;
            this.Useragent = Useragent;
        }

        #endregion
    }

    #endregion

}
