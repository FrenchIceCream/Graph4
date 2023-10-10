using System.CodeDom;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace Graph4
{
    public partial class Form1 : Form
    {
        private MyMatrix BezBukviKoef;
        private Graphics g; /*{ get { return Validate(); }; set { } }*/

        private List<PointF> points = new List<PointF>();
        private List<MyMatrix> matrixs = new List<MyMatrix>();
        private State state;
        private Pen _pen;
        private Pen _penRed;

        public Form1()
        {
            state = State.NoTask;
            InitializeComponent();
            BezBukviKoef = new MyMatrix(4, 4, new List<float>() { 1, -3, 3, -1, 0, 3, -6, 3, 0, 0, 3, -3, 0, 0, 0, 1 });
            g = Canvas.CreateGraphics();

            _pen = new Pen(Color.Black, 1);
            _penRed = new Pen(Color.Red, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            state = State.Task3Drawing;
        }

        private void AddDot(PointF point)
        {
            points.Add(point);
            if (points.Count <= 3)
            {
                if (matrixs.Count < 0)
                {
                    matrixs.Add(new MyMatrix(2, 4));
                }
                MyMatrix P;
                switch (points.Count)
                {
                    case 1:
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[0].X, points[0].X, points[0].X,
                            points[0].Y, points[0].Y, points[0].Y, points[0].Y });
                        break;
                    case 2:
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[0].X, points[1].X, points[1].X,
                            points[0].Y, points[0].Y, points[1].Y, points[1].Y });
                        break;
                    default:
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[1].X, points[1].X+(points[2].X -points[1].X)/2, points[2].X,
                            points[0].Y, points[1].Y, points[1].Y+(points[2].Y -points[1].Y)/2, points[2].Y });
                        break;
                }
                matrixs[0] = P * BezBukviKoef;

            }
            g.DrawRectangle(_penRed, point.X, point.Y, 1, 1);
        }

        private void Canvas_Click(object sender, EventArgs e)
        {
            if (state == State.Task3Drawing)
            {
                MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
                PointF p = new PointF(mouseEventArgs.X, mouseEventArgs.Y);
                AddDot(p);
            }
        }

        private void DrawCurve()
        {
            if (points.Count < 4)
            {
                return;
            }
            for (int i = 0; i < points.Count / 4; i++)
            {
                MyMatrix m = new MyMatrix(2, 4, new List<float>() { points[0].X, points[1].X, points[2].X, points[3].X,
                points[0].Y, points[1].Y, points[2].Y, points[3].Y});
                MyMatrix temp = m * BezBukviKoef;

                float singleT = 0;
                for (int k = 0; k < 500; k++)
                {
                    MyMatrix t = new MyMatrix(4, 1, new List<float>() {(float)Math.Pow(singleT,0),
                    (float)Math.Pow(singleT,1),
                    (float)Math.Pow(singleT,2),
                    (float)Math.Pow(singleT,3)});
                    MyMatrix tempRes = temp * t;
                    g.DrawRectangle(_pen, tempRes[0, 0], tempRes[1, 0], 1, 1);
                    singleT += 1 / 500f;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            state = State.Task3Line;
            DrawCurve();
        }
    }

    enum State { NoTask, Task1, Task2, Task3Drawing, Task3Line }

    class MyMatrix
    {
        public readonly int m, n;
        public float[,] matrix;
        public MyMatrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            matrix = new float[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                { matrix[i, j] = 0; }
            }

        }
        public MyMatrix(int m, int n, List<float> vals)
        {

            this.m = m;
            this.n = n;
            matrix = new float[m, n];
            for (int i = 0; i < m * n; i++)
            {
                matrix[i / n, i % n] = vals[i];
            }
        }

        public float this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        public static MyMatrix operator *(MyMatrix lhs, MyMatrix rhs)
        {
            if (lhs.n != rhs.m)
                throw new Exception("Неправильное перемножение матриц");

            MyMatrix res = new MyMatrix(lhs.m, rhs.n);
            for (int i = 0; i < lhs.m; i++)
            {
                for (int j = 0; j < rhs.n; j++)
                {
                    float temp = 0;

                    for (int k = 0; k < rhs.m; k++)
                    {
                        temp += lhs[i, k] * rhs[k, j];
                    }

                    res[i, j] = temp;
                }
            }

            return res;
        }

    }
}