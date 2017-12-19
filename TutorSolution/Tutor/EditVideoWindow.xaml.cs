using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;
using Tutor.UserControl;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for EditVideoWindow.xaml
    /// </summary>
    public partial class EditVideoWindow : Window
    {
        private bool thumbnailChosen = false;
        private string thumbnailPath = "";

        public string VideoID { get; set; }
        public Video VideoData { get; set; }


        public EditVideoWindow()
        {
            InitializeComponent();
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


        private async Task Sendrequest()
        {
            if (!string.IsNullOrWhiteSpace(VideoID))
            {

                Object resp = new JObject();
                resp = await ConnectionManager.SendRequestAsync(new VideoDetails() { VideoID = VideoID }, RequestType.VideoDetails, ResponseType.VideoDetails);
                if (resp != null)
                {
                    VideoData = ((JObject)resp).ToObject<Video>();
                    LoadData();
                    overlayWindow.Visibility = Visibility.Collapsed;
                }
                else
                {
                    loadingTextBlock.Text = "Load Failed.";
                    loadingTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void LoadData()
        {
            titleTextBlock.Text = VideoData.Title;
            descriptionTextBlock.Text = VideoData.Description;
            thumbnailContainer.Source = new BitmapImage(new Uri("http://" + SettingsManager.ServerIp + "/root" + VideoData.Thumbnail));
            foreach (var item in VideoData.Tags)
            {
                tagsContainer.Children.Add(new Tags(tagsContainer) { TagName = item });
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            progressRing.isActive = true;
            await Sendrequest();
            progressRing.isActive = false;

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
                thumbnailContainer.Source = new BitmapImage(new Uri(thumbnailPath));
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

        private async void modifyVideoButton_Click(object sender, RoutedEventArgs e)
        {
            progressRing.isActive = true;
            VideoUpdateRequest request = new VideoUpdateRequest()
            {
                VideoID = VideoID,
                Title = titleTextBlock.Text,
                Description = descriptionTextBlock.Text,
                IsThumbnailUpdateRequired = thumbnailChosen,
                Tags = new List<string>()
            };
            foreach (Tags tags in ((WrapPanel)tagsContainer).Children)
            {
                request.Tags.Add(tags.TagName);
            }
            Object resp = await ConnectionManager.SendRequestAsync(request, RequestType.VideoUpdate, ResponseType.VideoUpdate);
            if (resp != null)
            {
                VideoUpdateResponse ack = ((JObject)resp).ToObject<VideoUpdateResponse>();
                if (ack.Status == "OK")
                {
                    if (thumbnailChosen)
                    {
                        string tempPath = "Temporary/" + Guid.NewGuid() + ".jpg";
                        if (Directory.Exists("Temporary/"))
                            Directory.CreateDirectory("Temporary/");
                            File.Copy(thumbnailPath, tempPath);
                        var result = await ConnectionManager.SendFileAsync(tempPath, null);
                        if(result!=null)
                        {
                            var response = ((JObject)result).ToObject<Acknowledge>();
                            errorTextBlock.Text = response.Reason;
                            if(response.Status=="OK")
                            {
                                AppNotificationManager.PushMessage(new AppNotification() { Message = response.Reason });
                                Close();
                            }
                        }
                        if (File.Exists(tempPath))
                            File.Delete(tempPath);
                    }
                    AppNotificationManager.PushMessage(new AppNotification() { Message = ack.Reason });
                    Close();
                }
                errorTextBlock.Text = ack.Reason;

            }
            else
                errorTextBlock.Text = "Connection Error.";

        }
    }
}
