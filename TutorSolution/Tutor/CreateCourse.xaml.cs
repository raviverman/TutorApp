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
using System.Windows.Shapes;
using Tutor.Model.Data;
using Tutor.Model.Manager;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for CreateCourse.xaml
    /// </summary>
    public partial class CreateCourse : Window
    {
        public CreateCourse()
        {

            InitializeComponent();

        }

        public bool IsModifyCourseType { get; set; } = false;
        public string CourseID { get; set; }

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


        private async void createCourseButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(courseNameTextBlock.Text))
                return;
            progressRing.isActive = true;
            CreateCourseRequest request = new CreateCourseRequest()
            {
                CourseName = courseNameTextBlock.Text,
                UserId = SettingsManager.Username,
                Username = SettingsManager.FullName
            };
            if (IsModifyCourseType)
                request.UserId = CourseID;
            Object resp = await ConnectionManager.SendRequestAsync(request, IsModifyCourseType ? RequestType.ModifyCourse : RequestType.CreateCourse, ResponseType.Acknowledge);
            if (resp != null)
            {
                errorTextBlock.Text = ((JObject)resp).ToObject<Acknowledge>().Reason;
                if(IsModifyCourseType)
                    AppNotificationManager.PushMessage(new AppNotification() { Message = "Course changed successfully." });
                else
                    AppNotificationManager.PushMessage(new AppNotification() { Message= "Course created successfully." });
                Close();
            }
            else
                errorTextBlock.Text = "Connection Error";
            progressRing.isActive = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsModifyCourseType)
            {
                courseHeading.Text = "Edit Course";
                courseLabel.Text = "New Course Name";
                createCourseButton.Content = "Modify";
            }
        }
    }
}
