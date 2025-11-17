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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Quizes2.Controls;

namespace Quizes2
{
    public partial class ResultWindow : Window
    {
        public ResultWindow(string title, int score, string result)
        {
            InitializeComponent();

            TitleText.Text = title;
            ScoreText.Text = $"Баллы: {score}";
            ResultText.Text = result;

            CreateCube();
        }

        private void CreateCube()
        {
            CubeModel.Children.Clear();

            MeshGeometry3D mesh = new MeshGeometry3D();

            // вершины куба
            Point3D[] p =
            {
                new Point3D(-1,-1,-1),
                new Point3D( 1,-1,-1),
                new Point3D( 1, 1,-1),
                new Point3D(-1, 1,-1),
                new Point3D(-1,-1, 1),
                new Point3D( 1,-1, 1),
                new Point3D( 1, 1, 1),
                new Point3D(-1, 1, 1)
            };

            int[] idx =
            {
                // Front face (z = -1)
                0,2,1, 0,3,2,

                // Right face
                1,2,6, 1,6,5,

                // Back face (z = +1)
                5,6,7, 5,7,4,

                // Left face
                4,7,3, 4,3,0,

                // Top face
                3,7,6, 3,6,2,

                // Bottom face
                4,0,1, 4,1,5
            };


            foreach (var i in idx)
                mesh.Positions.Add(p[i]);

            GeometryModel3D cube = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.SkyBlue))
            };

            CubeModel.Children.Add(cube);

            // вращение
            var rot = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            var tr = new RotateTransform3D(rot);

            cube.Transform = tr;

            var animation = new System.Windows.Media.Animation.DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = new Duration(System.TimeSpan.FromSeconds(6)),
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };

            rot.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }
    }
}
