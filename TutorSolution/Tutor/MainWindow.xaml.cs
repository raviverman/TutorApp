using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;
using Tutor.Pages;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new DefaultPage());
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ConnectionManager.Connect();
            if(!SettingsManager.IsLoggedIn || !SettingsManager.KeepLoggedIn)
            {
                SettingsManager.LogoutUpdate();
                RequestLogin();
            }
            else
            {
                SetUpAccount(SettingsManager.AccountType == "Student" ? UserType.Student : UserType.Professional);
            }
           
            Thread thread = new Thread(new ThreadStart(NotificationRunner))
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ShowNotification(string message)
        {
            notificationTextBlock.Text = message;
            notificationPanel.Visibility = Visibility.Visible;

        }

        private void NotificationRunner()
        {
            while (true)
            {

                if (AppNotificationManager.Count > 0)
                {
                    AppNotification notification = AppNotificationManager.PullMessage();
                    Dispatcher.Invoke
                        (() =>
                        {
                            ShowNotification(notification.Message);
                        });
                    Thread.Sleep(5000);
                    Dispatcher.Invoke(() => { notificationPanel.Visibility = Visibility.Collapsed; });
                }
                else
                    Thread.Sleep(200);

            }
        }

        private void RequestLogin()
        {
            SignIn signInWindow = new SignIn() { CallerWindow = this};
            signInWindow.ShowDialog();
            
        }

        public void SetUpAccount(UserType type)
        {
            switch (type)
            {
                case UserType.Student:
                    createListViewItem.Visibility = Visibility.Collapsed;
                    myVideosListViewItem.Visibility = Visibility.Collapsed;
                    break;
                case UserType.Professional:
                    createListViewItem.Visibility = Visibility.Visible;
                    myVideosListViewItem.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private async void mainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainListView.SelectedIndex == -1)
                return;
            if (SettingsManager.IsLoggedIn == false && 
                ((ListViewItem)mainListView.SelectedItem).Tag.ToString() !="settings" &&
                ((ListViewItem)mainListView.SelectedItem).Tag.ToString() != "reconnect")
            {
                RequestLogin();
                mainListView.SelectedIndex = -1;
                return;
            }
            switch (((ListViewItem)mainListView.SelectedItem).Tag)
            {
                case "home":
                    mainProgressRing.isActive = true;
                    Object resp = new JObject();
                    resp = await ConnectionManager.SendRequestAsync(new HomeRequest() { Latest = "" }, RequestType.Home, ResponseType.Home);
                    if (resp != null)
                    {
                        HomePage page1 = new HomePage((JObject)resp, ResponseType.Home);
                        mainFrame.Navigate(page1);
                    }
                    else
                        AppNotificationManager.PushMessage(new AppNotification() {Message="Unable to load Home." });
                    mainProgressRing.isActive = false;
                    break;
                case "myvideos":
                    mainProgressRing.isActive = true;
                    Object response = new JObject();
                    resp = await ConnectionManager.SendRequestAsync(new CourseListRequest() { AuthorID=SettingsManager.Username }, RequestType.CourseList,ResponseType.CourseList );
                    if (resp != null)
                    {
                        HomePage page = new HomePage((JObject)resp, ResponseType.CourseList, "Courses") { Parent = mainFrame} ;
                        mainFrame.Navigate(page);
                    }
                    else
                        AppNotificationManager.PushMessage(new AppNotification() { Message = "Unable to load Courses." });

                    mainProgressRing.isActive = false;
                    break;
                case "favorites":
                    mainProgressRing.isActive = true;
                    Object response2 = new JObject();
                    response2 = await ConnectionManager.SendRequestAsync(new FavoritesRequest() { Userid = SettingsManager.Username}, RequestType.Favorites, ResponseType.Favorites);
                    if (response2 != null)
                    {
                        HomePage page1 = new HomePage((JObject)response2, ResponseType.Favorites, "Favorites");
                        mainFrame.Navigate(page1);
                    }
                    else
                        AppNotificationManager.PushMessage(new AppNotification() {Message="Unable to load Favorites." });

                    mainProgressRing.isActive = false;
                    break;
                case "create":
                    CreateCourse course = new CreateCourse();
                    course.ShowDialog();
                    break;
                case "logout":
                    SettingsManager.LogoutUpdate();
                    SettingsManager.SaveSettings();
                    while(mainFrame.CanGoBack)
                        mainFrame.RemoveBackEntry();
                    mainFrame.Navigate(new DefaultPage());
                    RequestLogin();
                    break;
                case "reconnect":
                    mainProgressRing.isActive = true;
                    await ConnectionManager.Connect();
                    mainProgressRing.isActive = false;
                    SettingsManager.ProfilePath = SettingsManager.ProfilePath;
                    break;
                case "settings":
                    Settings set = new Settings();
                    set.Show();
                    break;
                case "about":
                    new About().ShowDialog();
                    break;
                default:
                    break;
            }
            mainListView.SelectedIndex = -1;

        }

        private void editProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsManager.IsLoggedIn)
                return;
            ProfileSettings profileSet = new ProfileSettings();
            profileSet.ShowDialog();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(SearchTextBox.Text))
                placeholderText.Visibility = Visibility.Visible;

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholderText.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SearchTextBox.Text) || !SettingsManager.IsLoggedIn)
                return;
            mainProgressRing.isActive = true;
            Search searchRequest = new Search()
            {
                SearchBy = ((SearchType)(searchTypeListBox.SelectedIndex))
            };

            searchRequest.SearchText = SearchTextBox.Text.Trim();
            Object response = await ConnectionManager.SendRequestAsync(searchRequest, RequestType.Search, ResponseType.Search);
            if (response != null)
            {
                HomePage home = new HomePage(response, ResponseType.Search, "Search Results : " + SearchTextBox.Text) { Parent = mainFrame};
                mainFrame.Navigate(home);
            }
            else
                AppNotificationManager.PushMessage(new AppNotification() { Message="Failed to perform search." });
            mainProgressRing.isActive = false;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
             Button_Click(searchButton, null);
        }

    }
}
