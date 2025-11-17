using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Quizes1
{
    public class ResultPyramidForm : Form
    {
        private float angleX = -20f;
        private float angleY = 20f;
        private Point lastMouse;
        private bool isDragging = false;
        private System.Windows.Forms.Timer animTimer;
        private Bitmap[] faceTextures = new Bitmap[4];
        private float scale = 500f;
        private float distance = 4f;

        // Geometry: square base centered at origin, apex above
        private List<Point3D> vertices;

        public ResultPyramidForm(string congratsText, int score, string resultText, string imagePath)
        {
            this.Text = "Результат: Пирамида";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;

            CreateTextures(congratsText, score, resultText, imagePath);
            BuildGeometry();

            this.MouseDown += ResultPyramidForm_MouseDown;
            this.MouseMove += ResultPyramidForm_MouseMove;
            this.MouseUp += ResultPyramidForm_MouseUp;

            animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 20;
            animTimer.Tick += (s, e) => { angleY += 0.5f; Invalidate(); };
            animTimer.Start();
        }

        private void CreateTextures(string congratsText, int score, string resultText, string imagePath)
        {
            // Face 0: congrats text
            faceTextures[0] = new Bitmap(400, 300);
            using (var g = Graphics.FromImage(faceTextures[0]))
            {
                g.Clear(Color.White);
                g.DrawString(congratsText, new Font("Arial", 18, FontStyle.Bold), Brushes.Black, new RectangleF(10, 10, 380, 280));
            }

            // Face 1: score
            faceTextures[1] = new Bitmap(400, 300);
            using (var g = Graphics.FromImage(faceTextures[1]))
            {
                g.Clear(Color.White);
                g.DrawString($"Набрано баллов: {score}", new Font("Arial", 20, FontStyle.Bold), Brushes.Black, new RectangleF(10, 10, 380, 280));
            }

            // Face 2: result text
            faceTextures[2] = new Bitmap(400, 300);
            using (var g = Graphics.FromImage(faceTextures[2]))
            {
                g.Clear(Color.White);
                g.DrawString(resultText, new Font("Arial", 14), Brushes.Black, new RectangleF(10, 10, 380, 280));
            }

            // Face 3: image
            if (System.IO.File.Exists(imagePath))
            {
                var img = (Bitmap)Image.FromFile(imagePath);
                // fit to texture
                faceTextures[3] = new Bitmap(400, 300);
                using (var g = Graphics.FromImage(faceTextures[3]))
                {
                    g.Clear(Color.White);
                    g.DrawImage(img, new Rectangle(0, 0, 400, 300));
                }
            }
            else
            {
                faceTextures[3] = new Bitmap(400, 300);
                using (var g = Graphics.FromImage(faceTextures[3]))
                {
                    g.Clear(Color.LightGray);
                    g.DrawString("123.png не найден\nПоложите 123.png рядом с exe", new Font("Arial", 12), Brushes.Black, new RectangleF(10, 10, 380, 280));
                }
            }
        }

        private void BuildGeometry()
        {
            vertices = new List<Point3D>();
            float half = 1.0f;
            // base square (y = 0), clockwise
            vertices.Add(new Point3D(-half, 0, -half)); // 0
            vertices.Add(new Point3D(half, 0, -half));  // 1
            vertices.Add(new Point3D(half, 0, half));   // 2
            vertices.Add(new Point3D(-half, 0, half));  // 3
            // apex
            vertices.Add(new Point3D(0, 1.2f, 0));      // 4
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.White);

            // simple camera transform
            var faces = new List<FaceToDraw>();

            // lateral faces: triangles (0,1,4), (1,2,4), (2,3,4), (3,0,4)
            int[][] indices = new int[][]
            {
                new[]{0,1,4},
                new[]{1,2,4},
                new[]{2,3,4},
                new[]{3,0,4}
            };

            for (int f = 0; f < indices.Length; f++)
            {
                var idx = indices[f];
                var p0 = Project(Rotate(vertices[idx[0]]));
                var p1 = Project(Rotate(vertices[idx[1]]));
                var p2 = Project(Rotate(vertices[idx[2]]));

                // back-face culling: compute normal z in camera space
                var camP0 = Rotate(vertices[idx[0]]);
                var camP1 = Rotate(vertices[idx[1]]);
                var camP2 = Rotate(vertices[idx[2]]);
                var v1 = camP1 - camP0;
                var v2 = camP2 - camP0;
                var normal = v1.Cross(v2);
                if (normal.Z > 0) // facing camera (note: might need sign flip)
                {
                    // average depth for sorting
                    float depth = (camP0.Z + camP1.Z + camP2.Z) / 3f;
                    faces.Add(new FaceToDraw { Points = new PointF[] { p0, p1, p2 }, Depth = depth, Texture = faceTextures[f] });
                }
            }

            // sort by depth (far to near)
            foreach (var face in faces.OrderBy(x => x.Depth))
            {
                // map texture (rectangle) to triangle
                var dest = face.Points;
                PointF[] destPts = new PointF[] { dest[0], dest[1], dest[2] };
                Rectangle srcRect = new Rectangle(0, 0, face.Texture.Width, face.Texture.Height);
                // DrawImage with 3 dest points maps the image to triangle
                e.Graphics.DrawImage(face.Texture, destPts, srcRect, GraphicsUnit.Pixel);

                // optional border
                e.Graphics.DrawPolygon(Pens.Black, destPts);
            }
        }

        private PointF Project(Point3D p)
        {
            // perspective projection
            float z = p.Z + distance; // move camera back
            float factor = scale / z;
            float x = this.ClientSize.Width / 2 + p.X * factor;
            float y = this.ClientSize.Height / 2 - p.Y * factor;
            return new PointF(x, y);
        }

        private Point3D Rotate(Point3D p)
        {
            // rotate around X then Y
            double radX = angleX * Math.PI / 180.0;
            double radY = angleY * Math.PI / 180.0;

            // rotate X
            double cosy = Math.Cos(radY), siny = Math.Sin(radY);
            double cosx = Math.Cos(radX), sinx = Math.Sin(radX);

            // apply Y rotation
            double x1 = p.X * cosy + p.Z * siny;
            double z1 = -p.X * siny + p.Z * cosy;
            double y1 = p.Y;

            // apply X rotation
            double y2 = y1 * cosx - z1 * sinx;
            double z2 = y1 * sinx + z1 * cosx;
            return new Point3D((float)x1, (float)y2, (float)z2);
        }

        private void ResultPyramidForm_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastMouse = e.Location;
            animTimer.Stop();
        }

        private void ResultPyramidForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;
            var dx = e.X - lastMouse.X;
            var dy = e.Y - lastMouse.Y;
            angleY += dx * 0.5f;
            angleX += dy * 0.5f;
            lastMouse = e.Location;
            Invalidate();
        }

        private void ResultPyramidForm_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            animTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            animTimer?.Stop();
            foreach (var b in faceTextures) b?.Dispose();
        }

        private class FaceToDraw
        {
            public PointF[] Points;
            public float Depth;
            public Bitmap Texture;
        }

        private struct Point3D
        {
            public float X, Y, Z;
            public Point3D(float x, float y, float z) { X = x; Y = y; Z = z; }
            public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            public Point3D Cross(Point3D other) => new Point3D(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
        }
    }
}
