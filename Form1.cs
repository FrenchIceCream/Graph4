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
        private const int max_depth = 3;
        private Pen pen_tree;
        private MyMatrix BezBukviKoef;
        private Graphics g; /*{ get { return Validate(); }; set { } }*/
        private int NotFinishedDots = 0;

        private List<PointF> points = new List<PointF>();
        private List<PointF> realPoints = new List<PointF>();
        private List<MyMatrix> matrixs = new List<MyMatrix>();
        private State state;
        private Pen _pen;
        private Pen _penRed;
        private Pen _penGreen;
        private readonly float _searchDistance = 10;
        private int _selectedDot = -1;

        public Form1()
        {
            state = State.NoTask;
            InitializeComponent();
            BezBukviKoef = new MyMatrix(4, 4, new List<float>() { 1, -3, 3, -1, 0, 3, -6, 3, 0, 0, 3, -3, 0, 0, 0, 1 });
            g = Canvas.CreateGraphics();

            _pen = new Pen(Color.Black, 1);
            _penRed = new Pen(Color.Red, 1);
            pen_tree = new Pen(Color.RosyBrown, 2);
            _penGreen = new Pen(Color.Green, 2);
            slider.Maximum = 30;
            slider.Minimum = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float length = slider.Value * 2;
            Dictionary<char, string> rules = new Dictionary<char, string>();

            string[] lines = File.ReadAllLines("../../../rules.txt");
            string[] lines2 = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            char atom = char.Parse(lines2[0]);
            float deg = float.Parse(lines2[1]);
            string axiom = lines2[2];
            for (int i = 1; i < lines.Length; i++)
            {
                var temp = lines[i].Split("->");
                rules[char.Parse(temp[0])] = temp[1];
            }
            string res_string = TranslateRules(rules, axiom, ref length);

            Debug.WriteLine(res_string);

            Stack<Tuple<int, int, float, int>> states = new Stack<Tuple<int, int, float, int>> { };
            List<Tuple<Point, int>> points = new List<Tuple<Point, int>> { };

            CalcLSystem(res_string, deg, length, Canvas.Width / 2, Canvas.Height - 100, states, ref points, is_random.Checked);

            DrawLSystem(points);
        }

        private string TranslateRules(Dictionary<char, string> rules, string axiom, ref float length)
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
                        else
                            temp += cur;
                    }
                }
                res = temp;
            }

            return res;
        }

        private void CalcLSystem(string res_string, float deg, float length, int cur_x, int cur_y, Stack<Tuple<int, int, float, int>> states, ref List<Tuple<Point, int>> points, bool rnd = false)
        {
            int branch_depth = 1;
            points.Add(new Tuple<Point, int>(new Point(cur_x, cur_y), 0));
            Random r = new Random();
            float cur_deg = rnd ? 90 : 0;
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
                    branch_depth++;
                    states.Push(new Tuple<int, int, float, int>(cur_x, cur_y, cur_deg, branch_depth));
                }
                else if (cur == ')')
                {
                    var st = states.Peek();
                    states.Pop();
                    cur_x = st.Item1;
                    cur_y = st.Item2;
                    cur_deg = st.Item3;
                    branch_depth = st.Item4;
                    points.Add(new Tuple<Point, int>(new Point(cur_x, cur_y), branch_depth));
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
            g.Clear(Color.White);
            var c1 = Color.FromArgb(1, 23, 102, 20);
            var c2 = Color.FromArgb(1, 120, 80, 54);
            var max_d = points.Max(x => x.Item2);
            if (max_d == 1)
            {
                pen_tree.Width = 1;
                c1 = c2 = Color.Black;
            }
            else
                pen_tree.Width = max_depth * 2 - 2;
            Debug.WriteLine(max_d);

            var a = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                var cur = points[i];

                float coef = (float)(max_d - cur.Item2) / (float)(max_d);
                var c = Color.FromArgb(255, (int)(c1.R * (1f - coef) + c2.R * coef), (int)(c1.G * (1f - coef) + c2.G * coef), (int)(c1.B * (1f - coef) + c2.B * coef));
                pen_tree.Color = c;

                if (cur.Item2 >= a.Item2)
                {
                    if (cur.Item2 > a.Item2)
                        pen_tree.Width = pen_tree.Width - (cur.Item2 - a.Item2);
                    g.DrawLine(pen_tree, a.Item1, cur.Item1);
                }
                else
                    pen_tree.Width = pen_tree.Width + (a.Item2 - cur.Item2);
                a = cur;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            foreach (var p in points)
            {
                g.DrawRectangle(_penRed, p.X, p.Y, 1, 1);
            }
            state = State.Task3Drawing;
        }

        private void AddDot(PointF point)
        {
            points.Add(point);
            g.DrawRectangle(_penRed, point.X, point.Y, 1, 1);

            if (points.Count <= 4)
            {
                if (matrixs.Count == 0)
                {
                    matrixs.Add(new MyMatrix(2, 4));
                }
                MyMatrix P;
                switch (points.Count)
                {
                    case 1:
                        realPoints.Add(point);
                        realPoints.Add(point);
                        realPoints.Add(point);
                        realPoints.Add(point);
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[0].X, points[0].X, points[0].X,
                            points[0].Y, points[0].Y, points[0].Y, points[0].Y });
                        NotFinishedDots = 3;
                        break;
                    case 2:
                        realPoints[2] = (point);
                        realPoints[3] = (point);
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[0].X, points[1].X, points[1].X,
                            points[0].Y, points[0].Y, points[1].Y, points[1].Y });
                        NotFinishedDots = 2;
                        break;
                    case 3:
                        realPoints[1] = points[2];
                        realPoints[3] = (point);
                        realPoints[2] = new PointF((points[2].X + points[1].X) / 2, (points[2].Y + points[1].Y) / 2);
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[1].X, realPoints[realPoints.Count-2].X, points[2].X,
                            points[0].Y, points[1].Y, realPoints[realPoints.Count-2].Y, points[2].Y });
                        NotFinishedDots = 1;
                        break;
                    default:

                        realPoints[realPoints.Count - 2] = realPoints[realPoints.Count - 1];
                        realPoints[realPoints.Count - 1] = point;
                        P = new MyMatrix(2, 4, new List<float>() { points[0].X, points[1].X, points[2].X, points[3].X,
                            points[0].Y, points[1].Y, points[2].Y, points[3].Y });
                        NotFinishedDots = 0;
                        break;
                }
                matrixs[0] = P * BezBukviKoef;

            }
            else
            {
                switch (NotFinishedDots)
                {
                    case 0:
                        realPoints.Add(realPoints[realPoints.Count - 1]); // ���������� ��������� �����
                        realPoints[realPoints.Count - 2] = new PointF((realPoints[realPoints.Count - 3].X + realPoints[realPoints.Count - 1].X) / 2,
                            (realPoints[realPoints.Count - 3].Y + realPoints[realPoints.Count - 1].Y) / 2);//������������ ��� �������� �������������
                        MyMatrix P = new MyMatrix(2, 4, new List<float>()
                        { realPoints[realPoints.Count - 5].X, realPoints[realPoints.Count - 4].X, realPoints[realPoints.Count - 3].X, realPoints[realPoints.Count - 2].X,
                        realPoints[realPoints.Count - 5].Y, realPoints[realPoints.Count - 4].Y, realPoints[realPoints.Count - 3].Y, realPoints[realPoints.Count - 2].Y});
                        matrixs[matrixs.Count - 1] = P * BezBukviKoef;// ���������� ����� ������� � ������ �����������������

                        //��������� ��� ���� �������� �����
                        realPoints.Add(new PointF((realPoints[realPoints.Count - 2].X + realPoints[realPoints.Count - 1].X) / 2, (realPoints[realPoints.Count - 2].Y + realPoints[realPoints.Count - 1].Y) / 2));
                        realPoints.Add(point); // �������� ����� ������� � ����� ��� ���
                        //points.Add(point);

                        MyMatrix temp = new MyMatrix(2, 4, new List<float>()
                        { realPoints[realPoints.Count - 4].X, realPoints[realPoints.Count - 3].X, realPoints[realPoints.Count - 2].X, realPoints[realPoints.Count - 1].X,
                         realPoints[realPoints.Count - 4].Y, realPoints[realPoints.Count - 3].Y, realPoints[realPoints.Count - 2].Y, realPoints[realPoints.Count - 1].Y,});
                        matrixs.Add(temp * BezBukviKoef);
                        NotFinishedDots = 1;
                        break;
                    case 1:
                        realPoints[realPoints.Count - 2] = realPoints[realPoints.Count - 1];
                        realPoints[realPoints.Count - 1] = point;
                        //points.Add(point);
                        MyMatrix temp1 = new MyMatrix(2, 4, new List<float>()
                        { realPoints[realPoints.Count - 4].X, realPoints[realPoints.Count - 3].X, realPoints[realPoints.Count - 2].X, realPoints[realPoints.Count - 1].X,
                         realPoints[realPoints.Count - 4].Y, realPoints[realPoints.Count - 3].Y, realPoints[realPoints.Count - 2].Y, realPoints[realPoints.Count - 1].Y,});
                        matrixs[matrixs.Count - 1] = (temp1 * BezBukviKoef);
                        NotFinishedDots = 0;
                        break;
                }
            }
        }


        public int FindPoint(PointF point)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (Math.Sqrt(Math.Pow(Math.Abs(points[i].X - point.X), 2) + Math.Pow(Math.Abs(points[i].Y - point.Y), 2)) < _searchDistance)
                { return i; }
            }

            return -1;
        }


        private void Canvas_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
            PointF p = new PointF(mouseEventArgs.X, mouseEventArgs.Y);
            if (state == State.Task3Drawing)
            {

                AddDot(p);
            }
            if (state == State.Task3Delete)
            {
                int pos = FindPoint(p);
                if (pos != -1)
                {
                    points.RemoveAt(pos);
                    g.Clear(Color.White);
                    RecalculateAllMatrix();
                    DrawPoints();

                }
            }
            if (state == State.Task3Moving)
            {
                if (_selectedDot == -1)
                {
                    int pos = FindPoint(p);
                    if (pos != -1)
                    {
                        _selectedDot = pos;
                        g.DrawRectangle(_penGreen, points[pos].X, points[pos].Y, 1, 1);

                    }
                }
                else
                {
                    points[_selectedDot] = p;
                    UpdateMatrixWithDOt(_selectedDot);
                    _selectedDot = -1;
                    g.Clear(Color.White);
                    DrawPoints();
                }
            }
        }

        private void RecalculateAllMatrix()
        {
            realPoints.Clear();
            matrixs.Clear();

            if (points.Count == 0)
                return;
            if (points.Count == 1)
            {
                realPoints.Add(points[0]);
                realPoints.Add(points[0]);
                realPoints.Add(points[0]);
                realPoints.Add(points[0]);
                NotFinishedDots = 3;
            }
            else if (points.Count == 2)
            {
                realPoints.Add(points[0]);
                realPoints.Add(points[0]);
                realPoints.Add(points[1]);
                realPoints.Add(points[1]);
                NotFinishedDots = 2;
            }
            else if (points.Count == 3)
            {
                realPoints.Add(points[0]);
                realPoints.Add(points[1]);
                realPoints.Add(new PointF((points[1].X + points[2].X) / 2, (points[1].Y + points[2].Y) / 2));
                realPoints.Add(points[2]);
                NotFinishedDots = 1;
            }
            else if (points.Count == 4)
            {
                realPoints.Add(points[0]);
                realPoints.Add(points[1]);
                realPoints.Add(points[2]);
                realPoints.Add(points[3]);
                NotFinishedDots = 0;
            }
            else
            {
                realPoints.Add(points[0]);
                realPoints.Add(points[1]);
                realPoints.Add(points[2]);
                realPoints.Add(new PointF((points[2].X + points[3].X) / 2, (points[2].Y + points[3].Y) / 2));
                int i;
                for (i = 3; i < points.Count - 4; i += 2)
                {
                    realPoints.Add(points[i]);
                    realPoints.Add(points[i + 1]);
                    realPoints.Add(new PointF((points[i + 1].X + points[i + 2].X) / 2, (points[i + 1].Y + points[i + 2].Y) / 2));

                }

                if (i != points.Count - 1)
                {
                    switch (points.Count - i)
                    {
                        case 2:
                            realPoints.Add(points[i]);
                            realPoints.Add(new PointF((points[i].X + points[i + 1].X) / 2, (points[i].Y + points[i + 1].Y) / 2));
                            realPoints.Add(points[i + 1]);
                            NotFinishedDots = 1;
                            break;
                        case 3:

                            realPoints.Add(points[i]);
                            realPoints.Add(points[i + 1]);
                            realPoints.Add(points[i + 2]);
                            NotFinishedDots = 0;
                            break;
                        case 4:
                            realPoints.Add(points[i]);
                            realPoints.Add(points[i + 1]);
                            realPoints.Add(new PointF((points[i + 1].X + points[i + 2].X) / 2, (points[i + 1].Y + points[i + 2].Y) / 2));

                            realPoints.Add(points[i + 2]);
                            realPoints.Add(new PointF((points[i + 2].X + points[i + 3].X) / 2, (points[i + 2].Y + points[i + 3].Y) / 2));

                            realPoints.Add(points[i + 3]);
                            NotFinishedDots = 1;
                            break;

                    }
                }
            }
            int j = 0;
            for (j = 0; j < realPoints.Count - 4; j += 3)
            {
                MyMatrix temp = new MyMatrix(2, 4, new List<float>() { realPoints[j].X, realPoints[j+1].X, realPoints[j+2].X, realPoints[j+3].X,
                realPoints[j].Y, realPoints[j+1].Y, realPoints[j+2].Y, realPoints[j+3].Y,});
                matrixs.Add(temp * BezBukviKoef);
            }
            MyMatrix temp1 = new MyMatrix(2, 4, new List<float>() { realPoints[j].X, realPoints[j+1].X, realPoints[j+2].X, realPoints[j+3].X,
                realPoints[j].Y, realPoints[j+1].Y, realPoints[j+2].Y, realPoints[j+3].Y,});
            matrixs.Add(temp1 * BezBukviKoef);

        }

        private void DrawCurve()
        {
            g.Clear(Color.White);

            if (matrixs.Count == 0) { return; }

            foreach (var itMatr in matrixs)
            {
                float singleT = 0;
                for (int k = 0; k < 500; k++)
                {
                    MyMatrix t = new MyMatrix(4, 1, new List<float>() {(float)Math.Pow(singleT,0),
                    (float)Math.Pow(singleT,1),
                    (float)Math.Pow(singleT,2),
                    (float)Math.Pow(singleT,3)});
                    MyMatrix tempRes = itMatr * t;
                    g.DrawRectangle(_pen, tempRes[0, 0], tempRes[1, 0], 1, 1);
                    singleT += 1 / 500f;
                }
            }
        }

        private void DrawPoints()
        {
            foreach (var p in points)
            {
                g.DrawRectangle(_penRed, p.X, p.Y, 1, 1);
            }
        }

        private void UpdateMatrixWithDOt(int ind)
        {
            if (points.Count <= 4)
            {
                if (points.Count == 1)
                {
                    realPoints[0] = realPoints[1] = realPoints[2] = realPoints[3] = points[ind];
                }
                if (points.Count == 2)
                {
                    if (ind == 0)
                    {
                        realPoints[0] = realPoints[1] = points[ind];
                    }
                    else
                    {
                        realPoints[2] = realPoints[3] = points[ind];
                    }
                }
                if (points.Count == 3)
                {
                    if (ind == 0)
                    {
                        realPoints[0] = points[ind];

                    }
                    else
                    {
                        if (ind == 1)
                        {
                            realPoints[1] = points[ind];
                        }
                        else
                        {
                            realPoints[3] = points[ind];
                        }
                        realPoints[2] = new PointF((realPoints[1].X + realPoints[3].X) / 2, (realPoints[1].Y + realPoints[3].Y) / 2);
                    }
                }
                else
                {
                    realPoints[ind] = points[ind];
                }

                MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[0].X, realPoints[1].X, realPoints[2].X, realPoints[3].X,
                realPoints[0].Y, realPoints[1].Y, realPoints[2].Y, realPoints[3].Y,});
                matrixs[0] = t1 * BezBukviKoef;
            }
            else
            {
                if (ind <= 1)
                {
                    realPoints[ind] = points[ind];

                    MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[0].X, realPoints[1].X, realPoints[2].X, realPoints[3].X,
                realPoints[0].Y, realPoints[1].Y, realPoints[2].Y, realPoints[3].Y,});
                    matrixs[0] = t1 * BezBukviKoef;
                }
                else if (ind >= points.Count - 2)
                {
                    if (points.Count % 2 == 0)
                    {
                        int i = realPoints.Count - 4;
                        realPoints[realPoints.Count - (points.Count - ind)] = points[ind];
                        MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                        matrixs[matrixs.Count - 1] = t1 * BezBukviKoef;
                    }
                    else
                    {
                        if (points.Count - 1 == ind)
                        {
                            realPoints[realPoints.Count - 1] = points[ind];
                            int i = realPoints.Count - 4;
                            MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                            matrixs[matrixs.Count - 1] = t1 * BezBukviKoef;
                        }
                        else
                        {
                            int j = points.Count - 2;
                            realPoints[realPoints.Count - 3] = points[ind];
                            realPoints[realPoints.Count - 2] = new PointF((points[j].X + points[j + 1].X) / 2, (points[j].Y + points[j + 1].Y) / 2);
                            j = points.Count - 3;
                            realPoints[realPoints.Count - 4] = new PointF((points[j].X + points[j + 1].X) / 2, (points[j].Y + points[j + 1].Y) / 2);

                            int i = realPoints.Count - 4;
                            MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                            matrixs[matrixs.Count - 1] = t1 * BezBukviKoef;

                            i = realPoints.Count - 7;
                            t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                            matrixs[matrixs.Count - 2] = t1 * BezBukviKoef;
                        }
                    }
                }
                else
                {

                    int matrNumber = (ind / 2 - 1 + ind % 2);
                    int secondMatrNumber = 0;
                    int updateInd = matrNumber * 3;

                    if (ind % 2 == 0)// ������� ������� � �� ��� ������ ������ ���� ���������
                    {
                        updateInd += 2;
                        realPoints[updateInd] = points[ind];
                        realPoints[updateInd + 1] = new PointF((points[ind].X + points[ind + 1].X) / 2, (points[ind].Y + points[ind + 1].Y) / 2);
                        secondMatrNumber = matrNumber + 1;
                    }
                    else // ������� ������� � �� ��� ����� ������ ���� ���������
                    {
                        updateInd += 1;
                        realPoints[updateInd] = points[ind];
                        realPoints[updateInd - 1] = new PointF((points[ind].X + points[ind - 1].X) / 2, (points[ind].Y + points[ind - 1].Y) / 2);
                        secondMatrNumber = matrNumber - 1;
                    }

                    int i = secondMatrNumber * 3;
                    MyMatrix t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                    matrixs[secondMatrNumber] = t1 * BezBukviKoef;

                    i = matrNumber * 3;
                    t1 = new MyMatrix(2, 4, new List<float>() { realPoints[i].X, realPoints[i+1].X, realPoints[i+2].X, realPoints[i+3].X,
                        realPoints[i].Y, realPoints[i+1].Y, realPoints[i+2].Y, realPoints[i+3].Y});
                    matrixs[matrNumber] = t1 * BezBukviKoef;

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            state = State.Task3Line;
            DrawCurve();
            DrawPoints();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            state = State.Task3Delete;
            g.Clear(Color.White);
            DrawPoints();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            state = State.Task3Moving;
            _selectedDot = -1;
            g.Clear(Color.White);
            DrawPoints();
        }
    }

    enum State { NoTask, Task1, Task2, Task3Drawing, Task3Line, Task3Delete, Task3Moving }

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
                throw new Exception("������������ ������������ ������");

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