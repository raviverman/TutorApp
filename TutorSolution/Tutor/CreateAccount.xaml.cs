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
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            progressRing.isActive = true;
            Object resp = new JObject();
            resp = await ConnectionManager.SendRequestAsync(new AvailUsername() { Username = usernameTextBlock.Text }, RequestType.UsernameAvailability, ResponseType.Acknowledge);
            if(resp!=null)
            {
                Acknowledge ack = ((JObject)resp).ToObject<Acknowledge>();
                errorTextBlock.Text = ack.Reason;
            }
            else
                errorTextBlock.Text = "Connection Error";
            progressRing.isActive = false;
        }

        private async void createAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid())
                return;
            progressRing.isActive = true;
            Model.Data.CreateAccount accountRequest = new Model.Data.CreateAccount()
            {
                FullName = fullNameTextBlock.Text.Trim(),
                Username = usernameTextBlock.Text.Trim(),
                Password = Security.MD5Hash(passwordTextBlock.Password.Trim()),
                Type = accounttextBlock.SelectedIndex == 0 ? UserType.Student : UserType.Professional
             };
            Object result = new JObject();
            result = await ConnectionManager.SendRequestAsync(accountRequest, RequestType.SignUp, ResponseType.Acknowledge);
            if (result != null)
            {
                Acknowledge ack = ((JObject)result).ToObject<Acknowledge>();
                if (ack.Status == "OK")
                {
                    AppNotificationManager.PushMessage(new AppNotification() { Message = "Account created Successfully" });
                    Close();
                }
                else
                {
                    errorTextBlock.Text = ack.Reason;
                }

            }
            else
                errorTextBlock.Text = "Connection Error";
            progressRing.isActive = false;
            
        }

        private bool IsValid()
        {
            if(String.IsNullOrWhiteSpace(fullNameTextBlock.Text))
            {
                errorTextBlock.Text = "Enter Name";
                return false;
            }
            else if(string.IsNullOrWhiteSpace(usernameTextBlock.Text) || usernameTextBlock.Text.Contains(" "))
            {
                errorTextBlock.Text = "Username can't contain spaces";
                return false;
            }
            else if(string.IsNullOrWhiteSpace(passwordTextBlock.Password))
            {
                errorTextBlock.Text = "Enter Password";
                return false;
            }
            else if(string.IsNullOrWhiteSpace(confirmPasswordTextBlock.Password))
            {
                errorTextBlock.Text = "Reenter Password";
                return false;
            }
            else if (passwordTextBlock.Password != confirmPasswordTextBlock.Password)
            {
                return false;
            }
            return true;
        }

        private void confirmPasswordTextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBlock.Password != confirmPasswordTextBlock.Password)
                errorTextBlock.Text = "Both Passwords don't match";
            else
                errorTextBlock.Text = "";
        }
    }
}
