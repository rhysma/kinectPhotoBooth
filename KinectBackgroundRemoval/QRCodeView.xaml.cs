using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Diagnostics;

namespace KinectBackgroundRemoval
{
    /// <summary>
    /// Interaction logic for QRCodeView.xaml
    /// </summary>
    public partial class QRCodeView : UserControl
    {
        public string FileName
        {
            get;
            set;
        }
    
        /// <summary>
        /// Initial constructor for the overlay control.  Gets the filename information from the main window so we can
        /// build the right path for the QR code
        /// </summary>
        /// <param name="fileName"></param>
        public QRCodeView(string fileName)
        {
            FileName = fileName;

            InitializeComponent();

            //create a new image that will hold the QR code
            Image img = new Image();
            img.Height = 300;
            img.Width = 300;

            //this is the path to the Google QR code generator.  The filename of the image uploaded to Azure is
            //appended to the end so the QR code will take the user to the correct image on the server
            string fullFilePath = "https://chart.googleapis.com/chart?chs=300x300&cht=qr&choe=UTF-8&chl=http://otchack.azurewebsites.net/share.php?img=" + FileName;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bi.EndInit();


            //give the image source to the image and add it to the page
            img.Source = bi;
            wrapPanel1.Children.Add(img);
        }

        /// <summary>
        /// This method is called when the close button is clicked by the user after they have snapped their QR code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = (Panel)this.Parent;
            parent.Children.Remove(this);

            //Since the camera is off and we cannot restart it now, the application has to shutdown and reopen
            //to reinitialize the Kinect camera
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }   
            
    }
}
