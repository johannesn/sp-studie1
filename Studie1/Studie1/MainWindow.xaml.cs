﻿using System;
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

        public MainWindow()
        {
            InitializeComponent();
            attractionType = AttractionTypes.NONE;
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

                        foreach (Skeleton skeleton in this.skeletonData.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                        {
                            //if (skeleton.Position.Z < 1200)
                            //{
                            count++;
                            skeletons.Add(skeleton);
                            //}
                        }

                        //System.Console.WriteLine("Skeleton Count = " + count);

                        if (count > 0)
                        {
                            triggerable.triggerAction(skeletons);
                            foreach (Skeleton s in skeletons)
                            {
                                System.Console.WriteLine(s.TrackingId + " " + s.Position.X + " " + s.Position.Y + " " + s.Position.Z);
                            }
                        }
                    }
                }
            }
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