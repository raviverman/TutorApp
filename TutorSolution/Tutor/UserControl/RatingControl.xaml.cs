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
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : System.Windows.Controls.UserControl
    {
        public RatingControl()
        {
            InitializeComponent();
        }

        public string CourseID { get; set; }
        public TextBlock RatingBlock { get; set; }
        private bool ratingClicked = false;

        private Button[] ratingButtons = new Button[5];

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ratingButtons[0] = star1;
            ratingButtons[1] = star2;
            ratingButtons[2] = star3;
            ratingButtons[3] = star4;
            ratingButtons[4] = star5;
            Deselect();
        }

        private void Select(int count)
        {
            for (int i = 0; i <= count; i++)
            {
                ratingButtons[i].Foreground = new SolidColorBrush(Colors.Gold);
            }
        }

        private void Deselect()
        {
            foreach (var item in ratingButtons)
            {
                item.Foreground = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void star_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ratingClicked)
                return; 
            var selectedStar = ((Button)sender);
            Select(int.Parse(selectedStar.Tag.ToString()));

        }

        private void star_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!ratingClicked)
                Deselect();
        }

        private async void RateCourse(object sender, RoutedEventArgs e)
        {
            if (ratingClicked)
                return;
            var clickedButton = ((Button)sender);
            int start = int.Parse(clickedButton.Tag.ToString());
            for(int i = start;i>=0;i--)
            {
                ratingButtons[i].FontSize +=3;
                await Task.Delay(150);
                ratingButtons[i].FontSize -=3;

            }
            ratingProgress.isActive = true;
            RatingRequest request = new RatingRequest() { CourseID = CourseID, Rating = start + 1, UserID = SettingsManager.Username };
            Object response = await ConnectionManager.SendRequestAsync(request, RequestType.RateCourse, ResponseType.RateCourse);
            if(response==null || ((JObject)response).ToObject<RatingResponse>().Status!="OK")
            {
                ratingClicked = false;
                Deselect();
            }
            else
            {
                RatingBlock.Text = ((JObject)response).ToObject<RatingResponse>().NewRating.ToString("0.00");
            }
            ratingProgress.isActive = false;
            ratingClicked = true;

        }
    }
}
