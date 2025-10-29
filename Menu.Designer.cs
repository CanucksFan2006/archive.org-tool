namespace Archive.org_Tools
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            TitleLabel = new Label();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // TitleLabel
            // 
            TitleLabel.AutoSize = true;
            TitleLabel.Font = new Font("Arial", 52F, FontStyle.Bold);
            TitleLabel.Location = new Point(102, 9);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new Size(622, 81);
            TitleLabel.TabIndex = 0;
            TitleLabel.Text = "Archive.org Tools";
            TitleLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // button1
            // 
            button1.Font = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(12, 91);
            button1.Name = "button1";
            button1.Size = new Size(182, 23);
            button1.TabIndex = 1;
            button1.Text = "Youtube Thumbnail Grabber";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1Click;
            // 
            // button2
            // 
            button2.Font = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(12, 120);
            button2.Name = "button2";
            button2.Size = new Size(182, 23);
            button2.TabIndex = 2;
            button2.Text = "Keyword Searcher";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 541);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(TitleLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Menu";
            Text = "Archive.org Tools";
            Shown += MenuShown;
            Resize += MenuSizeChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TitleLabel;
        private Button button1;
        private Button button2;
    }
}
