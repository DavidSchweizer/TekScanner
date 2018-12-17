namespace TekScanner
{
    partial class ScanningForm
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
            this.ofd1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbOriginal = new System.Windows.Forms.TabPage();
            this.pbOriginal = new System.Windows.Forms.PictureBox();
            this.tbControls = new System.Windows.Forms.TabPage();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbGrey = new System.Windows.Forms.TabPage();
            this.pbGray = new System.Windows.Forms.PictureBox();
            this.tbEdges = new System.Windows.Forms.TabPage();
            this.pbEdges = new System.Windows.Forms.PictureBox();
            this.tbLines = new System.Windows.Forms.TabPage();
            this.pbLines = new System.Windows.Forms.PictureBox();
            this.tbAnalyzed = new System.Windows.Forms.TabPage();
            this.pbAnalyzed = new System.Windows.Forms.PictureBox();
            this.tbGrid = new System.Windows.Forms.TabPage();
            this.pbGrid = new System.Windows.Forms.PictureBox();
            this.tbCombi = new System.Windows.Forms.TabPage();
            this.pbCombi = new System.Windows.Forms.PictureBox();
            this.tbOCR = new System.Windows.Forms.TabPage();
            this.pbOCR = new System.Windows.Forms.PictureBox();
            this.bBoard = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.bLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbOriginal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOriginal)).BeginInit();
            this.tbControls.SuspendLayout();
            this.tbGrey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGray)).BeginInit();
            this.tbEdges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbEdges)).BeginInit();
            this.tbLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLines)).BeginInit();
            this.tbAnalyzed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnalyzed)).BeginInit();
            this.tbGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).BeginInit();
            this.tbCombi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCombi)).BeginInit();
            this.tbOCR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOCR)).BeginInit();
            this.SuspendLayout();
            // 
            // ofd1
            // 
            this.ofd1.FileName = "test1.jpg";
            this.ofd1.Filter = "Common Graphics files|*.jpg;*.bmp;*.png|all files|*.*";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.bBoard);
            this.splitContainer1.Panel2.Controls.Add(this.button2);
            this.splitContainer1.Panel2.Controls.Add(this.bLoad);
            this.splitContainer1.Size = new System.Drawing.Size(586, 665);
            this.splitContainer1.SplitterDistance = 565;
            this.splitContainer1.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbOriginal);
            this.tabControl1.Controls.Add(this.tbControls);
            this.tabControl1.Controls.Add(this.tbGrey);
            this.tabControl1.Controls.Add(this.tbEdges);
            this.tabControl1.Controls.Add(this.tbLines);
            this.tabControl1.Controls.Add(this.tbAnalyzed);
            this.tabControl1.Controls.Add(this.tbGrid);
            this.tabControl1.Controls.Add(this.tbCombi);
            this.tabControl1.Controls.Add(this.tbOCR);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(586, 565);
            this.tabControl1.TabIndex = 19;
            // 
            // tbOriginal
            // 
            this.tbOriginal.Controls.Add(this.pbOriginal);
            this.tbOriginal.Location = new System.Drawing.Point(4, 29);
            this.tbOriginal.Name = "tbOriginal";
            this.tbOriginal.Padding = new System.Windows.Forms.Padding(3);
            this.tbOriginal.Size = new System.Drawing.Size(578, 532);
            this.tbOriginal.TabIndex = 0;
            this.tbOriginal.Text = "Original";
            this.tbOriginal.UseVisualStyleBackColor = true;
            this.tbOriginal.Click += new System.EventHandler(this.tbOriginal_Click);
            // 
            // pbOriginal
            // 
            this.pbOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbOriginal.Location = new System.Drawing.Point(3, 3);
            this.pbOriginal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbOriginal.Name = "pbOriginal";
            this.pbOriginal.Size = new System.Drawing.Size(572, 526);
            this.pbOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbOriginal.TabIndex = 3;
            this.pbOriginal.TabStop = false;
            // 
            // tbControls
            // 
            this.tbControls.Controls.Add(this.textBox5);
            this.tbControls.Controls.Add(this.label5);
            this.tbControls.Controls.Add(this.textBox4);
            this.tbControls.Controls.Add(this.label4);
            this.tbControls.Controls.Add(this.textBox3);
            this.tbControls.Controls.Add(this.label3);
            this.tbControls.Controls.Add(this.textBox2);
            this.tbControls.Controls.Add(this.label2);
            this.tbControls.Controls.Add(this.textBox1);
            this.tbControls.Controls.Add(this.label1);
            this.tbControls.Location = new System.Drawing.Point(4, 29);
            this.tbControls.Name = "tbControls";
            this.tbControls.Size = new System.Drawing.Size(578, 532);
            this.tbControls.TabIndex = 4;
            this.tbControls.Text = "processing controls";
            this.tbControls.UseVisualStyleBackColor = true;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(176, 158);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(96, 26);
            this.textBox5.TabIndex = 27;
            this.textBox5.Text = "2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 162);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 20);
            this.label5.TabIndex = 26;
            this.label5.Text = "gap";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(176, 122);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(96, 26);
            this.textBox4.TabIndex = 25;
            this.textBox4.Text = "3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 122);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 20);
            this.label4.TabIndex = 24;
            this.label4.Text = "minLineWidth";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(176, 88);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(96, 26);
            this.textBox3.TabIndex = 23;
            this.textBox3.Text = "3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "treshold";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(212, 48);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(96, 26);
            this.textBox2.TabIndex = 21;
            this.textBox2.Text = "120";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 20);
            this.label2.TabIndex = 20;
            this.label2.Text = "CannyTresholdLinking";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(212, 18);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(96, 26);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "180";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "CannyTreshold:";
            // 
            // tbGrey
            // 
            this.tbGrey.Controls.Add(this.pbGray);
            this.tbGrey.Location = new System.Drawing.Point(4, 29);
            this.tbGrey.Name = "tbGrey";
            this.tbGrey.Padding = new System.Windows.Forms.Padding(3);
            this.tbGrey.Size = new System.Drawing.Size(578, 532);
            this.tbGrey.TabIndex = 1;
            this.tbGrey.Text = "Greyscaled";
            this.tbGrey.UseVisualStyleBackColor = true;
            // 
            // pbGray
            // 
            this.pbGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGray.Location = new System.Drawing.Point(3, 3);
            this.pbGray.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbGray.Name = "pbGray";
            this.pbGray.Size = new System.Drawing.Size(572, 526);
            this.pbGray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGray.TabIndex = 1;
            this.pbGray.TabStop = false;
            // 
            // tbEdges
            // 
            this.tbEdges.Controls.Add(this.pbEdges);
            this.tbEdges.Location = new System.Drawing.Point(4, 29);
            this.tbEdges.Name = "tbEdges";
            this.tbEdges.Size = new System.Drawing.Size(578, 532);
            this.tbEdges.TabIndex = 2;
            this.tbEdges.Text = "Edges";
            this.tbEdges.UseVisualStyleBackColor = true;
            // 
            // pbEdges
            // 
            this.pbEdges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbEdges.Location = new System.Drawing.Point(0, 0);
            this.pbEdges.Name = "pbEdges";
            this.pbEdges.Size = new System.Drawing.Size(578, 532);
            this.pbEdges.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbEdges.TabIndex = 0;
            this.pbEdges.TabStop = false;
            // 
            // tbLines
            // 
            this.tbLines.Controls.Add(this.pbLines);
            this.tbLines.Location = new System.Drawing.Point(4, 29);
            this.tbLines.Name = "tbLines";
            this.tbLines.Size = new System.Drawing.Size(578, 532);
            this.tbLines.TabIndex = 3;
            this.tbLines.Text = "Lines";
            this.tbLines.UseVisualStyleBackColor = true;
            // 
            // pbLines
            // 
            this.pbLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLines.Location = new System.Drawing.Point(0, 0);
            this.pbLines.Name = "pbLines";
            this.pbLines.Size = new System.Drawing.Size(578, 532);
            this.pbLines.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLines.TabIndex = 1;
            this.pbLines.TabStop = false;
            // 
            // tbAnalyzed
            // 
            this.tbAnalyzed.Controls.Add(this.pbAnalyzed);
            this.tbAnalyzed.Location = new System.Drawing.Point(4, 29);
            this.tbAnalyzed.Name = "tbAnalyzed";
            this.tbAnalyzed.Padding = new System.Windows.Forms.Padding(3);
            this.tbAnalyzed.Size = new System.Drawing.Size(578, 532);
            this.tbAnalyzed.TabIndex = 9;
            this.tbAnalyzed.Text = "Analyzed";
            this.tbAnalyzed.UseVisualStyleBackColor = true;
            // 
            // pbAnalyzed
            // 
            this.pbAnalyzed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbAnalyzed.Location = new System.Drawing.Point(3, 3);
            this.pbAnalyzed.Name = "pbAnalyzed";
            this.pbAnalyzed.Size = new System.Drawing.Size(572, 526);
            this.pbAnalyzed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbAnalyzed.TabIndex = 1;
            this.pbAnalyzed.TabStop = false;
            // 
            // tbGrid
            // 
            this.tbGrid.Controls.Add(this.pbGrid);
            this.tbGrid.Location = new System.Drawing.Point(4, 29);
            this.tbGrid.Name = "tbGrid";
            this.tbGrid.Size = new System.Drawing.Size(578, 532);
            this.tbGrid.TabIndex = 5;
            this.tbGrid.Text = "Grid";
            this.tbGrid.UseVisualStyleBackColor = true;
            // 
            // pbGrid
            // 
            this.pbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGrid.Location = new System.Drawing.Point(0, 0);
            this.pbGrid.Name = "pbGrid";
            this.pbGrid.Size = new System.Drawing.Size(578, 532);
            this.pbGrid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGrid.TabIndex = 3;
            this.pbGrid.TabStop = false;
            // 
            // tbCombi
            // 
            this.tbCombi.Controls.Add(this.pbCombi);
            this.tbCombi.Location = new System.Drawing.Point(4, 29);
            this.tbCombi.Name = "tbCombi";
            this.tbCombi.Size = new System.Drawing.Size(578, 532);
            this.tbCombi.TabIndex = 7;
            this.tbCombi.Text = "image+grid";
            this.tbCombi.UseVisualStyleBackColor = true;
            // 
            // pbCombi
            // 
            this.pbCombi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbCombi.Location = new System.Drawing.Point(0, 0);
            this.pbCombi.Name = "pbCombi";
            this.pbCombi.Size = new System.Drawing.Size(578, 532);
            this.pbCombi.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCombi.TabIndex = 4;
            this.pbCombi.TabStop = false;
            // 
            // tbOCR
            // 
            this.tbOCR.Controls.Add(this.pbOCR);
            this.tbOCR.Location = new System.Drawing.Point(4, 29);
            this.tbOCR.Name = "tbOCR";
            this.tbOCR.Size = new System.Drawing.Size(578, 532);
            this.tbOCR.TabIndex = 8;
            this.tbOCR.Text = "OCR";
            this.tbOCR.UseVisualStyleBackColor = true;
            // 
            // pbOCR
            // 
            this.pbOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbOCR.Location = new System.Drawing.Point(0, 0);
            this.pbOCR.Name = "pbOCR";
            this.pbOCR.Size = new System.Drawing.Size(578, 532);
            this.pbOCR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbOCR.TabIndex = 6;
            this.pbOCR.TabStop = false;
            // 
            // bBoard
            // 
            this.bBoard.Location = new System.Drawing.Point(369, 20);
            this.bBoard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bBoard.Name = "bBoard";
            this.bBoard.Size = new System.Drawing.Size(112, 35);
            this.bBoard.TabIndex = 21;
            this.bBoard.Text = "Create puzzle";
            this.bBoard.UseVisualStyleBackColor = true;
            this.bBoard.Click += new System.EventHandler(this.bBoard_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(210, 20);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 35);
            this.button2.TabIndex = 20;
            this.button2.Text = "Find Grid";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // bLoad
            // 
            this.bLoad.Location = new System.Drawing.Point(25, 20);
            this.bLoad.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(126, 56);
            this.bLoad.TabIndex = 19;
            this.bLoad.Text = "Load";
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.button3_Click);
            // 
            // ScanningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 665);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScanningForm";
            this.Text = "Scan puzzle file";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tbOriginal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOriginal)).EndInit();
            this.tbControls.ResumeLayout(false);
            this.tbControls.PerformLayout();
            this.tbGrey.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGray)).EndInit();
            this.tbEdges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbEdges)).EndInit();
            this.tbLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLines)).EndInit();
            this.tbAnalyzed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbAnalyzed)).EndInit();
            this.tbGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).EndInit();
            this.tbCombi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbCombi)).EndInit();
            this.tbOCR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOCR)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofd1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbOriginal;
        private System.Windows.Forms.PictureBox pbOriginal;
        private System.Windows.Forms.TabPage tbGrey;
        private System.Windows.Forms.PictureBox pbGray;
        private System.Windows.Forms.TabPage tbEdges;
        private System.Windows.Forms.PictureBox pbEdges;
        private System.Windows.Forms.TabPage tbLines;
        private System.Windows.Forms.PictureBox pbLines;
        private System.Windows.Forms.TabPage tbControls;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tbGrid;
        private System.Windows.Forms.PictureBox pbGrid;
        private System.Windows.Forms.TabPage tbCombi;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.PictureBox pbCombi;
        private System.Windows.Forms.TabPage tbOCR;
        private System.Windows.Forms.PictureBox pbOCR;
        private System.Windows.Forms.TabPage tbAnalyzed;
        private System.Windows.Forms.PictureBox pbAnalyzed;
        private System.Windows.Forms.Button bBoard;
    }
}

