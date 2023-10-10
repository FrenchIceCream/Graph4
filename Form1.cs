using System.CodeDom;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Graph4
{
    public partial class Form1 : Form
    {
        private const int max_depth = 5;
        private Pen pen_tree;
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

        private void button1_Click(object sender, EventArgs e)
        {
            float length = 50;
            Dictionary<char, string> rules = new Dictionary<char, string>();


            string[] lines = File.ReadAllLines("rules.txt");
            string[] lines2 = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            char atom = char.Parse(lines2[0]);
            float deg = float.Parse(lines2[1]);
            string axiom = lines2[2];
            for (int i = 1; i < lines.Length; i++)
            {
                var temp = lines[i].Split("->");
                rules[char.Parse(temp[0])] = temp[1];
            }
            string res_string = TranslateRules(rules, axiom);

            Stack<Tuple<int, int, float>> states = new Stack<Tuple<int, int, float>> { };
            List<Tuple<Point, int>> points = new List<Tuple<Point, int>> { };

            CalcLSystem(res_string, deg, length, 0, 0, states, ref points);

            DrawLSystem(points);
        }

        private string TranslateRules(Dictionary<char, string> rules, string axiom)
        {
            string res = axiom;
            for (int j = 0; j < max_depth; j++)
            {
                string temp = "";
                for (int i = 0; i < res.Length; i++)
                {
                    char cur = res[i];
                    if (rules.ContainsKey(cur))
                    {
                        string cur_rule = rules[cur];
                        temp += cur_rule;
                    }
                    else
                    {
                        if (cur == '+')
                            temp += "+";
                        else if (cur == '-')
                            temp += "-";
                        else if (cur == '(')
                            temp += "(";
                        else if (cur == ')')
                            temp += ")";
                    }
                }
                res = temp;
            }
            return res;
        }

        private void CalcLSystem(string res_string, float deg, float length, int cur_x, int cur_y, Stack<Tuple<int, int, float>> states, ref List<Tuple<Point, int>> points, bool rnd = false)
        {
            int branch_depth = 0;
            points.Add(new Tuple<Point, int>(new Point(cur_x, cur_y), 0));
            Random r = new Random();
            float cur_deg = 0;
            for (int i = 0; i < res_string.Length; i++)
            {
                char cur = res_string[i];
                if (cur == '+')
                {
                    cur_deg += rnd ? deg + r.Next(-20, 20) : deg;
                }
                else if (cur == '-')
                {
                    cur_deg -= rnd ? deg + r.Next(-20, 20) : deg;
                }
                else if (cur == '(')
                {
                    states.Push(new Tuple<int, int, float>(cur_x, cur_y, cur_deg));
                    length /= 2;
                    branch_depth++;
                }
                else if (cur == ')')
                {
                    var st = states.Peek();
                    length *= 2;
                    branch_depth--;
                    states.Pop();
                    cur_x = st.Item1;
                    cur_y = st.Item2;
                    cur_deg = st.Item3;
                }
                else
                {
                    cur_x = (int)Math.Round(cur_x + Math.Cos(cur_deg / 180 * Math.PI) * length);
                    cur_y = (int)Math.Round(cur_y - Math.Sin(cur_deg / 180 * Math.PI) * length);
                    points.Add(new Tuple<Point, int>(new Point(cur_x, cur_y), branch_depth));
                }
            }
        }

        private void DrawLSystem(List<Tuple<Point, int>> points)
        {

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