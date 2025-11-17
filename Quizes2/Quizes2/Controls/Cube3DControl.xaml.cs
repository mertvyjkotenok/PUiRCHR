using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Quizes2.Controls
{
    /// <summary>
    /// Логика взаимодействия для Cube3DControl.xaml
    /// </summary>
    public partial class Cube3DControl : UserControl
    {
        private Point lastPos;
        private bool rotating = false;

        public string Text1 { get; set; }
        public string ScoreText { get; set; }
        public string ResultText { get; set; }
        public ImageSource ImageSide { get; set; }

        public Cube3DControl()
        {
            InitializeComponent();
            Loaded += Cube3DControl_Loaded;
        }

        private void Cube3DControl_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCube(); // заполняем CubeModel
        }

        private void CreateCube()
        {
            // Очищаем перед созданием (единственный раз!)
            CubeModel.Children.Clear();

            double s = 1.0;

            // Каждая грань — отдельная модель (GeometryModel3D)
            AddFace(CubeModel, CreateTextBrush(Text1),
                new Point3D(-s, -s, s),   // front
                new Vector3D(2 * s, 0, 0),
                new Vector3D(0, 2 * s, 0));

            AddFace(CubeModel, CreateTextBrush(ScoreText),
                new Point3D(s, -s, s),   // right
                new Vector3D(0, 0, -2 * s),
                new Vector3D(0, 2 * s, 0));

            AddFace(CubeModel, CreateTextBrush(ResultText),
                new Point3D(s, -s, -s),   // back
                new Vector3D(-2 * s, 0, 0),
                new Vector3D(0, 2 * s, 0));

            AddFace(CubeModel, CreateImageBrush(ImageSide),
                new Point3D(-s, -s, -s),   // left
                new Vector3D(0, 0, 2 * s),
                new Vector3D(0, 2 * s, 0));

            // top
            AddFace(CubeModel, CreateTextBrush(""),
                new Point3D(-s, s, s),
                new Vector3D(2 * s, 0, 0),
                new Vector3D(0, 0, -2 * s));

            // bottom
            AddFace(CubeModel, CreateTextBrush(""),
                new Point3D(-s, -s, s),
                new Vector3D(2 * s, 0, 0),
                new Vector3D(0, 0, -2 * s));
        }

        private void AddFace(Model3DGroup parent, Brush brush, Point3D origin, Vector3D dx, Vector3D dy)
        {
            var mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection()
                {
                    origin,
                    origin + dx,
                    origin + dx + dy,
                    origin + dy
                },
                TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 },
                TextureCoordinates = new PointCollection()
                {
                    new Point(0,1),
                    new Point(1,1),
                    new Point(1,0),
                    new Point(0,0),
                }
            };

            var material = new DiffuseMaterial(brush);
            var geom = new GeometryModel3D(mesh, material)
            {
                BackMaterial = material // важно: чтобы обе стороны отрисовывались одинаково (если нужно)
            };

            parent.Children.Add(geom);
        }

        private Brush CreateTextBrush(string text)
        {
            // Если текст пуст — возвращаем простой цвет
            if (string.IsNullOrEmpty(text))
                return Brushes.LightGray;

            // Рендерим текст в RenderTargetBitmap
            var bmp = new RenderTargetBitmap(512, 512, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, 512, 512));

                var ft = new FormattedText(text ?? "",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    28, Brushes.Black,
                    1.0);

                // Центрируем текст
                var x = 20;
                var y = (512 - ft.Height) / 2;
                dc.DrawText(ft, new Point(x, y));
            }
            bmp.Render(dv);

            return new ImageBrush(bmp) { ViewportUnits = BrushMappingMode.Absolute };
        }

        private Brush CreateImageBrush(ImageSource img)
        {
            if (img == null)
                return Brushes.LightGray;
            return new ImageBrush(img) { Stretch = Stretch.UniformToFill };
        }

        // --- Управление мышью для вращения и зума ---
        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastPos = e.GetPosition(View);
                rotating = true;
                Mouse.Capture(this);
             
            }
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rotating = false;
            Mouse.Capture(null);
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MOVE " + e.GetPosition(View));
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Cam == null) return;

            double z = Cam.Position.Z - e.Delta * 0.001;
            if (z < 1) z = 1;
            if (z > 20) z = 20;
            Cam.Position = new Point3D(Cam.Position.X, Cam.Position.Y, z);
        }


    }
}
