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
using Schnittstellen;
using Microsoft.Kinect;

namespace Studie1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class Animated : Window, Triggerable
    {
        private bool isAnimating;
        private Storyboard storyboard;

        public Animated()
        {
            InitializeComponent();

            storyboard = FindResource("storyboard") as Storyboard;
            storyboard.Completed += animationCompleted;
            isAnimating = false;
        }

        public void triggerAction(Skeleton[] skeletonData)
        {
            if (!isAnimating)
            {
                storyboard.Begin(this);
                isAnimating = true;
            }
        }

        public void animationCompleted(object sender, EventArgs e)
        {
            isAnimating = false;
        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            triggerAction(null);
        }
    }
}
