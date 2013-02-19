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

namespace Studie1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class Avatar : Window, Triggerable
    {
        public Avatar()
        {
            InitializeComponent();
        }

        public void triggerAction()
        {

        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            triggerAction();
        }
    }
}
