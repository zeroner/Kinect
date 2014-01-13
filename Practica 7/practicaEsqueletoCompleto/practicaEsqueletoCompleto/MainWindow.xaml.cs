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

namespace practicaEsqueletoCompleto
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
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No se detecta ningun kinect", "Visor de Camara");
                Application.Current.Shutdown();
            }

            miKinect = KinectSensor.KinectSensors.FirstOrDefault();

            try
            {
                miKinect.SkeletonStream.Enable();
                miKinect.ColorStream.Enable();
                miKinect.Start();
            }
            catch
            {
                MessageBox.Show("La inicializacion del Kinect fallo", "Visor de camara");
                Application.Current.Shutdown();
            }

            miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
        }

        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvasEsqueleto.Children.Clear();
            Skeleton[] esqueletos = null;

            using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())
            {
                if (frameEsqueleto != null)
                {
                    esqueletos = new Skeleton[frameEsqueleto.SkeletonArrayLength];
                    frameEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint handJoint = esqueleto.Joints[JointType.HandRight];
                    Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight];

                    // Columna Vertebral
                    agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine]);

                    // Pierna Izquierda
                    agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter]);
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft]);
                    agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft]);
                    agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft]);
                    agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft]);

                    // Pierna Derecha
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight]);
                    agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight]);
                    agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight]);
                    agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight]);

                    // Brazo Izquierdo
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft]);
                    agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft]);
                    agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft]);

                    // Brazo Derecho
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight]);
                    agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight]);
                    agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight]);
                }
            }
        }

        void agregarLinea(Joint j1, Joint j2)
        {
            Line lineaHueso = new Line();
            lineaHueso.Stroke = new SolidColorBrush(Colors.Red);
            lineaHueso.StrokeThickness = 5;

            ColorImagePoint j1P = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;

            ColorImagePoint j2P = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;

            canvasEsqueleto.Children.Add(lineaHueso);
        }
    }
}
