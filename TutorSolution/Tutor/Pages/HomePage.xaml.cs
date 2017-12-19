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
using Tutor.UserControl;

namespace Tutor.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }


        public HomePage(Object resp, ResponseType type, string Heading = "Home")
        {
            this.resp = resp;
            this.Type = type;
            this.Heading = Heading;
            InitializeComponent();

        }
        new public Frame Parent { get; set; } = new Frame();
        public bool CanGoBack { get; set; } = false;
        private string Heading = "";
        public string CourseID { get; set; } = "N/A";
        private List<VideoMini> list = new List<VideoMini>();
        private List<CourseDetails> courseList = new List<CourseDetails>();
        private SearchType _searchType = SearchType.Title;
        private Object resp;
        private ResponseType Type;
        private void On_Navigated()
        {
            
            JObject response = (JObject)resp;
            if (response == null)
                return;
            if(Type == ResponseType.Home)
            {
                HomePageResponse videoList = response.ToObject<HomePageResponse>();
                list = videoList.VideoList;
            }
            else if(Type == ResponseType.CourseList)
            {
                courseList = ((JObject)response).ToObject<CourseListResponse>().CourseList;
            }
            else if(Type == ResponseType.EditCourseVideo)
            {
                EditCourseVideosResponse videoList = response.ToObject<EditCourseVideosResponse>();
                list = videoList.videoList;
            }
            else if(Type == ResponseType.Favorites)
            {
                FavoritesResponse result = response.ToObject<FavoritesResponse>();
                list = result.VideoList;
            }
            else if(Type == ResponseType.Search)
            {
                SearchResponse r = response.ToObject<SearchResponse>();
                _searchType = r.Type;
                if (r.Type == SearchType.Course)
                {
                    if(r.CourseList!=null)
                    courseList = r.CourseList;
                }
                else
                {
                    if (r.VideoList != null)
                        list = r.VideoList;  
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoForward)
                return;
            if (CanGoBack)
                GoBackButton.Visibility = Visibility.Visible;
            headingTextBlock.Text = Heading;
            On_Navigated();
            if (Type == ResponseType.Home)
            {
                foreach (var item in list)
                {
                    videoGridView.Children.Add(new VideoTemplate() { VideoID = item.VideoId, Thumbnail = item.Thumbnail, Title = item.Title, Duration = item.Duration,Margin = new Thickness(5) });
                }
            }
            else if(Type == ResponseType.CourseList)
            {
                foreach (var item in courseList)
                {
                    videoGridView.Children.Add(new CourseTemplate() { Tag=item, Title = item.CourseName, Margin = new Thickness(5),Rating = item.Rating.ToString(), SuperParent = Parent });
                }
            }
            else if (Type == ResponseType.EditCourseVideo)
            {
                foreach (var item in list)
                {
                    videoGridView.Children.Add(new EditVideoTemplate() { VideoID = item.VideoId, Thumbnail = item.Thumbnail, VideoTitle = item.Title, Duration = item.Duration, Margin = new Thickness(5), CourseID = CourseID });
                }
            }
            else if(Type == ResponseType.Favorites)
            {
                foreach (var item in list)
                {
                    videoGridView.Children.Add(new EditVideoTemplate() { VideoID = item.VideoId, Thumbnail = item.Thumbnail, VideoTitle = item.Title, Duration = item.Duration, Margin = new Thickness(5), CourseID = CourseID, IsFavoritesType = true });
                }
            }
            else if (Type == ResponseType.Search)
            {
                if (_searchType == SearchType.Course)
                {
                    foreach (var item in courseList)
                    {
                        videoGridView.Children.Add(new CourseTemplate() { Tag = item, Title = item.CourseName, Margin = new Thickness(5), Rating = item.Rating.ToString(), SuperParent = Parent, IsReadOnly = true });
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        videoGridView.Children.Add(new VideoTemplate() { VideoID = item.VideoId, Thumbnail = item.Thumbnail, Title = item.Title, Duration = item.Duration, Margin = new Thickness(5) });
                    }
                }
            }
            if (videoGridView.Children.Count == 0)
                messageTextBlock.Visibility = Visibility.Visible;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
