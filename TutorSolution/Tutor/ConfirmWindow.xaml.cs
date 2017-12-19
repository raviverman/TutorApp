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

namespace Tutor
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public bool Result { get; set; } = false;
        public ConfirmWindow(string name)
        {
            InitializeComponent();
            deleteLabel.Text += name + " ?";
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

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
