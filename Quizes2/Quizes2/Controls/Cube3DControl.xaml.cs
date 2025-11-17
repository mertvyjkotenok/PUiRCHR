using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Quizes2.Controls
{
    public partial class Cube3DControl : UserControl
    {
        public string Text1 { get; set; }
        public string ScoreText { get; set; }
        public string ResultText { get; set; }
        public ImageSource ImageSide { get; set; }

        public Cube3DControl()
        {
            InitializeComponent();
        }

        // ---------- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ----------

        private Material CreateTextBrush(string text)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            var brush = new VisualBrush(tb);
            return new DiffuseMaterial(brush);
        }

        private Material CreateImageBrush(ImageSource img)
        {
            return new DiffuseMaterial(new ImageBrush(img));
        }

        private GeometryModel3D CreateFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Material material)
        {
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection { p0, p1, p2, p3 },
                TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 },
                TextureCoordinates = new PointCollection
                {
                    new Point(0,1),
                    new Point(1,1),
                    new Point(1,0),
                    new Point(0,0)
                }
            };

            return new GeometryModel3D(mesh, material);
        }

        // ---------- СОЗДАНИЕ КУБА ----------

        public void CreateCube()
        {
            CubeModel.Children.Clear();

            double s = 1;

            // FRONT  (Text1)
            CubeModel.Children.Add(CreateFace(
                new Point3D(-s, -s, s), new Point3D(s, -s, s),
                new Point3D(s, s, s), new Point3D(-s, s, s),
                CreateTextBrush(Text1)));

            // RIGHT  (ScoreText)
            CubeModel.Children.Add(CreateFace(
                new Point3D(s, -s, s), new Point3D(s, -s, -s),
                new Point3D(s, s, -s), new Point3D(s, s, s),
                CreateTextBrush(ScoreText)));

            // BACK  (ResultText)
            CubeModel.Children.Add(CreateFace(
                new Point3D(-s, -s, -s), new Point3D(s, -s, -s),
                new Point3D(s, s, -s), new Point3D(-s, s, -s),
                CreateTextBrush(ResultText)));

            // LEFT  (Image)
            if (ImageSide != null)
                CubeModel.Children.Add(CreateFace(
                    new Point3D(-s, -s, -s), new Point3D(-s, -s, s),
                    new Point3D(-s, s, s), new Point3D(-s, s, -s),
                    CreateImageBrush(ImageSide)));
        }

        // ---------- ВРАЩЕНИЕ ----------

        bool dragging = false;
        Point last;

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragging = true;
            last = e.GetPosition(this);
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            var p = e.GetPosition(this);
            double dx = p.X - last.X;
            double dy = p.Y - last.Y;

            RotY.Angle += dx * 0.5;
            RotX.Angle -= dy * 0.5;

            last = p;
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double z = Cam.Position.Z - e.Delta * 0.001;
            if (z < 1) z = 1;
            if (z > 20) z = 20;

            Cam.Position = new Point3D(0, 0, z);
        }
    }
}
