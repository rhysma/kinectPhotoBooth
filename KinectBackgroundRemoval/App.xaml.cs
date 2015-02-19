using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Kinect.Wpf.Controls;

namespace KinectBackgroundRemoval
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal KinectRegion KinectRegion { get; set; }
    }
}
