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
using System.IO;
using System.Threading;
using Schnittstellen;
using Microsoft.Kinect;

namespace Studie1
{
    /// <summary>
    /// Interaktionslogik für Auditiv.xaml
    /// </summary>
    public partial class Auditiv : Window, Triggerable
    {
        private Thread newThread;

        public Auditiv()
        {
            InitializeComponent();
        }

        public void triggerAction(List<Skeleton> skeletonData)
        {
            if ((newThread == null || !newThread.IsAlive) && skeletonData.Count > 0)
            {
                newThread = new Thread(this.playSound);
                newThread.Start(); 
            }
        }

        private void mouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<Skeleton> data = new List<Skeleton>();
            data.Add(new Skeleton());
            triggerAction(data);
        }

        private void playSound()
        {
            Stream audioStream = Properties.Resources.greeting;
            audioStream.Position = 0;
            SoundPlayer simpleSound = new SoundPlayer(audioStream);
            simpleSound.PlaySync();
        }
    }
}
