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

namespace Tutor_Server.User_Control
{
    /// <summary>
    /// Interaction logic for ListItem.xaml
    /// </summary>
    public partial class ListItem : UserControl
    {
        public String Text { get; set; }
        public Double HintWidth { get; set; } = 5;
        public Thickness PaddingText { get; set; } = new Thickness(0, 0, 0, 0);
        public SolidColorBrush HintColor { get; set; } = new SolidColorBrush(Colors.LightBlue);
        public ListItem()
        {
            InitializeComponent();
            this.DataContext = this;
            //this.DataContextChanged += (s, e) => 
        }
    }
}
