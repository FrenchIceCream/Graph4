using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Graph4
{
    public partial class Form1 : Form
    {
        private const int max_depth = 5;
        private Pen pen_tree;
        public Form1()
        {
            InitializeComponent();
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
    }
}