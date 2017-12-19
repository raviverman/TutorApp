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

namespace Tutor.UserControl
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private string _tagName = "Default";
        private WrapPanel _parent = new WrapPanel();
        private bool _isReadOnlyType = false;

        public bool isReadOnlyType
        {
            get { return _isReadOnlyType; }
            set { _isReadOnlyType = value; }
        }


        public string TagName
        {
            get { return _tagName; }
            set
            {
                _tagName = value;
                NotifyPropertyChanged();
            }
        }
        new public WrapPanel Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                NotifyPropertyChanged();
            }
        }

        public Tags()
        {
            InitializeComponent();
            DataContext = this;
        }
        public Tags(WrapPanel parent)
        {
            InitializeComponent();
            Parent = parent;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Parent.Children.Remove(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DeleteTagButton.Visibility = isReadOnlyType ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
