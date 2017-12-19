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
using Tutor.Model.Manager;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Properties.Settings.Default.SettingsSaving += Default_SettingsSaving;
        }

        private void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionManager.UpdateConnectionSettings();
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

        private bool IsValid()
        {
            if (String.IsNullOrWhiteSpace(serverIpTextBox.Text))
            {
                serverIpTextBox.Text = "localhost";
                return false;
            }
            else if(String.IsNullOrWhiteSpace(serverPortTextBox.Text))
            {
                serverPortTextBox.Text = "0";
                return false;
            }
            else
            {
                int ans = 0;
                var result = int.TryParse(serverPortTextBox.Text, out ans);
                return result;
            }
        }

        private void saveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid())
                return;
            SettingsManager.ServerIp = serverIpTextBox.Text;
            SettingsManager.ServerPort = int.Parse(serverPortTextBox.Text);
            SettingsManager.SaveSettings();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            serverIpTextBox.Text =  SettingsManager.ServerIp;
            serverPortTextBox.Text = SettingsManager.ServerPort.ToString();
        }

        private void serverPortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            sampleTextBlock.Text = String.Format("http://{0}:{1}/", serverIpTextBox.Text, serverPortTextBox.Text);
        }
    }
}
