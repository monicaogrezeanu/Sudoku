using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sudoku
{
    public partial class Form1 : Form
    {

        int n = 9, mistakes = 0, empty = 0;
        bool load =  false;
        private int[,] comp;
        private int[,] incomp;
        string diff = null, lev = null;
        Stopwatch stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add("easy");
            comboBox1.Items.Add("medium");
            comboBox1.Items.Add("hard");
            comboBox1.Items.Add("expert");

            comboBox2.Items.Add("1");
            comboBox2.Items.Add("2");
            comboBox2.Items.Add("3");
            comboBox2.Items.Add("4");
            comboBox2.Items.Add("5");


            label3.Text ="Mistakes: " + mistakes.ToString() + "/3";
            comp = new int[n, n];
            incomp = new int[n, n];

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)control;
                    textBox.TextChanged += textBox_TextChanged;
                    textBox.KeyPress += textBox_KeyPress;
                    textBox.Font = new Font(textBox.Font.FontFamily, 21);
                    textBox.Font = new Font("Arial", textBox.Font.Size);
                    textBox.TextAlign = HorizontalAlignment.Center;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 2);
            Graphics g = e.Graphics;
            g.DrawLine(pen, 297, 101, 297, 641);
            g.DrawLine(pen, 480, 101, 480, 641);
            g.DrawLine(pen, 118, 280, 659, 280);
            g.DrawLine(pen, 118, 463, 659, 463);
            pen.Dispose();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
            if (Char.IsDigit(e.KeyChar) && e.KeyChar >= '1' && e.KeyChar <= '9')
            {

                if (textBox.Text.Length == 1)
                {
                    textBox.Text = e.KeyChar.ToString();
                    e.Handled = true;
                }
            }
            else if (e.KeyChar == '\b')
            {
                textBox.Clear();
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (load == false)
            {
                System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender;
                string digits = ((System.Windows.Forms.TextBox)sender).Name.Substring(7);
                int i = int.Parse(digits.Substring(0, 1));
                int j = int.Parse(digits.Substring(1, 1));

                if (textBox.Text.Equals(comp[i - 1, j - 1].ToString()))
                {
                    textBox.BackColor = Color.LightCyan;
                    textBox.Enabled = false;
                    empty--;
                }
                else if (textBox.Text == "")
                { }
                else{
                    textBox.ForeColor = Color.Red;
                    mistakes++;
                    label3.Text = "Mistakes: " + mistakes.ToString() + "/3";
                }
                if (mistakes == 3)
                {
                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;
                    string formattedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                    string message = "You lost the game." +
                        "\r\n\r\nTime elapsed: " + formattedTime;
                    string title = "Game Over";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }
                if(empty == 0)
                {
                    stopwatch.Stop();
                    TimeSpan elapsed = stopwatch.Elapsed;
                    string formattedTime = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                    string message = "You completed the level." +
                        "\r\n\r\nTime elapsed: " + formattedTime;
                    string title = "Congratulations!";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load = true;
            empty = 0;
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                string message = "Select the difficulty and level.";
                string title = "Warning";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                diff = comboBox1.SelectedItem.ToString();
                lev = comboBox2.SelectedItem.ToString();

                int[] vector = new int[n * n];
                int[] vector2 = new int[n * n];

                string fileName = diff + lev + ".xml";
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                XmlSerializer serializer = new XmlSerializer(vector.GetType());
                FileStream fileStream = new FileStream(filePath, FileMode.Open);

                vector = (int[])serializer.Deserialize(fileStream);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        incomp[i, j] = vector[i * n + j];
                        string textBoxName = "textBox" + (i + 1).ToString() + (j + 1).ToString(); // construct the name of the TextBox control
                        Control[] matches = this.Controls.Find(textBoxName, true);

                        if (matches.Length > 0 && matches[0] is System.Windows.Forms.TextBox)
                        {
                            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)matches[0];
                            textBox.Text = incomp[i, j].ToString();
                            if (textBox.Text == "0")
                            {
                                textBox.Clear();
                                empty++;
                                textBox.Enabled = true;
                            }

                            else
                            {
                                textBox.Enabled = false;
                            }

                        }
                    }
                }
                fileStream.Close();

                string fileName2 = diff + "r" + lev + ".xml";
                string filePath2 = Path.Combine(Environment.CurrentDirectory, fileName2);
                FileStream fileStream2 = new FileStream(filePath2, FileMode.Open);

                vector2 = (int[])serializer.Deserialize(fileStream2);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        comp[i, j] = vector2[i * n + j];
                    }
                }
                fileStream2.Close();
                mistakes = 0;
                label3.Text = "Mistakes: " + mistakes.ToString() + "/3";
                load = false;
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "A standard Sudoku puzzle consists of a 9x9 grid," +
                " divided into nine 3x3 boxes.\r\n\r\n" +
                "The objective is to fill in each empty cell with a number " +
                "from 1 to 9, such that each row, column, and 3x3 box contains" +
                " all the numbers from 1 to 9 exactly once.\r\n\r\n" +
                "The puzzle typically starts with some cells already filled" +
                " in. These are called \"givens.\"\r\n\r\nThe givens must be" +
                " used as clues to help determine the rest of the solution. " +
                "You can't change the given numbers, and they must be included" +
                " in the final solution.\r\n\r\nEach row, column, and 3x3 box " +
                "must contain only one instance of each number. No number can " +
                "be repeated in the same row, column, or 3x3 box.\r\n\r\n" +
                "To solve a Sudoku puzzle, you need to use logical reasoning " +
                "and deduction, eliminating possibilities until only one number " +
                "is left for each cell.\r\n\r\nThere are different levels of " +
                "difficulty for Sudoku puzzles, ranging from easy to very difficult." +
                " The level of difficulty is determined by the number and placement " +
                "of the givens.";
            string title = "The rules of Sudoku";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
