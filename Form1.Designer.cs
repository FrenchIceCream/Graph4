namespace Graph4
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Canvas = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            slider = new TrackBar();
            is_random = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)slider).BeginInit();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.BackColor = SystemColors.Window;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(795, 535);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            Canvas.Click += Canvas_Click;
            // 
            // button1
            // 
            button1.Location = new Point(821, 24);
            button1.Name = "button1";
            button1.Size = new Size(127, 39);
            button1.TabIndex = 1;
            button1.Text = "Задание 1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(821, 171);
            button2.Name = "button2";
            button2.Size = new Size(127, 39);
            button2.TabIndex = 2;
            button2.Text = "Задание 2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(821, 229);
            button3.Name = "button3";
            button3.Size = new Size(127, 39);
            button3.TabIndex = 3;
            button3.Text = "Задание 3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(821, 288);
            button4.Name = "button4";
            button4.Size = new Size(127, 39);
            button4.TabIndex = 4;
            button4.Text = "Рисовать";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(821, 332);
            button5.Name = "button5";
            button5.Size = new Size(127, 39);
            button5.TabIndex = 5;
            button5.Text = "Удалить";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(821, 376);
            button6.Name = "button6";
            button6.Size = new Size(127, 39);
            button6.TabIndex = 6;
            button6.Text = "Двинуть";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // slider
            // 
            slider.Location = new Point(821, 69);
            slider.Name = "slider";
            slider.Size = new Size(127, 56);
            slider.TabIndex = 7;
            // 
            // is_random
            // 
            is_random.AutoSize = true;
            is_random.Location = new Point(838, 117);
            is_random.Name = "is_random";
            is_random.Size = new Size(90, 24);
            is_random.TabIndex = 8;
            is_random.Text = "random?";
            is_random.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 533);
            Controls.Add(is_random);
            Controls.Add(slider);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(Canvas);
            Name = "Form1";
            Text = "Графика. Лабораторная 5";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            ((System.ComponentModel.ISupportInitialize)slider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Canvas;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private TrackBar slider;
        private CheckBox is_random;
    }
}