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
using System.Media;

namespace Studie1
{
    /// <summary>
    /// Interaktionslogik für Auditiv.xaml
    /// </summary>
    public partial class Auditiv : Window, Triggerable
    {
        public Auditiv()
        {
            InitializeComponent();
        }

        public void triggerAction()
        {
            SoundPlayer simpleSound = new SoundPlayer(@"greeting.wma");
            simpleSound.Play();
        }

        private void mouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            triggerAction();
        }
    }
}
