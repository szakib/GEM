namespace GEM
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            this.stateLabel2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.experimentIDLabel = new System.Windows.Forms.Label();
            this.popSizeLabel = new System.Windows.Forms.Label();
            this.generationLabel = new System.Windows.Forms.Label();
            this.fitnessLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.resetButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.goodPopFitnessLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.badPopFitnessLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(109, 12);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(12, 58);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(21, 13);
            this.stateLabel.TabIndex = 2;
            this.stateLabel.Text = "Init";
            this.stateLabel.Click += new System.EventHandler(this.stateLabel_Click);
            // 
            // stateLabel2
            // 
            this.stateLabel2.AutoSize = true;
            this.stateLabel2.Location = new System.Drawing.Point(12, 95);
            this.stateLabel2.Name = "stateLabel2";
            this.stateLabel2.Size = new System.Drawing.Size(0, 13);
            this.stateLabel2.TabIndex = 3;
            this.stateLabel2.Click += new System.EventHandler(this.stateLabel2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(317, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Current Generation:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(317, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Experiment ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(317, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Population Size:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(317, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Overall Fitness:";
            // 
            // experimentIDLabel
            // 
            this.experimentIDLabel.AutoSize = true;
            this.experimentIDLabel.Location = new System.Drawing.Point(431, 22);
            this.experimentIDLabel.Name = "experimentIDLabel";
            this.experimentIDLabel.Size = new System.Drawing.Size(0, 13);
            this.experimentIDLabel.TabIndex = 8;
            // 
            // popSizeLabel
            // 
            this.popSizeLabel.AutoSize = true;
            this.popSizeLabel.Location = new System.Drawing.Point(431, 53);
            this.popSizeLabel.Name = "popSizeLabel";
            this.popSizeLabel.Size = new System.Drawing.Size(0, 13);
            this.popSizeLabel.TabIndex = 9;
            // 
            // generationLabel
            // 
            this.generationLabel.AutoSize = true;
            this.generationLabel.Location = new System.Drawing.Point(431, 84);
            this.generationLabel.Name = "generationLabel";
            this.generationLabel.Size = new System.Drawing.Size(0, 13);
            this.generationLabel.TabIndex = 10;
            // 
            // fitnessLabel
            // 
            this.fitnessLabel.AutoSize = true;
            this.fitnessLabel.Location = new System.Drawing.Point(431, 115);
            this.fitnessLabel.Name = "fitnessLabel";
            this.fitnessLabel.Size = new System.Drawing.Size(0, 13);
            this.fitnessLabel.TabIndex = 11;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 130);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(203, 23);
            this.progressBar.TabIndex = 12;
            this.progressBar.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(209, 12);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 13;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(317, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Good Pop. Fitness:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // goodPopFitnessLabel
            // 
            this.goodPopFitnessLabel.AutoSize = true;
            this.goodPopFitnessLabel.Location = new System.Drawing.Point(431, 146);
            this.goodPopFitnessLabel.Name = "goodPopFitnessLabel";
            this.goodPopFitnessLabel.Size = new System.Drawing.Size(0, 13);
            this.goodPopFitnessLabel.TabIndex = 15;
            this.goodPopFitnessLabel.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(317, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Bad Pop. Fitness:";
            // 
            // badPopFitnessLabel
            // 
            this.badPopFitnessLabel.AutoSize = true;
            this.badPopFitnessLabel.Location = new System.Drawing.Point(431, 177);
            this.badPopFitnessLabel.Name = "badPopFitnessLabel";
            this.badPopFitnessLabel.Size = new System.Drawing.Size(0, 13);
            this.badPopFitnessLabel.TabIndex = 17;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 211);
            this.Controls.Add(this.badPopFitnessLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.goodPopFitnessLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.fitnessLabel);
            this.Controls.Add(this.generationLabel);
            this.Controls.Add(this.popSizeLabel);
            this.Controls.Add(this.experimentIDLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stateLabel2);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Name = "MainForm";
            this.Text = "Genetic Evaluator for Meta-Learning";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.Label stateLabel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label experimentIDLabel;
        private System.Windows.Forms.Label popSizeLabel;
        private System.Windows.Forms.Label generationLabel;
        private System.Windows.Forms.Label fitnessLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label goodPopFitnessLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label badPopFitnessLabel;
    }
}

