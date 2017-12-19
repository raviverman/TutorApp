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
using Tutor.Model.Manager;

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for VideoTemplate.xaml
    /// </summary>
    public partial class VideoTemplate : System.Windows.Controls.UserControl
    {
        private string _thumbnail = @"/Assets/T_grayed.png";
        private string _title = "Generic Video Title";
        private string _duration = "45:51";
        public string VideoID { get; set; }
        public string Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                UpdateValue();
            }
        }
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                UpdateValue();
            }
        }
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                UpdateValue();
            }
        }
        public VideoTemplate()
        {
            InitializeComponent();
        }

        private void UpdateValue()
        {
            titleTextBlock.Text = _title;
            durationTextBlock.Text = _duration;
            thumbnailImage.Source = new BitmapImage(new Uri("http://" + SettingsManager.ServerIp + "/root" + _thumbnail));
            mainContainer.ToolTip = _title;
        }

        private void mainContainer_Click(object sender, RoutedEventArgs e)
        {
            VideoWindow videoView = new VideoWindow() { VideoID = VideoID };
            videoView.Show();
        }
    }
}
