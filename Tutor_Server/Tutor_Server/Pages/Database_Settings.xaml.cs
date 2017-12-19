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
using Tutor_Server.Model.Manager;

namespace Tutor_Server.Pages
{
    /// <summary>
    /// Interaction logic for Database_Settings.xaml
    /// </summary>
    public partial class Database_Settings : Page
    {
        public Database_Settings()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            databaseNameTextBlock.Text = SettingsManager.DatabaseName;
            databasePortTextBlock.Text = SettingsManager.DatabasePort.ToString();
            usernameTextBlock.Text = SettingsManager.DatabaseUsername;
            passwordTextBlock.Password = SettingsManager.DatabasePassword;
            specificIPcheckBox.IsChecked = !SettingsManager.AnyIpUsed;
            specificIPTextBlock.Text = SettingsManager.SpecificServerIP;
        }


        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBlock = ((Control)sender);
            switch((string)textBlock.Tag)
            {
                case "username":
                    Settings.DatabaseUsername = usernameTextBlock.Text;
                    break;
                case "password":
                    Settings.DatabasePassword = passwordTextBlock.Password;
                    break;
                case "databasename":
                    Settings.DatabaseName = databaseNameTextBlock.Text;
                    break;
                case "databaseport":
                    int value = 0;
                    int.TryParse(databasePortTextBlock.Text, out value);
                    if (value != 0)
                        Settings.DatabasePort = value;
                    else
                        databasePortTextBlock.Text = "0";
                    break;
                case "specificip":
                     if((bool)specificIPcheckBox.IsChecked)
                        {
                            Settings.AnyIpUsed = false;
                            Settings.SpecificServerIP = specificIPTextBlock.Text;
                        }
                     else
                             Settings.AnyIpUsed = true;
                    break;

            }
        }


        private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager manager = new DatabaseManager($"datasource=localhost;port={databasePortTextBlock.Text};username={usernameTextBlock.Text};password={passwordTextBlock.Password};database={databaseNameTextBlock.Text}");
            manager.SetUpDatabase();
        }
    }
}
