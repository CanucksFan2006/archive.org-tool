using System.Buffers.Text;
using System.Diagnostics;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

namespace Archive.org_Tools
{
    public partial class Menu : Form
    {
        private ThumbnailGrabber thumbnailGrabber = null;
        private KeywordSearcher keywordSearcher = null;
        string version = "1.2";
        HttpClient client = new HttpClient();
        Random random = new Random();
        public Menu()
        {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            TitleLabel.Location = new Point((this.ClientSize.Width - TitleLabel.Width) / 2, 10);
            getVersion();
        }
        private async void getVersion()
        {
            this.Name += $" v{version}";
            this.Text += $" v{version}";
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync("https://pastebin.com/raw/jWxsm4Mu");
            }
            catch
            {
                MessageBox.Show("Error fetching the current version number. Try connecting to the internet or waiting a few minutes before trying again.");
                return;
            }
            if (resp.IsSuccessStatusCode)
            {
                string version2 = await resp.Content.ReadAsStringAsync();
                if (version != version2)
                {
                    MessageBox.Show("This version is out of date. Download the newest version from the tutorial video on BigStarSecret2006's YouTube Channel.");
                    this.Close();
                }
                button1.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                MessageBox.Show("Error fetching the current version number. Try connecting to the internet or waiting a few minutes before trying again.");
                this.Close();
            }
            return;
        }
        private void MenuShown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        private void MenuSizeChanged(object sender, EventArgs e)
        {
            TitleLabel.Location = new Point((this.ClientSize.Width - TitleLabel.Width) / 2, 10);
        }
        private void Button1Click(object sender, EventArgs e)
        {
            if (thumbnailGrabber == null || thumbnailGrabber.IsDisposed)
            {
                thumbnailGrabber = new ThumbnailGrabber();
                thumbnailGrabber.Show();
            }
            else
            {
                thumbnailGrabber.BringToFront();
            }
        }
        private void Button2Click(object sender, EventArgs e)
        {
            if (keywordSearcher == null || keywordSearcher.IsDisposed)
            {
                keywordSearcher = new KeywordSearcher();
                keywordSearcher.Show();
            }
            else
            {
                keywordSearcher.BringToFront();
            }
        }
    }
}