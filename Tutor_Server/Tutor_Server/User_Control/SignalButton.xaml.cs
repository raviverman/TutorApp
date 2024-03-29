﻿using System;
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

namespace Tutor_Server.User_Control
{
    /// <summary>
    /// Interaction logic for SignalButton.xaml
    /// </summary>
    public partial class SignalButton : UserControl, INotifyPropertyChanged
    {
        private Color _color = Colors.Green;
        public Color Color {
            get
            {
                return this._color;
            } 
            set
            {
                _color = value;
                NotifyPropertyChanged();
            }
        }

        public SignalButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
