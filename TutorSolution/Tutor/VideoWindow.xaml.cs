using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Tutor.Model.Data;
using Tutor.Model.Manager;
using Tutor.UserControl;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        public string VideoID { get; set; }
        public Video video { get; set; }
        private static double totalDurationSeconds = 1000000;
        private bool isMediaPlaying = false;
        DispatcherTimer timer = new DispatcherTimer();
        private bool isFullScreen = false;
        private double volume = 0.5;
        private bool isMute = false;

        public VideoWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(2000);
            timer.Tick += new EventHandler(UpdateSliderValue);
        }

        private void UpdateSliderValue(object sender, EventArgs e)
        {
            if (mainMediaElement.IsBuffering)
                mediaBufferProgress.isActive = true;
            else
                mediaBufferProgress.isActive = false;
            double totalSeconds = mainMediaElement.Position.TotalSeconds;
            if (!seekSlider.IsMouseOver)
                seekSlider.Value = totalSeconds;
            currentPositionTextBlock.Text = mainMediaElement.Position.ToString().Substring(0, 8);
            seekSlider.SelectionEnd = totalSeconds;
            if(totalSeconds == totalDurationSeconds)
            {
                playButton.Content = "\uE149";
                controlsContainer.Opacity = 1;
                isMediaPlaying = false;
            }
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            overlayWindow.Visibility = Visibility.Visible;
            mainProgressRing.isActive = true;
            await Sendrequest();
            mainProgressRing.isActive = false;
            mainMediaElement.Volume = volume;

        }

        private async Task Sendrequest()
        {
            if(!string.IsNullOrWhiteSpace(VideoID))
            {
                
                Object resp = new JObject();
                resp = await ConnectionManager.SendRequestAsync(new VideoDetails() { VideoID = VideoID, UserID = SettingsManager.Username}, RequestType.VideoDetails, ResponseType.VideoDetails);
                if (resp != null)
                {
                    video = ((JObject)resp).ToObject<Video>();
                    LoadVideo();
                    overlayWindow.Visibility = Visibility.Collapsed;

                }
                else
                {
                    loadingTextBlock.Text = "Load Failed.";
                    loadingTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void LoadVideo()
        {
            videoTitleTextBlock.Text = video.Title;
            mainMediaElement.Source = new Uri("http://"+SettingsManager.ServerIp+"/root" + video.Path);
            authorTextBlock.Text = video.AuthorName;
            courseTextBlock.Text = "@" + video.Course;
            likeTextBlock.Text = video.Likes.ToString();
            dislikeTextBlock.Text = video.Dislikes.ToString();
            
            if (video.Likes + video.Dislikes != 0)
                likeDislikeBar.Value = (video.Likes * 100) / (video.Likes + video.Dislikes);
            else
                likeDislikeBar.Value = 50;
            profileImage.Source = new BitmapImage(new Uri("http://" + SettingsManager.ServerIp + "/root" + video.AuthorImage));
            descriptionTextBlock.Text = video.Description;

            postCommentComment.Tag = video.VideoID; //used in posting comment
            foreach(var item in video.CommentList)
            {
                commentsListView.Items.Add(new CommentView() { Comment = item, IsReadOnlyType = true, VideoOwner=video.AuthorId , Tag = item.Uid});
            }

            foreach (var item in video.Tags)
            {
                tagContainer.Children.Add(new Tags() { TagName = item, isReadOnlyType = true, FontSize=12, Margin = new Thickness(2) });
            }
            //to update rating.
            ratingControl.CourseID = video.CourseID;
            ratingControl.RatingBlock = ratingTextBlock;
            if (video.AuthorId == SettingsManager.Username)
                ratingControl.IsEnabled = false;

            ratingTextBlock.Text = video.CourseRating.ToString("0.00");
            if(video.IsFavorite)
            {
                addToFavoritesButton.IsEnabled = false;
                addFavoriteTextBlock.Text = "\uE00B";
            }
            //mainMediaElement.NaturalDuration;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
                if (isFullScreen)
                    setFullScreenOn();
            }
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                restoreButton.ToolTip = "Maximize";
                restoreButton.Content = "\uE739";
            }
            else
            {
                restoreButton.ToolTip = "Restore";
                restoreButton.Content = "\uE923";
            }
        }


        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if(isMediaPlaying==false)
            {
                if (playButton.Content.Equals("\uE149"))
                    mainMediaElement.Position = new TimeSpan(0);
                mainMediaElement.Play();
                isMediaPlaying = true;
                playButton.Content = "\uE103";
            }
            else
            {
                mainMediaElement.Pause();
                isMediaPlaying = false;
                playButton.Content = "\uE102";
            }
        }
        
        private void setFullScreenOn()
        {
            if(!isFullScreen)
            {
                TitleRow.Height = new GridLength(0);
                //VideoRow.MaxHeight = 10000;
                ContentRow.Height = new GridLength(0);
                fullScreenButton.Content = "\uE73F";
                scrollbar.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                WindowState = WindowState.Maximized;
                mainMediaElement.MaxHeight = Height-80;
                isFullScreen = true;
            }
            else
            {
                TitleRow.Height = new GridLength(1,GridUnitType.Auto);
                //VideoRow.MaxHeight = 450;
                mainMediaElement.MaxHeight = 450;
                WindowState = WindowState.Normal;
                scrollbar.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                ContentRow.Height = new GridLength(1,GridUnitType.Star);
                fullScreenButton.Content = "\uE740";
                isFullScreen = false;
            }
        }

        private void seekSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mainMediaElement.Position = TimeSpan.FromSeconds(seekSlider.Value);
        }

        private void mainMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if(mainMediaElement.NaturalDuration.HasTimeSpan)
            {
                mediaDurationTextBlock.Text = mainMediaElement.NaturalDuration.TimeSpan.ToString().Substring(0, 8);
                seekSlider.Maximum = mainMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                totalDurationSeconds = seekSlider.Maximum;
                timer.Start();
            }
        }

        private void fullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            setFullScreenOn();
        }

        private async void addToFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            addToFavoritesButton.IsEnabled = false;
            AddToFavorites request = new AddToFavorites() { UserID = SettingsManager.Username, VideoID = VideoID };
            Object response = await ConnectionManager.SendRequestAsync(request, RequestType.AddFavorites, ResponseType.Acknowledge);
            if(response!=null)
            {
                if(((JObject)response).ToObject<Acknowledge>().Status=="OK")
                    addFavoriteTextBlock.Text = "\uE00B";
            }
            addToFavoritesButton.IsEnabled = true;
        }

        private async void LikeDislikeButton_Click(object sender, RoutedEventArgs e)
        {
            int likeDislike = int.Parse(((Button)sender).Tag.ToString());
            LikeVideo request = new LikeVideo() { LikeDislike = likeDislike, UserID = SettingsManager.Username, VideoID = video.VideoID };
            Object response = await ConnectionManager.SendRequestAsync(request, RequestType.LikeVideo, ResponseType.Acknowledge);
            if(response!=null)
            {
                if(((JObject)response).ToObject<Acknowledge>().Status=="OK")
                {
                    int likes = int.Parse(likeTextBlock.Text);
                    int dislikes = int.Parse(dislikeTextBlock.Text);
                    if (likeDislike > 0)
                    {
                        likeTextBlock.Text = (likes + 1).ToString();
                        likeDislikeBar.Value = (likes + 1)*100 / (likes + dislikes + 1);
                    }
                    else
                    {
                        dislikeTextBlock.Text = (dislikes + 1).ToString();
                        likeDislikeBar.Value = (likes) *100/ (likes + dislikes + 1);
                    }
                }
            }
        }


        private void mainMediaElement_BufferingStarted(object sender, RoutedEventArgs e)
        {
            mediaBufferProgress.isActive = true;

        }

        private void mainMediaElement_BufferingEnded(object sender, RoutedEventArgs e)
        {
            mediaBufferProgress.isActive = false;

        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            if(isMute)
            {
               // volumeButton.Content = "\uE995";
                mainMediaElement.Volume = volume;
            }
            else
            {
               // volumeButton.Content = "\uE198";
                volume = mainMediaElement.Volume;
                mainMediaElement.Volume = 0;
            }
            isMute = !isMute;
        }

        private void volumeFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            if(volumeFlyout.Visibility == Visibility.Visible)
            {
                volumeFlyout.Visibility = Visibility.Collapsed;
            }
            else
            {
                volumeFlyout.Visibility = Visibility.Visible;

            }
        }

        private void mediaContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            volumeFlyout.Visibility = Visibility.Collapsed;
        }
    }
}
