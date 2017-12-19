using Microsoft.Win32;
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
using Tutor.Model.Data;
using Tutor.Model.Manager;

namespace Tutor
{
    /// <summary>
    /// Interaction logic for ProfileSettings.xaml
    /// </summary>
    public partial class ProfileSettings : Window
    {

        private bool isPhotoPicked = false;
        private BitmapImage imagePicked = new BitmapImage();
        private string filePath = "";
        new private int Width = 0;
        new private int Height = 0;
        private int Minimum = 0;
        private int OriginX = 0;
        private int OriginY = 0;


        public ProfileSettings()
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

        private async void saveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            progressRing.isActive = true;
            if (isPhotoPicked)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder()
                {
                    QualityLevel = 50
                };
                CroppedBitmap bm = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                if (!Directory.Exists("Temporary"))
                    Directory.CreateDirectory("Temporary");
                encoder.Frames.Add(BitmapFrame.Create(bm));
                using (FileStream fs = new FileStream("Temporary/" + SettingsManager.Username + ".jpg", FileMode.OpenOrCreate))
                    encoder.Save(fs);
            }

            ProfileUpdateRequest request = new ProfileUpdateRequest()
            {
                FullName = fullNameTextBlock.Text,
                Username = SettingsManager.Username,
                RequiresImageUpdate = isPhotoPicked,
                RequiresPasswordUpdate = !String.IsNullOrWhiteSpace(passwordTextBlock.Password),
                OldPassword = Security.MD5Hash(passwordTextBlock.Password),
                Password = Security.MD5Hash(newPasswordTextBlock.Password)
            };
            Object resp = await ConnectionManager.SendRequestAsync(request, RequestType.UpdateProfile, ResponseType.UpdateProfile);
            if (resp != null)
            {
                ProfileUpdateResponse response = ((JObject)resp).ToObject<ProfileUpdateResponse>();
                if (response.Status == "OK")
                {
                    if(isPhotoPicked)
                        resp = await ConnectionManager.SendFileAsync("Temporary/" + SettingsManager.Username + ".jpg",null);
                    if (resp != null)
                    {
                        var response2 = ((JObject)resp).ToObject<Acknowledge>();
                        errorTextBlock.Text = response2.Reason;
                        SettingsManager.UpdateImage();
                        SettingsManager.FullName = SettingsManager.FullName;
                        SettingsManager.Password = SettingsManager.Password;
                        if (File.Exists("Temporary/" + SettingsManager.Username + ".jpg"))
                            File.Delete("Temporary/" + SettingsManager.Username + ".jpg");
                        if(response2.Status == "OK")
                        AppNotificationManager.PushMessage(new AppNotification() { Message = "Profile Updated Successfully." });
                        Close();
                    }
                    else
                        errorTextBlock.Text = "Connection Error";
                    if (File.Exists("Temporary/" + SettingsManager.Username + ".jpg"))
                        File.Delete("Temporary/" + SettingsManager.Username + ".jpg");
                }
                else
                {
                    errorTextBlock.Text = response.Reason;
                }
            }

            progressRing.isActive = false;
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "Image Files JPEG, PNG |*.jpg;*.jpeg;*.png"
            };

            var result = fileDialog.ShowDialog();
            if(result!=null && result==true)
            {
                profilePhoto.Source = new BitmapImage(new Uri(fileDialog.FileName, UriKind.RelativeOrAbsolute));
                imagePicked = new BitmapImage(new Uri(fileDialog.FileName, UriKind.RelativeOrAbsolute));
                isPhotoPicked = true;
                Height = imagePicked.PixelHeight;
                Width = imagePicked.PixelWidth;

                Minimum =  Width <  Height ?  Width :  Height;
                
                filePath = fileDialog.FileName;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(0, 0, Minimum-1, Minimum-1));
                profilePhoto.Source = bp;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fullNameTextBlock.Text = SettingsManager.FullName;
            profilePhoto.Source = new BitmapImage(new Uri(SettingsManager.ProfilePath, UriKind.RelativeOrAbsolute));
        }


        private void left_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if((OriginX-delta) > 0)
            {
                OriginX = OriginX - delta;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum , Minimum));
                profilePhoto.Source = bp;
            }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if ((OriginY - delta) > 0)
            {
                OriginY = OriginY - delta;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;
            }
        }

        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if ((OriginX + Minimum + delta) <= Width)
            {
                OriginX = OriginX + delta;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if ((OriginY + Minimum + delta) <= Height)
            {
                OriginY = OriginY + delta;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;
                
            }
        }

        private void zoominButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if ((OriginX + Minimum - delta*5) <= Width && (OriginY + Minimum - delta * 5) <= Height)
            {
                Minimum -= delta*5;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;

            }
            
        }
        private void zoomoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPhotoPicked)
                return;
            int delta = (int)senstivityBar.Value;
            if ((OriginX + Minimum + delta*5) <= Width && (OriginX + Minimum + delta * 5) <= Height)
            {
                Minimum += delta*5;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;

            }
            else if ((OriginX - delta * 5) >= 0 && (OriginY - delta * 5) > 0)
            {
                OriginX -= delta * 5;
                OriginY -= delta * 5;
                Minimum += delta * 5;
                CroppedBitmap bp = new CroppedBitmap(imagePicked, new Int32Rect(OriginX, OriginY, Minimum, Minimum));
                profilePhoto.Source = bp;
            }
        }
    }
}
