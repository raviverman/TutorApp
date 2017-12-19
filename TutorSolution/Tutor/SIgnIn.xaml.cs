using Newtonsoft.Json.Linq;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for SIgnIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignIn()
        {
            InitializeComponent();
        }
        
        public MainWindow CallerWindow { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorTextBlock.Text = "";
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();

            }
            catch (Exception)
            { }
        }

        private async void sendSigninRequestAsync()
        {
            errorTextBlock.Text = "";
            string username = usernametextBlock.Text.Trim();
            string password = passwordTextBlock.Password.Trim();
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrEmpty(password))
            {
                errorTextBlock.Text = "Fill all fields.";
                return;
            }

            progressRing.isActive = true;

            Object resp = new JObject();
            resp = await ConnectionManager.SendRequestAsync(new Model.Data.SignIn() { Username = username, Password = Security.MD5Hash(password), Challenge = DateTime.Now.Ticks.ToString() }, RequestType.SignIn, ResponseType.SignIn);
            progressRing.isActive = false;
            if (resp == null)
            {
                errorTextBlock.Text = "Connection Error. Retry";
            }
            else
            {
                SignInResponse Response = ((JObject)resp).ToObject<SignInResponse>();
                if (Response.IsAccepted)
                {
                    progressRing.isActive = true;
                    await fetchAccountInfo(username, Response.SessionID);
                    AppNotificationManager.PushMessage(new AppNotification() { Message = SettingsManager.FullName + " signed in successfully." });

                    progressRing.isActive = false;
                    SettingsManager.AuthKey = Response.SessionID;
                    SettingsManager.IsLoggedIn = true;
                    SettingsManager.KeepLoggedIn = false;

                    if (loggedmeinCheckBox.IsChecked!=null && loggedmeinCheckBox.IsChecked==true)
                    {
                        SettingsManager.KeepLoggedIn = true;
                        SettingsManager.SaveSettings();
                    }
                    Close();
                }
                else
                {
                    errorTextBlock.Text = Response.Reason;
                }
            }
            //AppNotificationManager.PushMessage(new AppNotification() { Message = Response.SessionID });
        }

        private async Task fetchAccountInfo(string username, string ssid)
        {
            AccountDetails details = new AccountDetails() { Username = username, SessionID = ssid };
            Object resp = new JObject();
            resp = await ConnectionManager.SendRequestAsync(details, RequestType.AccountDetails, ResponseType.AccountDetails);
            if(resp!=null)
            {
                AccountDetailsResponse Response = ((JObject)resp).ToObject<AccountDetailsResponse>();
                SettingsManager.ProfilePath = "http://" + SettingsManager.ServerIp + "/root" + Response.ProfileImage;
                SettingsManager.AccountType = Response.AccType;
                SettingsManager.Username = Response.Username;
                SettingsManager.FullName = Response.FullName;
                CallerWindow.SetUpAccount(Response.Type);

            }

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount account = new CreateAccount();
            account.Show();
            if (!ConnectionManager.IsConnected)
            {
                errorTextBlock.Text = "Connection Error. Retry";
            }
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            sendSigninRequestAsync();
        }

        private void passwordTextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                signInButton_Click(null, null);
            }
        }
    }
}
