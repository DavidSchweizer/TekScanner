namespace Tek1
{
    partial class PlayForm
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
            this.components = new System.ComponentModel.Container();
            this.ofd1 = new System.Windows.Forms.OpenFileDialog();
            this.sfd1 = new System.Windows.Forms.SaveFileDialog();
            this.ttSolve = new System.Windows.Forms.ToolTip(this.components);
            this.bCheck = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.bLoad = new System.Windows.Forms.Button();
            this.bSolve = new System.Windows.Forms.Button();
            this.bReset = new System.Windows.Forms.Button();
            this.split = new System.Windows.Forms.SplitContainer();
            this.playPanel1 = new Tek1.PlayPanel();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofd1
            // 
            this.ofd1.DefaultExt = "tx";
            this.ofd1.DereferenceLinks = false;
            this.ofd1.FileName = "9x7-1.tx";
            this.ofd1.Filter = "tx files (*.tx)|*.tx|All files (*.*)|*.*";
            this.ofd1.Title = "Open TEK file";
            // 
            // sfd1
            // 
            this.sfd1.CreatePrompt = true;
            this.sfd1.DefaultExt = "tx";
            // 
            // bCheck
            // 
            this.bCheck.BackgroundImage = global::Tek1.Properties.Resources.check;
            this.bCheck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bCheck.Location = new System.Drawing.Point(16, 375);
            this.bCheck.Name = "bCheck";
            this.bCheck.Size = new System.Drawing.Size(54, 54);
            this.bCheck.TabIndex = 16;
            this.ttSolve.SetToolTip(this.bCheck, "(Un)Check errors");
            this.bCheck.UseVisualStyleBackColor = true;
            this.bCheck.Click += new System.EventHandler(this.bCheck_Click);
            // 
            // bSave
            // 
            this.bSave.BackgroundImage = global::Tek1.Properties.Resources.save_new;
            this.bSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bSave.Location = new System.Drawing.Point(158, 382);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(44, 43);
            this.bSave.TabIndex = 6;
            this.bSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ttSolve.SetToolTip(this.bSave, "Save the current state to a file");
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // bLoad
            // 
            this.bLoad.BackgroundImage = global::Tek1.Properties.Resources.open_new;
            this.bLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLoad.Location = new System.Drawing.Point(108, 382);
            this.bLoad.Name = "bLoad";
            this.bLoad.Size = new System.Drawing.Size(44, 43);
            this.bLoad.TabIndex = 0;
            this.bLoad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ttSolve.SetToolTip(this.bLoad, "Load a new puzzle from a file");
            this.bLoad.UseVisualStyleBackColor = true;
            this.bLoad.Click += new System.EventHandler(this.bLoad_Click);
            // 
            // bSolve
            // 
            this.bSolve.BackgroundImage = global::Tek1.Properties.Resources.solve;
            this.bSolve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bSolve.Location = new System.Drawing.Point(16, 435);
            this.bSolve.Name = "bSolve";
            this.bSolve.Size = new System.Drawing.Size(54, 54);
            this.bSolve.TabIndex = 7;
            this.ttSolve.SetToolTip(this.bSolve, "Solve the puzzle");
            this.bSolve.UseVisualStyleBackColor = true;
            this.bSolve.Click += new System.EventHandler(this.bSolveClick);
            // 
            // bReset
            // 
            this.bReset.BackgroundImage = global::Tek1.Properties.Resources.reset;
            this.bReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bReset.Location = new System.Drawing.Point(110, 435);
            this.bReset.Name = "bReset";
            this.bReset.Size = new System.Drawing.Size(54, 54);
            this.bReset.TabIndex = 8;
            this.ttSolve.SetToolTip(this.bReset, "Reset the puzzle");
            this.bReset.UseCompatibleTextRendering = true;
            this.bReset.UseVisualStyleBackColor = true;
            this.bReset.Click += new System.EventHandler(this.bReset_Click);
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.SizeChanged += new System.EventHandler(this.panel1_Resize);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.playPanel1);
            this.split.Panel2.Controls.Add(this.bCheck);
            this.split.Panel2.Controls.Add(this.bSave);
            this.split.Panel2.Controls.Add(this.bLoad);
            this.split.Panel2.Controls.Add(this.bSolve);
            this.split.Panel2.Controls.Add(this.bReset);
            this.split.Size = new System.Drawing.Size(946, 552);
            this.split.SplitterDistance = 614;
            this.split.TabIndex = 10;
            // 
            // playPanel1
            // 
            this.playPanel1.Location = new System.Drawing.Point(3, 3);
            this.playPanel1.Name = "playPanel1";
            this.playPanel1.Size = new System.Drawing.Size(300, 321);
            this.playPanel1.TabIndex = 17;
            this.playPanel1.View = null;
            
            // 
            // PlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 552);
            this.Controls.Add(this.split);
            this.KeyPreview = true;
            this.Name = "PlayForm";
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bLoad;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.SaveFileDialog sfd1;
        private System.Windows.Forms.Button bSolve;
        private System.Windows.Forms.Button bReset;
        private System.Windows.Forms.ToolTip ttSolve;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.OpenFileDialog ofd1;
        private System.Windows.Forms.Button bCheck;
        private PlayPanel playPanel1;
    }
}