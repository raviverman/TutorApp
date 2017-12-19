using Newtonsoft.Json.Linq;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for CommentView.xaml
    /// </summary>
    public partial class CommentView : System.Windows.Controls.UserControl
    {
        //public ListView Parent { get; set; }
        public Comment Comment{ get; set; }
        public bool IsReadOnlyType { get; set; } = true;
        public PostComment commentDetails{ get; set; }
        public string VideoOwner { get; set; }
        private string[] Month =
        {
            "",
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sept",
            "Oct",
            "Nov",
            "Dec"
        };

        public CommentView()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsReadOnlyType)
            {
                readOnlyComment.Visibility = Visibility.Collapsed;
                writeComment.Visibility = Visibility.Visible;
            }
            if (IsReadOnlyType)
            {
                profileImage.Source = new BitmapImage(new Uri("http://" + SettingsManager.ServerIp + "/root" + Comment.Thumbnail));
                usernameTextBlock.Text = Comment.Username;
                dateTextBlock.Text = String.Format("{0} - {1} {2}, {3}", Comment.Date.ToShortTimeString(), Comment.Date.Day, Month[Comment.Date.Month], Comment.Date.Year);
                commentTextBlock.Text = Comment.Content;
                likeTextBlock.Text = Comment.Likes.ToString();
                dislikeTextBlock.Text = Comment.Dislikes.ToString();
                
                if (SettingsManager.Username == Comment.UserId || SettingsManager.Username == VideoOwner)
                    deleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                profileImage1.Source = new BitmapImage(new Uri(SettingsManager.ProfilePath, UriKind.RelativeOrAbsolute));
                usernameTextBlock1.Text = SettingsManager.FullName;

            }
        }

        private async void postCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(commentTextBlock1.Text) || Tag ==null)
                return;
            PostComment comment = new PostComment()
            {
                UserID = SettingsManager.Username,
                Username = SettingsManager.FullName,
                Content = commentTextBlock1.Text,
                Thumbnail = SettingsManager.ProfilePath.Replace("http://" + SettingsManager.ServerIp + "/root", ""),
                VideoID = Tag.ToString()
            };

            postCommentProgess.isActive = true;
            postCommentButton.IsEnabled = false;
            Object resp = new JObject();
            resp = await ConnectionManager.SendRequestAsync(comment,RequestType.PostComment, ResponseType.Acknowledge);
            if(resp!=null)
            {
                Acknowledge ack = ((JObject)resp).ToObject<Acknowledge>();
                if(ack.Status=="OK")
                {
                    postCommentButton.Content = "Posted";
                    
                    ((ListView)Parent).Items.Insert(1,new CommentView() {
                        Comment = new Comment()
                        {
                            Content = commentTextBlock1.Text,
                            Date = DateTime.Now,
                            Dislikes = 0,
                            Likes = 0,
                            Thumbnail = SettingsManager.ProfilePath.Replace("http://" + SettingsManager.ServerIp + "/root", ""),
                            Uid = SettingsManager.Username,
                            Username = SettingsManager.FullName,
                            UserId = SettingsManager.Username,
                            VideoID = Tag.ToString()
                        }
                        , IsReadOnlyType = true
                        , VideoOwner = ""
                        , Tag =ack.Reason
                      });
                    commentTextBlock1.Text = "";

                }
                else
                    postCommentButton.Content = "Failed";
            }
            else
            {
                postCommentButton.IsEnabled = true;
                postCommentButton.Content = "Repost";
            }
            postCommentProgess.isActive = false;
             
        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteComment comment = new DeleteComment()
            {
                VideoID = Comment.VideoID,
                CommentID = (string)Tag,
                UserID = Comment.UserId
            };

            deleteCommentProgressRing.isActive = true;
            Object resp = new JObject();
            resp = await ConnectionManager.SendRequestAsync(comment, RequestType.DeleteComment, ResponseType.Acknowledge);
            if(resp!=null)
            {
                Acknowledge ack = ((JObject)resp).ToObject<Acknowledge>();
                if(ack.Status=="OK")
                {
                    Visibility = Visibility.Collapsed;
                }
            }
            deleteCommentProgressRing.isActive = false;
        }

        private async void LikeDislikeButton_Click(object sender, RoutedEventArgs e)
        {
            int likeDislike = int.Parse(((Button)sender).Tag.ToString());
            LikeComment request = new LikeComment() { LikeDislike = likeDislike, UserID = SettingsManager.Username,  CommentID= Comment.Uid};
            Object response = await ConnectionManager.SendRequestAsync(request, RequestType.LikeComment, ResponseType.Acknowledge);
            if (response != null)
            {
                if (((JObject)response).ToObject<Acknowledge>().Status == "OK")
                {
                    if (likeDislike > 0)
                        likeTextBlock.Text = (int.Parse(likeTextBlock.Text) + 1).ToString();
                    else
                        dislikeTextBlock.Text = (int.Parse(dislikeTextBlock.Text) + 1).ToString();
                }
            }
        }

        private void commentTextBlock1_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholderBlock.Visibility = Visibility.Collapsed;
        }

        private void commentTextBlock1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(commentTextBlock1.Text))
                placeholderBlock.Visibility = Visibility.Visible;
        }
    }
}
