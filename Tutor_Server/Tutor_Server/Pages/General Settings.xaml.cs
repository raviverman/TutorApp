using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for General_Settings.xaml
    /// </summary>
    public partial class General_Settings : Page
    {
        public General_Settings()
        {
            InitializeComponent();
        }

        private void browseFile_Click(object sender, RoutedEventArgs e)
        {
            var ClickedButton = ((Button)sender);
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Batch File(*.bat) |*.bat",
                CheckFileExists = true
            };

            var result = (bool)fileDialog.ShowDialog();
            if(result)
            {
                switch((string)ClickedButton.Tag)
                {
                    case "httpStart":
                        httpStartPathTextBlock.Text = fileDialog.FileName;
                        Settings.HttpStartPath = fileDialog.FileName;
                        break;
                    case "httpStop":
                        httpStopPathTextBlock.Text = fileDialog.FileName;
                        Settings.HttpStopPath = fileDialog.FileName;
                        break;
                    case "sqlStart":
                        sqlStartPathTextBlock.Text = fileDialog.FileName;
                        Settings.SqlStartPath = fileDialog.FileName;
                        break;
                    case "sqlStop":
                        sqlStopPathTextBlock.Text = fileDialog.FileName;
                        Settings.SqlStopPath = fileDialog.FileName;
                        break;
                }
            }
        }

        private void serverRootButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Pick Folder";
            bool result = (bool)dialog.ShowDialog();
            if(result)
            {
                serverRootTextBlock.Text = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                Settings.ServerRoot = serverRootTextBlock.Text;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            httpStartPathTextBlock.Text = SettingsManager.HttpStartPath;
            httpStopPathTextBlock.Text = SettingsManager.HttpStopPath;
            sqlStartPathTextBlock.Text = SettingsManager.SqlStartPath;
            sqlStopPathTextBlock.Text = SettingsManager.SqlStopPath;
            serverRootTextBlock.Text = SettingsManager.ServerRoot;
            serverPortTextBlock.Text = SettingsManager.ServerPort.ToString();
        }

        private void InitializeServerRoot_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(serverRootTextBlock.Text + "/Videos"))
                Directory.CreateDirectory(serverRootTextBlock.Text + "/Videos");
            if (!Directory.Exists(serverRootTextBlock.Text + "/Defaults"))
                Directory.CreateDirectory(serverRootTextBlock.Text + "/Defaults");
            if (!Directory.Exists(serverRootTextBlock.Text + "/Temp"))
                Directory.CreateDirectory(serverRootTextBlock.Text + "/Temp");
            if (!Directory.Exists(serverRootTextBlock.Text + "/User"))
                Directory.CreateDirectory(serverRootTextBlock.Text + "/User");
            if (!Directory.Exists(serverRootTextBlock.Text + "/Thumbnails"))
                Directory.CreateDirectory(serverRootTextBlock.Text + "/Thumbnails");
            StatusManager.PushMessage("Directory Initialized.", StatusType.Success);
        }

        private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(serverPortTextBlock.Text, out int value) && value != 0)
            {
                Settings.ServerPort = value;
            }
            else
                serverPortTextBlock.Text = "0";
        }
    }
}
