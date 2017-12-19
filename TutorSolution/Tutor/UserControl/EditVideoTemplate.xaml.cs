using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for EditVideoTemplate.xaml
    /// </summary>
    public partial class EditVideoTemplate : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private string _videoTitle = "Video Title";

        public string VideoTitle
        {
            get { return _videoTitle; }
            set { _videoTitle = value; NotifyPropertyChanged(); }
        }

        private string _thumbnail;

        public string Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = $"http://{SettingsManager.ServerIp}/root{value}"; NotifyPropertyChanged(); }
        }

        private string _videoID;

        public string VideoID
        {
            get { return _videoID; }
            set { _videoID = value; NotifyPropertyChanged(); }
        }

        private string _courseID;

        public string CourseID
        {
            get { return _courseID; }
            set { _courseID = value; }
        }

        private string _duration;

        public string Duration
        {
            get { return _duration; }
            set { _duration = value; NotifyPropertyChanged(); }
        }

        private bool _isFavoritesType = false;

        public bool IsFavoritesType
        {
            get { return _isFavoritesType; }
            set { _isFavoritesType = value; }
        }


        public EditVideoTemplate()
        {
            InitializeComponent();
            DataContext = this;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void deleteVideoButton_Click(object sender, RoutedEventArgs e)
        {
            //enclose inside success.
            ConfirmWindow confirm = new ConfirmWindow(VideoTitle);
            confirm.ShowDialog();
            if(confirm.Result && !IsFavoritesType)
            {
                DeleteVideoRequest request = new DeleteVideoRequest() { VideoID = VideoID, CourseID = CourseID };
                Object result = await ConnectionManager.SendRequestAsync(request, RequestType.DeleteVideo, ResponseType.Acknowledge);
                if(result!=null)
                {
                    if(((JObject)result).ToObject<Acknowledge>().Status=="OK")
                    {
                        ((WrapPanel)Parent).Children.Remove(this);
                        AppNotificationManager.PushMessage(new AppNotification() { Message = "Video Deleted Successfully." });
                    }
                }
                else
                    AppNotificationManager.PushMessage(new AppNotification() { Message = "Unable to delete video." });
            }
            else if(confirm.Result && IsFavoritesType)
            {
                DeleteFavorite request = new DeleteFavorite() { UserID = SettingsManager.Username, VideoID = VideoID };
                Object result = await ConnectionManager.SendRequestAsync(request, RequestType.DeleteFavorite, ResponseType.Acknowledge);
                if (result != null)
                {
                    if (((JObject)result).ToObject<Acknowledge>().Status == "OK")
                    {
                        ((WrapPanel)Parent).Children.Remove(this);
                        AppNotificationManager.PushMessage(new AppNotification() { Message = "Favorite Deleted Successfully." });
                    }
                }
                else
                    AppNotificationManager.PushMessage(new AppNotification() { Message = "Unable to delete favorite." });
            }

        }

        private void addVideoButton_Click(object sender, RoutedEventArgs e)
        {
            EditVideoWindow window = new EditVideoWindow() { VideoID = VideoID };
            window.Show();
        }

        private void mainContainer_Click(object sender, RoutedEventArgs e)
        {
            VideoWindow videoView = new VideoWindow() { VideoID = VideoID };
            videoView.Show();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(IsFavoritesType)
            {
                addVideoButton.Visibility = Visibility.Collapsed;
                deleteVideoButton.ToolTip = "Remove Favorite";
            }
        }
    }
}
