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
using Microsoft.Kinect;
using Studie1Avatar;
using Schnittstellen;

namespace Studie1
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KinectSensor kinect;
        private Skeleton[] skeletonData;
        private Triggerable triggerable;
        private enum AttractionTypes { NONE, STATIC, AUDITIV, ANIMATED, AVATAR };
        private AttractionTypes attractionType;
        private List<int> lastUsers;

        public MainWindow()
        {
            InitializeComponent();
            attractionType = AttractionTypes.NONE;
            lastUsers = new List<int>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady); // Get Ready for Skeleton Ready Events
            kinect.SkeletonStream.AppChoosesSkeletons = true;
            kinect.Start(); // Start Kinect sensor
        }

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && this.skeletonData != null) // check that a frame is available
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData); // get the skeletal information in this frame

                    if (this.skeletonData.Length > 0 && triggerable != null)
                    {
                        int count = 0;
                        List<Skeleton> skeletons = new List<Skeleton>();
                        List<int> remaining = new List<int>();
                        remaining.AddRange(lastUsers);

                        foreach (Skeleton skeleton in this.skeletonData.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                        {
                            count++;
                            skeletons.Add(skeleton);
                            remaining.Remove(skeleton.TrackingId);
                            if (!lastUsers.Contains(skeleton.TrackingId))
                            {
                                this.userEntered(skeleton.TrackingId);
                            }
                        }

                        lastUsers.RemoveRange(0, lastUsers.Count);

                        foreach (Skeleton s in skeletons)
                        {
                            lastUsers.Add(s.TrackingId);
                        }

                        foreach (int id in remaining)
                        {
                            this.userLeft(id);
                        }

                        if (count > 0)
                        {
                            triggerable.triggerAction(skeletons);
                        }
                    }
                }
            }
        }

        private void userLeft(int id)
        {
            System.Console.WriteLine("User left: " + id);
        }

        private void userEntered(int id)
        {
            System.Console.WriteLine("User entered: " + id);
        }

        private void animated_Click(object sender, RoutedEventArgs e)
        {
            if (triggerable != null)
            {
                (triggerable as Window).Close();
                triggerable = null;
                attractionType = AttractionTypes.NONE;
            }
            else
            {
                Animated animatedWindow = new Animated();
                animatedWindow.Show();
                this.triggerable = animatedWindow;
                attractionType = AttractionTypes.ANIMATED;
            }
        }

        private void static_Click(object sender, RoutedEventArgs e)
        {
            if (triggerable != null)
            {
                (triggerable as Window).Close();
                triggerable = null;
                attractionType = AttractionTypes.NONE;
            }
            else
            {
                Static staticWindow = new Static();
                staticWindow.Show();
                this.triggerable = staticWindow;
                attractionType = AttractionTypes.STATIC;
            }
        }

        private void auditiv_Click(object sender, RoutedEventArgs e)
        {
            if (triggerable != null)
            {
                (triggerable as Window).Close();
                triggerable = null;
                attractionType = AttractionTypes.NONE;
            }
            else
            {
                Auditiv auditivWindow = new Auditiv();
                auditivWindow.Show();
                this.triggerable = auditivWindow;
                attractionType = AttractionTypes.AUDITIV;
            }
        }

        private void avatar_Click(object sender, RoutedEventArgs e)
        {
            if (triggerable != null)
            {
                (triggerable as Avatar).Exit();
                triggerable = null;
                attractionType = AttractionTypes.NONE;
            }
            else
            {
                Avatar game = new Avatar();
                this.triggerable = game;
                game.Run();
                attractionType = AttractionTypes.AVATAR;
            }
        }

    }
}
