using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Tutor.Model.Data;
using Tutor.Model.Manager;
using Tutor.UserControl;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for AddVideo.xaml
    /// </summary>
    public partial class AddVideo : Window
    {
        //Set for new upload video
        public CourseDetails CourseID { get; set; }

        private TimeSpan duration  = TimeSpan.FromSeconds(0);
        private int videoHeight = 0;
        private int videoWidth = 0;
        private string thumbnailPath = "";
        private bool thumbnailChosen = false;

        //set for edit video type window
        public bool IsEditVideoType { get; set; } = false;
        public Video VideoDetails { get; set; }

        public AddVideo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorTextBlock.Text = "";
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
            { }
        }

        private void chooseThumbnailButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Image File (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                CheckFileExists = true
            };
            bool result = (bool)fileDialog.ShowDialog();
            if (result)
            {
                thumbnailChosen = true;
                thumbnailPath = fileDialog.FileName;
                thumbnailContainer.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void chooseVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Video File (*.mp4)|*.mp4",
                CheckFileExists = true
            };
            bool result = (bool)fileDialog.ShowDialog();
            if (!result)
                return;
            MediaElement media = new MediaElement() {Name="hiddenMedia", LoadedBehavior = MediaState.Manual, Height=0,Width=0, Volume=0} ;
            media.Loaded += Media_Loaded;
            panel.Children.Add(media);
            if(result)
            {
                chosenVideoPathTextBlock.Text = fileDialog.FileName;
                media.Source = new Uri(fileDialog.FileName);
                media.Play();
                
            }
            
        }

        private void Media_Loaded(object sender, RoutedEventArgs e)
        {
            var media = ((MediaElement)sender);
            while (!media.IsLoaded || !media.NaturalDuration.HasTimeSpan)
                Thread.Sleep(200);
            duration = media.NaturalDuration.TimeSpan;
            videoHeight = media.NaturalVideoHeight;
            videoWidth = media.NaturalVideoWidth;
            media.Close();
        }

        private async void uploadVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid() && !IsEditVideoType)
            {
                progressRing.isActive = true;
                CreateVideoRequest request = new CreateVideoRequest()
                {
                    AuthorId = SettingsManager.Username,
                    AuthorName = SettingsManager.FullName,
                    CourseId = CourseID.CourseID,
                    CourseName = CourseID.CourseName,
                    Decription = descriptionTextBlock.Text,
                    Duration = duration,
                    Height = videoHeight,
                    Width = videoWidth,
                    Title = titleTextBlock.Text,
                    Tags = new List<string>()
                };

                foreach (Tags tag in tagsContainer.Children)
                {
                    request.Tags.Add(tag.TagName);
                }
                Object resp = await ConnectionManager.SendRequestAsync(request, RequestType.CreateVideo, ResponseType.CreateVideo);
                if (resp != null)
                {
                    CreateVideoResponse response = ((JObject)resp).ToObject<CreateVideoResponse>();
                    string path = Guid.NewGuid().ToString() + ".jpg";
                    File.Copy(thumbnailPath, path);
                    
                    Object response2 = await ConnectionManager.SendVideoFileAsync(chosenVideoPathTextBlock.Text, path, response.Port);
                    if (File.Exists(path))
                        File.Delete(path);
                    if (response2 != null)
                        AppNotificationManager.PushMessage(new AppNotification() { Message = ((JObject)response2).ToObject<Acknowledge>().Reason });
                    else
                        AppNotificationManager.PushMessage(new AppNotification() { Message = "Video Upload Failed Failed." });
                    Close();
                }
                else
                    errorTextBlock.Text = "Connection Error";
                progressRing.isActive = false;
            }
            else if (IsValid() && IsEditVideoType)
            {
                progressRing.isActive = true;
                VideoUpdateRequest request = new VideoUpdateRequest()
                {
                    Title = titleTextBlock.Text,
                    Description = descriptionTextBlock.Text,
                    IsThumbnailUpdateRequired = thumbnailChosen,
                    Tags = new List<string>()
                };
                foreach (Tags tags in ((WrapPanel)tagsContainer).Children)
                {
                    request.Tags.Add(tags.TagName);
                }
                Object resp = await ConnectionManager.SendRequestAsync(request, RequestType.VideoUpdate, ResponseType.Acknowledge);
                if (resp != null)
                {
                    VideoUpdateResponse ack = ((JObject)resp).ToObject<VideoUpdateResponse>();
                    if(ack.Status=="OK")
                    {
                        try
                        {
                            if (thumbnailChosen)
                                await ConnectionManager.SendFileAsync(thumbnailPath, null);
                        }catch(Exception ex)
                        {
                            AppNotificationManager.PushMessage(new AppNotification() { Message = ex.Message });
                            Close();
                        }
                    }
                    errorTextBlock.Text = ack.Reason;

                }
                else
                    errorTextBlock.Text = "Connection Error.";

            }
        }

        private bool IsValid()
        {
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //titleTextBlock.Text = CourseID;
            if(IsEditVideoType)
            {
                titleBar.Text = "Edit Video";
                chooseVideoContainer.Visibility = Visibility.Collapsed;
                uploadVideoButton.Content = "Modify";
                titleTextBlock.Text = VideoDetails.Title;
                descriptionTextBlock.Text = VideoDetails.Description;
                tagHeadingTextBlock.Text = "Tags (If added, will overwrite existing tags)";
            }
        }

        private void addTagButton_Click(object sender, RoutedEventArgs e)
        {
            addTag();
        }

        private void addTag()
        {
            if (String.IsNullOrWhiteSpace(addTagsBlock.Text) || tagsContainer.Children.Count >= 5)
                return;
            tagsContainer.Children.Add(new Tags(tagsContainer) { TagName = addTagsBlock.Text, Margin = new Thickness(5, 0, 5, 0) });
            addTagsBlock.Text = "";
        }
        private void addTagsBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                addTag();
        }
    }
}
