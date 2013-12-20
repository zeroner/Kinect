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

namespace DeteccionEsqueletos
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
            if (KinectSensor.KinectSensors.Count == 0) {
                MessageBox.Show("Ningun kinect detectado", "Visor de Posicion de Articulasion");
                Application.Current.Shutdown();
                return;
            }

            miKinect = KinectSensor.KinectSensors.FirstOrDefault();

            try
            {
                miKinect.SkeletonStream.Enable();
                miKinect.Start();
            }
            catch {
                MessageBox.Show("Fallo en la Iniciacion de Kinect", "Visor de Posicion de Articulasion");
                Application.Current.Shutdown();
            }

            miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
        }

        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            string mensaje = "No hay datos de esqueleto";
            string mensajeCaptura = "";
            Skeleton[] esqueletos = null;

            using (SkeletonFrame framesEsqueleto = e.OpenSkeletonFrame()) {
                if (framesEsqueleto != null) {
                    esqueletos = new Skeleton[framesEsqueleto.SkeletonArrayLength];
                    framesEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach(Skeleton esqueleto in esqueletos){
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked) {

                    if (esqueleto.ClippedEdges == 0)
                    {
                        mensajeCaptura = "Colocado Perfectamente";
                    }
                    else {
                        if ((esqueleto.ClippedEdges & FrameEdges.Bottom) != 0)
                            mensajeCaptura += "Moverse mas arriba";
                        if ((esqueleto.ClippedEdges & FrameEdges.Top) != 0)
                            mensajeCaptura += "Moverse mas abajo";
                        if ((esqueleto.ClippedEdges & FrameEdges.Right) != 0)
                            mensajeCaptura += "Moverse mas a la izquierda";
                        if ((esqueleto.ClippedEdges & FrameEdges.Left) != 0)
                            mensajeCaptura += "Moverse mas a la derecha";
                    }

                    Joint jointCabeza = esqueleto.Joints[JointType.HandRight];

                    SkeletonPoint posicionCabeza = jointCabeza.Position;

                    mensaje = string.Format("Cabeza: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", posicionCabeza.X, posicionCabeza.Y, posicionCabeza.Z);
                }
            }
            textBlockEstatus.Text = mensaje;
            textBlockCaptura.Text = mensajeCaptura;
        }

    }
}
