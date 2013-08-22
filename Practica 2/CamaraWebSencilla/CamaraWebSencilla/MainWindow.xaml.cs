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
using Microsoft.Kinect;

namespace CamaraWebSencilla
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor miKinect;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            miKinect = KinectSensor.KinectSensors.FirstOrDefault();
            miKinect.Start();
            miKinect.ColorStream.Enable();
            miKinect.ColorFrameReady += miKinect_ColorFrameReady;
        }

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frameImagen = e.OpenColorImageFrame())
            {

                if (frameImagen == null)
                    return;

                byte[] datosColor = new byte[frameImagen.PixelDataLength];

                frameImagen.CopyPixelDataTo(datosColor);

                mostrarVideo.Source = BitmapSource.Create(
                    frameImagen.Width, frameImagen.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    datosColor,
                    frameImagen.Width * frameImagen.BytesPerPixel
                    );
            }
        }
    }
}