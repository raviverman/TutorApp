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

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : System.Windows.Controls.UserControl
    {
        private bool _isActive = false;
        public bool isActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (_isActive)
                    mainGrid.Visibility = Visibility.Visible;
                else
                    mainGrid.Visibility = Visibility.Hidden;
            }
        }
        private SolidColorBrush _foregroundbrush  = new SolidColorBrush(Color.FromArgb(255, 34, 177, 76));
        public SolidColorBrush ForegroundBrush
        {
            get
            {
                return _foregroundbrush;
            }
            set
            {
                _foregroundbrush = value;
            }
        }

        public Progress()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rotator.StrokeDashOffset = this.Height / 50;
            rotator.StrokeThickness = this.Height / 10;
            rotator.Stroke = ForegroundBrush;
        }
    }
}
