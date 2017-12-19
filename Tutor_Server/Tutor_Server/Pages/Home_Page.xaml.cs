using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using Tutor_Server.Model;
using Tutor_Server.Model.Data;
using Tutor_Server.Model.Manager;

namespace Tutor_Server.Pages
{
    /// <summary>
    /// Interaction logic for Home_Page.xaml
    /// </summary>
    public partial class Home_Page : Page
    {
        public Home_Page()
        {
            InitializeComponent();
            clientsConnectedTextBlock.DataContext = this;
        }

        private Stopwatch _uptime = new Stopwatch();


        public Stopwatch Uptime
        {
            get { return _uptime; }
            set {
                _uptime = value;
            }
        }

        public static int ConnectedClient { get; set; }

        //Starts updating status List as soon as page is loaded.
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(FetchStatusMessages));
            t.IsBackground = true;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        /// <summary>
        /// Private Method running on independent thread to fetch message.
        /// </summary>
        private void FetchStatusMessages()
        {
            while(true)
            {
                if(StatusManager.Count>0)
                {
                    ObservableCollection<Status> messages = new ObservableCollection<Status>();
                    StatusManager.PullMessages(messages);
                    this.Dispatcher.Invoke(() =>
                        {
                            foreach (var message in messages)
                            {
                                statusListView.Items.Add(new User_Control.ListItem() { Text = message.StatusMessage,
                                                                                       HintColor=new SolidColorBrush(StatusManager.GetColor(message.Type)),
                                                                                       PaddingText = new Thickness(10),
                                                                                       FontSize = 14
                                                                                        });
                                statusListView.ScrollIntoView(statusListView.Items.GetItemAt(statusListView.Items.Count - 1));
                            }
                        });
                    
                }
                Thread.Sleep(200);
                try
                {
                    Dispatcher.Invoke(
                   () =>
                   {
                       clientsConnectedTextBlock.Text = ConnectedClient.ToString();
                       uptimeTextBlock.Text = Uptime.Elapsed.ToString().Substring(0, 8);
                   });
                }
                catch { }

            }
        }

       
        private void databaseButton_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (databaseButton.Value == 0)
            {
                databaseSignal.Color = Colors.OrangeRed;
                StatusManager.PushMessage("Stopping MySQL Server...");
                StartService(SettingsManager.SqlStopPath);
                StatusManager.PushMessage("MySQL Server Stopped.", StatusType.Error);
                //Add  command to stop server.
                return;
            }
            databaseSignal.Color = Colors.LightGreen;
            StatusManager.PushMessage("Starting MySQL Server...");
            var status = StartService(SettingsManager.SqlStartPath, "");
            if (status)
                StatusManager.PushMessage("MySQL Server running.", StatusType.Success);
            else
                StatusManager.PushMessage("MySQL Server Couldn't be started.", StatusType.Error);
            //Add Command to start server.

        }

        private void serverButton_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (serverButton.Value == 0)
            {
                serverSignal.Color = Colors.OrangeRed;
                StatusManager.PushMessage("Stopping Apache Server...");
                StartService(SettingsManager.HttpStopPath);
                StatusManager.PushMessage("Apache Server Stopped.", StatusType.Error);
                //Add  command to stop server.--done
                return;
            }
            serverSignal.Color = Colors.LightGreen;
            StatusManager.PushMessage("Starting Apache Server...");
            var status = StartService(SettingsManager.HttpStartPath, "");
            if (status)
            {
                StatusManager.PushMessage("Apache Server running.", StatusType.Success);
            }
            else
                StatusManager.PushMessage("Server Couldn't be started.", StatusType.Error);
            //Add Command to start server. --done
        }
        
        //To start mysql and http server - System.StartProcess
        private bool StartService(String serviceName, String args="")
        {
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = serviceName;
            process.StartInfo.Arguments = args;
            bool result = false;
            try
            {
                result = process.Start();
            }
            catch(Exception e)
            {
                StatusManager.PushMessage(e.Message, StatusType.Error);
            }


            return result;
        }

        private void mainServerButton_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mainServerButton.Value == 0)
            {
                mainServerSignal.Color = Colors.OrangeRed;
                StatusManager.PushMessage("Stopping Tutor Server Server...");
                MainServer.Stop();
                Uptime.Reset();
                //Add  command to stop server.
                return;
            }
            mainServerSignal.Color = Colors.LightGreen;
            //Add Command to start server
            Uptime.Start();
            StatusManager.PushMessage("Starting Tutor Server Server...");
            MainServer.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }
    }
}
