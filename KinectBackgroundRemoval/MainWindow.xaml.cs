using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Globalization;
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel;

namespace KinectBackgroundRemoval
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //variables to hold the path and filename information needed for saving files
        string path = "";
        string fileName = "";

        KinectSensor _sensor; //Kinect sensor object
        MultiSourceFrameReader _reader;  //kinect frame reader for getting the color frames

        //Create a background removal tool.
        BackgroundRemovalTool _backgroundRemovalTool;

        /// <summary>
        /// Main form constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //turn off the status message text that shows up in the middle of the screen since it isn't needed
            StatusMessage.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Window loaded is called when the window first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();  //initialize the sensor

                //Initialize the background removal tool.
                _backgroundRemovalTool = new BackgroundRemovalTool(_sensor.CoordinateMapper);

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.BodyIndex);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        /// <summary>
        /// Window closed is called as the program is closing to dispose of resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        /// <summary>
        /// This method takes in the color frame information from the Kinect.  The camera source gets the information about the
        /// picture so we can display the person(s) on the screen while removing the background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
            using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
            using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
            {

                if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                {

                    // 3) Update the image source.
                    camera.Source = _backgroundRemovalTool.GreenScreen(colorFrame, depthFrame, bodyIndexFrame);
                }

               
            }
        }


        #region "Button Clicks"

        //button click events for each one of the background-changing buttons on the right side of the form
        private void Beach_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Beach");
           
        }

        private void Underwater_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Underwater");

        }

        private void Mars_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Mars");

        }

        private void Park_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Park");

        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Space");

        }

        private void Motorcycle_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Motorcycle");

        }

        private void Sports_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Sports");

        }

        private void KimKanye_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("KimKanye");

        }

        private void TheQueen_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("TheQueen");

        }

        private void Bond_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Bond");

        }

        private void Superman_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Superman");

        }

        private void Liberty_Click(object sender, RoutedEventArgs e)
        {
            ((ImageBrush)Resources["myBrush"]).ImageSource = GetImage("Liberty");

        }


        #endregion



        /// <summary>
        /// This method finds the particular image resource needed by the button that was pressed so
        /// the background can change to the right image
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ImageSource GetImage(string name)
        {
            switch (name)
            {
                case "Beach":
                    return (ImageSource)FindResource("beach");
                case "Underwater":
                    return (ImageSource)FindResource("underwater");
                case "Mars":
                    return (ImageSource)FindResource("mars");
                case "Park":
                    return (ImageSource)FindResource("park");
                case "Space":
                    return (ImageSource)FindResource("space");
                case "Motorcycle":
                    return (ImageSource)FindResource("motorcycle");
                case "Sports":
                    return (ImageSource)FindResource("sports");
                case "KimKanye":
                    return (ImageSource)FindResource("kimKanye");
                case "TheQueen":
                    return (ImageSource)FindResource("theQueen");
                case "Bond":
                    return (ImageSource)FindResource("bond");
                case "Superman":
                    return (ImageSource)FindResource("superman");
                case "Liberty":
                    return (ImageSource)FindResource("liberty");
                default:
                    return null;
            }
        }


        /// <summary>
        /// Handles the user clicking on the screenshot button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {

            StatusMessage.Visibility = System.Windows.Visibility.Visible;            

            // Create a render target to which we'll render our composite image
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)grid.ActualWidth, (int)grid.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(grid);

                //hide the navigation
                navigationRegion.Visibility = System.Windows.Visibility.Hidden;
                screenshotButton.Visibility = System.Windows.Visibility.Hidden;
                StatusMessage.Visibility = System.Windows.Visibility.Hidden;

                dc.DrawRectangle(brush, null, new Rect(new Point(), new Size(grid.ActualWidth, grid.ActualHeight)));
            }

            renderBitmap.Render(dv);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            fileName = "KinectScreenshot-CoordinateMapping-" + time + ".png";

            path = Path.Combine(myPhotos, fileName);

            // Write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }
                
                //The camera has to be turned off or the user will be standing in front of the QR code window
                camera.Visibility = System.Windows.Visibility.Hidden;

                //upload the picture to Azure
                AzureUpload();

                //show the user the QR code that has been made for them
                //ViewQRCode();

            }
            catch (IOException)
            { }
            finally
            {
                //enable the navigation
                navigationRegion.Visibility = System.Windows.Visibility.Visible;
                screenshotButton.Visibility = System.Windows.Visibility.Visible;
                StatusMessage.Visibility = System.Windows.Visibility.Visible;
                StatusMessage.Text = "Picture Taken! Generating QR Code - Please wait";

            }
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Method to drive the process of uploading the file through the Azure connection
        /// A background worker process is used so the UI doesn't hang while the file is uploading
        /// </summary>
        private void AzureUpload()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }
        
        /// <summary>
        /// This method contains the code for the upload process to Azure. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            var credentials = new StorageCredentials("otchack",
                                        "tTCWj2dF+FGqUq6AdNIwaFGWeOYVYmWD2PHE8HCotVIFO57kDy2ulH2HLKAYnIp+YEWyCjQQF0/MMCqljYr6Tg==");
            var client = new CloudBlobClient(new Uri("http://otchack.blob.core.windows.net/"), credentials);

            // Retrieve a reference to a container. (You need to create one using the mangement portal, or call container.CreateIfNotExists())
            var container = client.GetContainerReference("kinect");

            // Retrieve reference to a blob named "myfile.gif".
            var blockBlob = container.GetBlockBlobReference(fileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(path))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        /// <summary>
        /// This method is run once the background worker process has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            StatusMessage.Visibility = System.Windows.Visibility.Hidden;

            //show the user the QR code that has been made for them
            ViewQRCode();
        }

        /// <summary>
        /// This method displays the child user control overlay with the QR code
        /// </summary>
        private void ViewQRCode()
        {
            var selectionDisplay = new QRCodeView(fileName);

            this.kinectRegionGrid.Children.Add(selectionDisplay);

            // Selection dialog covers the entire interact-able area, so the current press interaction
            // should be completed. Otherwise hover capture will allow the button to be clicked again within
            // the same interaction (even whilst no longer visible).
            selectionDisplay.Focus();

            // Since the selection dialog covers the entire interact-able area, we should also complete
            // the current interaction of all other pointers.  This prevents other users interacting with elements
            // that are no longer visible.
            this.kinectRegion.InputPointerManager.CompleteGestures();

        }  


    }
}
