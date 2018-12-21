namespace TekScanner
{
    partial class GridSizeForm
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
            this.gbSize = new System.Windows.Forms.GroupBox();
            this.nudCols = new System.Windows.Forms.NumericUpDown();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbShowGrid = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbGrid = new System.Windows.Forms.TabPage();
            this.pbGrid = new System.Windows.Forms.PictureBox();
            this.bSave = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.gbSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tbGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // gbSize
            // 
            this.gbSize.Controls.Add(this.nudCols);
            this.gbSize.Controls.Add(this.nudRows);
            this.gbSize.Controls.Add(this.label2);
            this.gbSize.Controls.Add(this.label1);
            this.gbSize.Location = new System.Drawing.Point(429, 93);
            this.gbSize.Name = "gbSize";
            this.gbSize.Size = new System.Drawing.Size(200, 121);
            this.gbSize.TabIndex = 24;
            this.gbSize.TabStop = false;
            this.gbSize.Text = "Grid Size";
            // 
            // nudCols
            // 
            this.nudCols.Location = new System.Drawing.Point(80, 66);
            this.nudCols.Name = "nudCols";
            this.nudCols.Size = new System.Drawing.Size(93, 26);
            this.nudCols.TabIndex = 3;
            this.nudCols.ValueChanged += new System.EventHandler(this.nudCols_ValueChanged);
            // 
            // nudRows
            // 
            this.nudRows.Location = new System.Drawing.Point(80, 33);
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(93, 26);
            this.nudRows.TabIndex = 2;
            this.nudRows.ValueChanged += new System.EventHandler(this.nudRows_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Cols:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rows:";
            // 
            // cbShowGrid
            // 
            this.cbShowGrid.AutoSize = true;
            this.cbShowGrid.Checked = true;
            this.cbShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowGrid.Location = new System.Drawing.Point(449, 49);
            this.cbShowGrid.Name = "cbShowGrid";
            this.cbShowGrid.Size = new System.Drawing.Size(98, 24);
            this.cbShowGrid.TabIndex = 23;
            this.cbShowGrid.Text = "Hide grid";
            this.cbShowGrid.UseVisualStyleBackColor = true;
            this.cbShowGrid.CheckedChanged += new System.EventHandler(this.cbShowGrid_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbGrid);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(411, 450);
            this.tabControl1.TabIndex = 22;
            // 
            // tbGrid
            // 
            this.tbGrid.Controls.Add(this.pbGrid);
            this.tbGrid.Location = new System.Drawing.Point(4, 29);
            this.tbGrid.Name = "tbGrid";
            this.tbGrid.Size = new System.Drawing.Size(403, 417);
            this.tbGrid.TabIndex = 5;
            this.tbGrid.Text = "Grid";
            this.tbGrid.UseVisualStyleBackColor = true;
            // 
            // pbGrid
            // 
            this.pbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbGrid.Location = new System.Drawing.Point(0, 0);
            this.pbGrid.Name = "pbGrid";
            this.pbGrid.Size = new System.Drawing.Size(403, 417);
            this.pbGrid.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGrid.TabIndex = 3;
            this.pbGrid.TabStop = false;
            // 
            // bSave
            // 
            this.bSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bSave.Location = new System.Drawing.Point(429, 391);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 47);
            this.bSave.TabIndex = 25;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(554, 391);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 47);
            this.bCancel.TabIndex = 26;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(494, 250);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(117, 26);
            this.textBox1.TabIndex = 27;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(494, 282);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(117, 26);
            this.textBox2.TabIndex = 28;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // GridSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 450);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.gbSize);
            this.Controls.Add(this.cbShowGrid);
            this.Controls.Add(this.tabControl1);
            this.Name = "GridSizeForm";
            this.Text = "GridSizeForm";
            this.gbSize.ResumeLayout(false);
            this.gbSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tbGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSize;
        private System.Windows.Forms.NumericUpDown nudCols;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbShowGrid;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbGrid;
        private System.Windows.Forms.PictureBox pbGrid;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}