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
using Tutor_Server.Model.Manager;
using Tutor_Server.Pages;

namespace Tutor_Server
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            settingsFrame.Navigate(new General_Settings());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
            {
            }
        }

        public static string DatabaseUsername = SettingsManager.DatabaseUsername;
        public static string DatabaseName = SettingsManager.DatabaseName;
        public static string DatabasePassword = SettingsManager.DatabasePassword;
        public static string ServerRoot = SettingsManager.ServerRoot;
        public static int DatabasePort = SettingsManager.DatabasePort;
        public static int ServerPort = SettingsManager.ServerPort;
        public static string HttpStartPath = SettingsManager.HttpStartPath;
        public static string HttpStopPath = SettingsManager.HttpStopPath;
        public static string SqlStartPath = SettingsManager.SqlStartPath;
        public static string SqlStopPath = SettingsManager.SqlStopPath;
        public static bool AnyIpUsed = SettingsManager.AnyIpUsed;
        public static string SpecificServerIP = SettingsManager.SpecificServerIP;


        private void NavigateSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ///save settings
            ///
            Button clickedButton = ((Button)sender);
            if (clickedButton.Foreground == new SolidColorBrush(Colors.White)) 
                  return;
            databaseSettingsButton.Background = new SolidColorBrush(Colors.Transparent);
            generalSettingsButton.Background = new SolidColorBrush(Colors.Transparent);
            generalSettingsButton.Foreground = new SolidColorBrush(Colors.Gray);
            databaseSettingsButton.Foreground = new SolidColorBrush(Colors.Gray);
            switch ((String)clickedButton.Tag)
            {
                case "general":
                    if (settingsFrame.CanGoBack)
                        settingsFrame.GoBack();
                    generalSettingsButton.Background = (SolidColorBrush)Application.Current.Resources["AppThemeBrush"];
                    generalSettingsButton.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case "database":
                    if (settingsFrame.CanGoForward)
                        settingsFrame.GoForward();
                    else
                    settingsFrame.Navigate(new Database_Settings());
                    databaseSettingsButton.Background = (SolidColorBrush)Application.Current.Resources["AppThemeBrush"];
                    databaseSettingsButton.Foreground = new SolidColorBrush(Colors.White);
                    break;
                default:
                    break;
            }
        }

        private void saveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.DatabaseUsername = DatabaseUsername;
            SettingsManager.DatabaseName = DatabaseName;
            SettingsManager.DatabasePassword = DatabasePassword;
            SettingsManager.ServerRoot = ServerRoot;
            SettingsManager.DatabasePort = DatabasePort;
            SettingsManager.ServerPort = ServerPort;
            SettingsManager.HttpStartPath = HttpStartPath;
            SettingsManager.HttpStopPath = HttpStopPath;
            SettingsManager.SqlStartPath = SqlStartPath;
            SettingsManager.SqlStopPath = SqlStopPath;
            SettingsManager.AnyIpUsed = AnyIpUsed;
            SettingsManager.SpecificServerIP = SpecificServerIP;
            SettingsManager.SaveSettings();
            Close();
        }
    }
}
