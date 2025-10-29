namespace Archive.org_Tools
{
    partial class KeywordSearcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeywordSearcher));
            textBox1 = new TextBox();
            listBox1 = new ListBox();
            button1 = new Button();
            button2 = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox2 = new TextBox();
            label3 = new Label();
            textBox3 = new TextBox();
            label4 = new Label();
            listBox2 = new ListBox();
            label5 = new Label();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            label6 = new Label();
            listBox3 = new ListBox();
            label7 = new Label();
            button6 = new Button();
            listBox4 = new ListBox();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            button7 = new Button();
            label11 = new Label();
            label12 = new Label();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            label13 = new Label();
            checkBox1 = new CheckBox();
            label14 = new Label();
            label15 = new Label();
            listBox5 = new ListBox();
            label16 = new Label();
            listBox6 = new ListBox();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Arial", 9F);
            textBox1.Location = new Point(12, 27);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(262, 21);
            textBox1.TabIndex = 0;
            // 
            // listBox1
            // 
            listBox1.Font = new Font("Arial", 9F);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(12, 56);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(262, 79);
            listBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(12, 141);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += AddKeyword;
            // 
            // button2
            // 
            button2.Font = new Font("Arial", 9F);
            button2.Location = new Point(199, 141);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 4;
            button2.Text = "Remove";
            button2.UseVisualStyleBackColor = true;
            button2.Click += RemoveKeyword;
            // 
            // label1
            // 
            label1.Font = new Font("Arial", 9F, FontStyle.Bold);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(262, 15);
            label1.TabIndex = 5;
            label1.Text = "Add Keywords";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Font = new Font("Arial", 9F, FontStyle.Bold);
            label2.Location = new Point(280, 9);
            label2.Name = "label2";
            label2.Size = new Size(262, 15);
            label2.TabIndex = 6;
            label2.Text = "URL";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Arial", 9F);
            textBox2.Location = new Point(280, 27);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(262, 21);
            textBox2.TabIndex = 7;
            textBox2.KeyDown += GetPages;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(280, 160);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 8;
            label3.Text = "label3";
            label3.Visible = false;
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Arial", 9F);
            textBox3.Location = new Point(548, 27);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(262, 21);
            textBox3.TabIndex = 9;
            textBox3.TextChanged += ButtonTextEdit;
            textBox3.KeyDown += GetPageContent;
            // 
            // label4
            // 
            label4.Font = new Font("Arial", 9F, FontStyle.Bold);
            label4.Location = new Point(548, 9);
            label4.Name = "label4";
            label4.Size = new Size(262, 15);
            label4.TabIndex = 10;
            label4.Text = "Get Every Archived URL from Archive.org";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox2
            // 
            listBox2.Font = new Font("Arial", 9F);
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(12, 212);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(262, 79);
            listBox2.TabIndex = 11;
            // 
            // label5
            // 
            label5.Font = new Font("Arial", 9F, FontStyle.Bold);
            label5.Location = new Point(12, 194);
            label5.Name = "label5";
            label5.Size = new Size(262, 15);
            label5.TabIndex = 12;
            label5.Text = "CDX Page Statuses";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button3
            // 
            button3.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button3.Location = new Point(548, 54);
            button3.Name = "button3";
            button3.Size = new Size(262, 23);
            button3.TabIndex = 13;
            button3.Text = "Get All Pages";
            button3.UseVisualStyleBackColor = true;
            button3.Click += GetAllPagesThreadMaker;
            // 
            // button4
            // 
            button4.AutoSize = true;
            button4.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.Location = new Point(548, 83);
            button4.Name = "button4";
            button4.Size = new Size(262, 23);
            button4.TabIndex = 14;
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
            button4.Click += GetPageContentButton;
            // 
            // button5
            // 
            button5.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button5.Location = new Point(280, 54);
            button5.Name = "button5";
            button5.Size = new Size(262, 23);
            button5.TabIndex = 15;
            button5.Text = "Get Total Pages for URL";
            button5.UseVisualStyleBackColor = true;
            button5.Click += GetPagesButton;
            // 
            // label6
            // 
            label6.Font = new Font("Arial", 9F, FontStyle.Bold);
            label6.Location = new Point(12, 350);
            label6.Name = "label6";
            label6.Size = new Size(262, 15);
            label6.TabIndex = 17;
            label6.Text = "URL Page Statuses";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox3
            // 
            listBox3.Font = new Font("Arial", 9F);
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 15;
            listBox3.Location = new Point(12, 368);
            listBox3.Name = "listBox3";
            listBox3.Size = new Size(262, 79);
            listBox3.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 479);
            label7.Name = "label7";
            label7.Size = new Size(113, 15);
            label7.TabIndex = 18;
            label7.Text = "Pages Remaining: ";
            label7.Visible = false;
            // 
            // button6
            // 
            button6.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button6.Location = new Point(12, 453);
            button6.Name = "button6";
            button6.Size = new Size(262, 23);
            button6.TabIndex = 19;
            button6.Text = "Search for Keywords";
            button6.UseVisualStyleBackColor = true;
            button6.Visible = false;
            button6.Click += GetAllArchivedPagesThreadMaker;
            // 
            // listBox4
            // 
            listBox4.Font = new Font("Arial", 9F);
            listBox4.FormattingEnabled = true;
            listBox4.ItemHeight = 15;
            listBox4.Location = new Point(280, 368);
            listBox4.Name = "listBox4";
            listBox4.Size = new Size(262, 79);
            listBox4.TabIndex = 20;
            // 
            // label8
            // 
            label8.Font = new Font("Arial", 9F, FontStyle.Bold);
            label8.Location = new Point(280, 350);
            label8.Name = "label8";
            label8.Size = new Size(262, 15);
            label8.TabIndex = 21;
            label8.Text = "Matches";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 494);
            label9.Name = "label9";
            label9.Size = new Size(113, 15);
            label9.TabIndex = 22;
            label9.Text = "Pages Per Second:";
            label9.Visible = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(12, 509);
            label10.Name = "label10";
            label10.Size = new Size(102, 15);
            label10.TabIndex = 23;
            label10.Text = "Time Remaining:";
            label10.Visible = false;
            // 
            // button7
            // 
            button7.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button7.Location = new Point(93, 141);
            button7.Name = "button7";
            button7.Size = new Size(100, 23);
            button7.TabIndex = 24;
            button7.Text = "Default";
            button7.UseVisualStyleBackColor = true;
            button7.Click += AddDefaults;
            // 
            // label11
            // 
            label11.Location = new Point(280, 87);
            label11.Name = "label11";
            label11.Size = new Size(39, 21);
            label11.TabIndex = 25;
            label11.Text = "From:";
            label11.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            label12.Location = new Point(280, 114);
            label12.Name = "label12";
            label12.Size = new Size(39, 21);
            label12.TabIndex = 26;
            label12.Text = "To: ";
            label12.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(325, 87);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(185, 21);
            textBox4.TabIndex = 27;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(325, 114);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(185, 21);
            textBox5.TabIndex = 28;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label13.Location = new Point(280, 145);
            label13.Name = "label13";
            label13.Size = new Size(116, 15);
            label13.TabIndex = 29;
            label13.Text = "Match Subdomains:";
            // 
            // checkBox1
            // 
            checkBox1.Location = new Point(402, 146);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(15, 15);
            checkBox1.TabIndex = 30;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(12, 294);
            label14.Name = "label14";
            label14.Size = new Size(94, 15);
            label14.TabIndex = 31;
            label14.Text = "Items in Queue:";
            label14.Visible = false;
            // 
            // label15
            // 
            label15.Font = new Font("Arial", 9F, FontStyle.Bold);
            label15.Location = new Point(280, 194);
            label15.Name = "label15";
            label15.Size = new Size(262, 15);
            label15.TabIndex = 32;
            label15.Text = "CDX Crawls in Progress";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox5
            // 
            listBox5.Font = new Font("Arial", 9F);
            listBox5.FormattingEnabled = true;
            listBox5.ItemHeight = 15;
            listBox5.Location = new Point(280, 212);
            listBox5.Name = "listBox5";
            listBox5.Size = new Size(262, 79);
            listBox5.TabIndex = 33;
            listBox5.SelectedIndexChanged += insertCDX;
            // 
            // label16
            // 
            label16.Font = new Font("Arial", 9F, FontStyle.Bold);
            label16.Location = new Point(548, 194);
            label16.Name = "label16";
            label16.Size = new Size(262, 15);
            label16.TabIndex = 34;
            label16.Text = "URL Scrapes in Progress";
            label16.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listBox6
            // 
            listBox6.Font = new Font("Arial", 9F);
            listBox6.FormattingEnabled = true;
            listBox6.ItemHeight = 15;
            listBox6.Location = new Point(548, 212);
            listBox6.Name = "listBox6";
            listBox6.Size = new Size(262, 79);
            listBox6.TabIndex = 35;
            listBox6.SelectedIndexChanged += prepareKeywordScrape;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(280, 457);
            label17.Name = "label17";
            label17.Size = new Size(85, 15);
            label17.TabIndex = 36;
            label17.Text = "Total Matches:";
            label17.Visible = false;
            // 
            // label18
            // 
            label18.Location = new Point(280, 294);
            label18.Name = "label18";
            label18.Size = new Size(262, 15);
            label18.TabIndex = 37;
            label18.Text = "Select a domain to get its remaining URLs";
            label18.TextAlign = ContentAlignment.TopCenter;
            // 
            // label19
            // 
            label19.Location = new Point(548, 294);
            label19.Name = "label19";
            label19.Size = new Size(262, 15);
            label19.TabIndex = 38;
            label19.Text = "Select a domain to continue scraping its URLs";
            label19.TextAlign = ContentAlignment.TopCenter;
            // 
            // KeywordSearcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 541);
            Controls.Add(label19);
            Controls.Add(label18);
            Controls.Add(label17);
            Controls.Add(listBox6);
            Controls.Add(label16);
            Controls.Add(listBox5);
            Controls.Add(label15);
            Controls.Add(label14);
            Controls.Add(checkBox1);
            Controls.Add(label13);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(button7);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(listBox4);
            Controls.Add(button6);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(listBox3);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label5);
            Controls.Add(listBox2);
            Controls.Add(label4);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Controls.Add(textBox1);
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "KeywordSearcher";
            Text = "Keyword Searcher";
            FormClosing += ClosingForm;
            Shown += KeywordShown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private ListBox listBox1;
        private Button button1;
        private Button button2;
        private Label label1;
        private Label label2;
        private TextBox textBox2;
        private Label label3;
        private TextBox textBox3;
        private Label label4;
        private ListBox listBox2;
        private Label label5;
        private Button button3;
        private Button button4;
        private Button button5;
        private Label label6;
        private ListBox listBox3;
        private Label label7;
        private Button button6;
        private ListBox listBox4;
        private Label label8;
        private Label label9;
        private Label label10;
        private Button button7;
        private Label label11;
        private Label label12;
        private TextBox textBox4;
        private TextBox textBox5;
        private Label label13;
        private CheckBox checkBox1;
        private Label label14;
        private Label label15;
        private ListBox listBox5;
        private Label label16;
        private ListBox listBox6;
        private Label label17;
        private Label label18;
        private Label label19;
    }
}