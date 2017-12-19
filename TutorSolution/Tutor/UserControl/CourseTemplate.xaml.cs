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
using Tutor.Pages;

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for CourseTemplate.xaml
    /// </summary>
    public partial class CourseTemplate : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        //Course ID in TAG
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }
        private string _rating;
        public string Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isReadOnly = false;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }

        public Frame SuperParent { get; set; }
        public CourseTemplate()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void mainContainer_Click(object sender, RoutedEventArgs e)
        {
            Object resp = await ConnectionManager.SendRequestAsync(new EditCourseVideos() { CourseID = ((CourseDetails)Tag).CourseID }, RequestType.EditCourseVideo, ResponseType.EditCourseVideo);
            if (resp != null && !IsReadOnly)
            {
                HomePage page1 = new HomePage((JObject)resp, ResponseType.EditCourseVideo, ((CourseDetails)Tag).CourseName) { CanGoBack = true, CourseID = ((CourseDetails)Tag).CourseID };
                SuperParent.Navigate(page1);
            }
            else if(resp != null && IsReadOnly)
            {
                HomePage page1 = new HomePage((JObject)resp, ResponseType.Home, ((CourseDetails)Tag).CourseName) { CanGoBack = true, CourseID = ((CourseDetails)Tag).CourseID };
                SuperParent.Navigate(page1);
            }
            //AppNotificationManager.PushMessage(new AppNotification() { Message = "Course Entered." });
        }

        private void addVideoButton_Click(object sender, RoutedEventArgs e)
        {
            AddVideo video = new AddVideo() { CourseID = ((CourseDetails)Tag) };
            video.Show();
        }

        private async void deleteVideoButton_Click(object sender, RoutedEventArgs e)
        {
            
            DeleteCourseRequest request = new DeleteCourseRequest() { CourseID = ((CourseDetails)Tag).CourseID };
            Object resp = await ConnectionManager.SendRequestAsync(request, RequestType.DeleteCourse, ResponseType.Acknowledge);
            if(resp!=null)
            {
                Acknowledge result = ((JObject)resp).ToObject<Acknowledge>();
                if(result.Status=="OK")
                {
                    ((WrapPanel)Parent).Children.Remove(this);
                }
                AppNotificationManager.PushMessage(new AppNotification() { Message = result.Reason });

            }
            else
                AppNotificationManager.PushMessage(new AppNotification() { Message = "Failed to delete course."});

           
        }

        private void modifyCourseButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCourse course = new CreateCourse() { IsModifyCourseType = true, CourseID = ((CourseDetails)Tag).CourseID };
            course.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(IsReadOnly)
            {
                modifyCourseButton.Visibility = Visibility.Collapsed;
                deleteVideoButton.Visibility = Visibility.Collapsed;
                addVideoButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}