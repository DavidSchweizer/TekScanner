namespace Tek1
{
    partial class ConfigurationForm
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
            this.clbHeuristics = new System.Windows.Forms.CheckedListBox();
            this.bOK = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clbHeuristics
            // 
            this.clbHeuristics.FormattingEnabled = true;
            this.clbHeuristics.Location = new System.Drawing.Point(8, 5);
            this.clbHeuristics.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.clbHeuristics.Name = "clbHeuristics";
            this.clbHeuristics.Size = new System.Drawing.Size(291, 349);
            this.clbHeuristics.TabIndex = 0;
            this.clbHeuristics.SelectedIndexChanged += new System.EventHandler(this.clbHeuristics_SelectedIndexChanged);
            // 
            // bOK
            // 
            this.bOK.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(324, 327);
            this.bOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(50, 27);
            this.bOK.TabIndex = 12;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // bDown
            // 
            this.bDown.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            //this.bDown.BackgroundImage = global::Tek1.Properties.Resources.down;
            this.bDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bDown.Location = new System.Drawing.Point(303, 76);
            this.bDown.Margin = new System.Windows.Forms.Padding(2);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(50, 27);
            this.bDown.TabIndex = 14;
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Click += new System.EventHandler(this.button2_Click);
            // 
            // bUp
            // 
            this.bUp.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            //this.bUp.BackgroundImage = global::Tek1.Properties.Resources.up;
            this.bUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bUp.Location = new System.Drawing.Point(303, 45);
            this.bUp.Margin = new System.Windows.Forms.Padding(2);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(50, 27);
            this.bUp.TabIndex = 13;
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 387);
            this.Controls.Add(this.bDown);
            this.Controls.Add(this.bUp);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.clbHeuristics);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ConfigurationForm";
            this.Text = "ConfigurationForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbHeuristics;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bDown;
    }
}