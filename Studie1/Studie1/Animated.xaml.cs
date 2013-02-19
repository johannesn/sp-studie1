using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Studie1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class Animated : Window, Triggerable
    {
        public Animated()
        {
            InitializeComponent();
        }

        public void triggerAction()
        {
            Storyboard storyboard = (Storyboard)FindResource("storyboard");
            storyboard.Begin(this);
        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            triggerAction();
        }
    }
}
