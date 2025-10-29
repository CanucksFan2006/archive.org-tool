using System;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.Unicode;
using System.Windows.Forms.VisualStyles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using System.Security.Cryptography;

namespace Archive.org_Tools
{
    public partial class ThumbnailGrabber : Form
    {
        ConcurrentQueue<string> URLList = new ConcurrentQueue<string>();
        HttpClient client = new HttpClient();
        ConcurrentQueue<string[]> subdomainQueue = new ConcurrentQueue<string[]>();
        string[][] subdomains = [["i", ""], ["i1", ""], ["i2", ""], ["i3", ""], ["i4", ""], ["img", ""], ["i", "_webp"], ["i1", "_webp"], ["i2", "_webp"], ["i3", "_webp"], ["i4", "_webp"], ["img", "_webp"], ["imgyt",""], ["imgyt","_webp"]];
        Dictionary<Bitmap,string> bitmaps = new Dictionary<Bitmap, string>();
        string location;
        FileStream? myfile;
        List<string> defaultValues = new List<string>();
        int inProgress = 0;
        public ThumbnailGrabber()
        {
            InitializeComponent();
            textBox1.Location = new System.Drawing.Point((this.ClientSize.Width - textBox1.Width) / 2, 10);
            pictureBox1.Location = new System.Drawing.Point((this.ClientSize.Width - pictureBox1.Width) / 2, 40);
            flowLayoutPanel1.Location = new System.Drawing.Point((this.ClientSize.Width - flowLayoutPanel1.Width) / 2, 230);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:141.0) Gecko/20100101 Firefox/141.0");
            label1.Location = new System.Drawing.Point((this.ClientSize.Width - label1.Width) / 2 + textBox1.Width / 2 + label1.Width / 2 + 1, 40);
            label2.Location = new System.Drawing.Point((this.ClientSize.Width - label2.Width) / 2 + textBox1.Width / 2 + label2.Width / 2 + 1, 40 + label1.Height);
            label3.Location = new System.Drawing.Point((this.ClientSize.Width - label3.Width) / 2 + textBox1.Width / 2 + label3.Width / 2 + 1, 40 + label1.Height * 2);
            label4.Location = new System.Drawing.Point((this.ClientSize.Width - label4.Width) / 2 + textBox1.Width / 2 + label4.Width / 2 + 1, 40 + label1.Height * 3);
            label5.Location = new System.Drawing.Point((this.ClientSize.Width - label5.Width) / 2 + textBox1.Width / 2 + label5.Width / 2 + 1, 40 + label1.Height * 4);
            label6.Location = new System.Drawing.Point((this.ClientSize.Width - label6.Width) / 2 + textBox1.Width / 2 + label6.Width / 2 + 1, 40 + label1.Height * 5);
            label7.Location = new System.Drawing.Point((this.ClientSize.Width - label7.Width) / 2 + textBox1.Width / 2 + label7.Width / 2 + 1, 40 + label1.Height * 6);
            label8.Location = new System.Drawing.Point((this.ClientSize.Width - label8.Width) / 2 + textBox1.Width / 2 + label8.Width / 2 + 1, 40 + label1.Height * 7);
            label9.Location = new System.Drawing.Point((this.ClientSize.Width - label9.Width) / 2 + textBox1.Width / 2 + label9.Width / 2 + 1, 40 + label1.Height * 8);
            label10.Location = new System.Drawing.Point((this.ClientSize.Width - label10.Width) / 2 + textBox1.Width / 2 + label10.Width / 2 + 1, 40 + label1.Height * 9);
            label11.Location = new System.Drawing.Point((this.ClientSize.Width - label11.Width) / 2 + textBox1.Width / 2 + label11.Width / 2 + 1, 40 + label1.Height * 10);
            label12.Location = new System.Drawing.Point((this.ClientSize.Width - label12.Width) / 2 + textBox1.Width / 2 + label12.Width / 2 + 1, 40 + label1.Height * 11);
            label13.Location = new System.Drawing.Point((this.ClientSize.Width - label13.Width) / 2 + textBox1.Width / 2 + label13.Width / 2 + 1, 40 + label1.Height * 14);
            label14.Location = new System.Drawing.Point((this.ClientSize.Width - label14.Width) / 2 + textBox1.Width / 2 + label14.Width / 2 + 1, 40 + label1.Height * 12);
            label15.Location = new System.Drawing.Point((this.ClientSize.Width - label15.Width) / 2 + textBox1.Width / 2 + label15.Width / 2 + 1, 40 + label1.Height * 13);
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            foreach (string[] x in subdomains)
            {
                subdomainQueue.Enqueue(x);
            }
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thumbnails"))
            {
                location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thumbnails";
            }
            else
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "thumbnails"));
                location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thumbnails";
            }
        }
        private void ThumbnailGrabberSizeChange(object sender, EventArgs e)
        {
            textBox1.Location = new System.Drawing.Point((this.ClientSize.Width - textBox1.Width) / 2, 10);
            pictureBox1.Location = new System.Drawing.Point((this.ClientSize.Width - pictureBox1.Width) / 2, 40);
            flowLayoutPanel1.Location = new System.Drawing.Point((this.ClientSize.Width - flowLayoutPanel1.Width) / 2, 230);
            label1.Location = new System.Drawing.Point((this.ClientSize.Width - label1.Width) / 2 + textBox1.Width / 2 + label1.Width / 2 + 1, 40);
            label2.Location = new System.Drawing.Point((this.ClientSize.Width - label2.Width) / 2 + textBox1.Width / 2 + label2.Width / 2 + 1, 40 + label1.Height);
            label3.Location = new System.Drawing.Point((this.ClientSize.Width - label3.Width) / 2 + textBox1.Width / 2 + label3.Width / 2 + 1, 40 + label1.Height * 2);
            label4.Location = new System.Drawing.Point((this.ClientSize.Width - label4.Width) / 2 + textBox1.Width / 2 + label4.Width / 2 + 1, 40 + label1.Height * 3);
            label5.Location = new System.Drawing.Point((this.ClientSize.Width - label5.Width) / 2 + textBox1.Width / 2 + label5.Width / 2 + 1, 40 + label1.Height * 4);
            label6.Location = new System.Drawing.Point((this.ClientSize.Width - label6.Width) / 2 + textBox1.Width / 2 + label6.Width / 2 + 1, 40 + label1.Height * 5);
            label7.Location = new System.Drawing.Point((this.ClientSize.Width - label7.Width) / 2 + textBox1.Width / 2 + label7.Width / 2 + 1, 40 + label1.Height * 6);
            label8.Location = new System.Drawing.Point((this.ClientSize.Width - label8.Width) / 2 + textBox1.Width / 2 + label8.Width / 2 + 1, 40 + label1.Height * 7);
            label9.Location = new System.Drawing.Point((this.ClientSize.Width - label9.Width) / 2 + textBox1.Width / 2 + label9.Width / 2 + 1, 40 + label1.Height * 8);
            label10.Location = new System.Drawing.Point((this.ClientSize.Width - label10.Width) / 2 + textBox1.Width / 2 + label10.Width / 2 + 1, 40 + label1.Height * 9);
            label11.Location = new System.Drawing.Point((this.ClientSize.Width - label11.Width) / 2 + textBox1.Width / 2 + label11.Width / 2 + 1, 40 + label1.Height * 10);
            label12.Location = new System.Drawing.Point((this.ClientSize.Width - label12.Width) / 2 + textBox1.Width / 2 + label12.Width / 2 + 1, 40 + label1.Height * 11);
            label13.Location = new System.Drawing.Point((this.ClientSize.Width - label13.Width) / 2 + textBox1.Width / 2 + label13.Width / 2 + 1, 40 + label1.Height * 14);
            label14.Location = new System.Drawing.Point((this.ClientSize.Width - label14.Width) / 2 + textBox1.Width / 2 + label14.Width / 2 + 1, 40 + label1.Height * 12);
            label15.Location = new System.Drawing.Point((this.ClientSize.Width - label15.Width) / 2 + textBox1.Width / 2 + label15.Width / 2 + 1, 40 + label1.Height * 13);
        }
        private void ThumbnailGrabberShown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label)
                {
                    defaultValues.Add(ctrl.Name+" "+ctrl.Text);
                }
            }
        }
        private async void TextBoxSubmit(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && inProgress == 0)
            {
                inProgress = 1;
                pictureBox1.ImageLocation = null;
                myfile = File.Open(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thumbnails" + "/Links.txt",FileMode.Append,FileAccess.Write);
                foreach (string item in defaultValues)
                {
                    string name = item.Split(" ")[0];
                    string defaultValue;
                    try
                    {
                        defaultValue = item.Split(" ")[1] + " " + item.Split(" ")[2] + " " + item.Split(" ")[3];
                    }
                    catch
                    {
                        defaultValue = item.Split(" ")[1] + " " + item.Split(" ")[2];
                    }
                    Control[] controllist = this.Controls.Find(name, true);
                    if (controllist.Length > 0 && controllist[0] is Label)
                    {
                        controllist[0].Text = defaultValue;
                        controllist[0].Visible = false;
                    }
                }
                flowLayoutPanel1.Controls.Clear();
                string inputValue = textBox1.Text;
                string id = "";
                string id2 = "";
                byte[] base64bytes;
                if (inputValue.Length == 11 || inputValue.Contains("v="))
                {
                    if (inputValue.Contains("v="))
                    {
                        try
                        {
                            id = inputValue.Substring(inputValue.IndexOf("v=") + 2, 11);
                        }
                        catch
                        {
                            MessageBox.Show("Invalid URL!");
                            return;
                        }
                    }
                    if (inputValue.Length == 11)
                    {
                        id = inputValue;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid ID!");
                    return;
                }
                base64bytes = Convert.FromBase64String(id.Replace("-", "+").Replace("_", "/") + "=");
                id2 = Convert.ToBase64String(base64bytes).Substring(0, 11).Replace("+", "-").Replace("/", "_");
                if (id == id2)
                {
                    var response = await client.GetAsync($"https://i.ytimg.com/vi/{id}/mqdefault.jpg");
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Video not Available. Checking Archive.org");
                        List<Task> tasks = new List<Task>();
                        foreach (Control control in this.Controls)
                        {
                            if (control is Label label)
                            {
                                if (label.Name != "label13")
                                {
                                    label.Visible = true;
                                }
                            }
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            int tIndex = i;
                            tasks.Add(Task.Run(async () => await ThumbNailCDXGrabber(id, tIndex)));

                        }
                        await Task.WhenAll(tasks);
                        myfile.Close();
                        myfile = null;
                        tasks.Clear();
                        MessageBox.Show("All threads finished. Grabbing all the thumbnails from archive.org. (This may take a couple minutes)");
                        label13.Visible = true;
                        label13.Text = "Links Written To\n" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thumbnails" + "\\Links.txt";
                        if (URLList.Count > 0)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int tIndex = i;
                                tasks.Add(Task.Run(async () => await ThumbNailGrabber(tIndex)));

                            }
                        }
                        await Task.WhenAll(tasks);
                        if (bitmaps.Count > 0)
                        {
                            int thumbindex = 0;
                            foreach (KeyValuePair<Bitmap, string> kvp in bitmaps)
                            {
                                Bitmap bitmap = kvp.Key;
                                string name = kvp.Value;
                                if (name.Contains("?"))
                                {
                                    name = name.Split("?")[0];
                                }
                                if (name.Contains("."))
                                {
                                    name = name.Split(".")[0];
                                }
                                PictureBox pictureBox = new PictureBox();
                                pictureBox.Size = new System.Drawing.Size(bitmap.Width, bitmap.Height);
                                pictureBox.Image = bitmap;
                                pictureBox.Margin = new Padding(0);
                                flowLayoutPanel1.Controls.Add(pictureBox);
                                try
                                {
                                    bitmap.Save(location + $"/{thumbindex.ToString()}---{name}.png");
                                }
                                catch
                                {
                                    MessageBox.Show(name);
                                }
                                thumbindex++;
                            }
                            bitmaps.Clear();
                            URLList.Clear();
                        }
                    }
                    else
                    {
                        pictureBox1.ImageLocation = $"https://i.ytimg.com/vi/{id}/mqdefault.jpg";
                        myfile.Close();
                        myfile = null;
                    }
                }
                else
                {
                    MessageBox.Show("Invaid ID!");
                }
                inProgress = 0;
                subdomainQueue.Clear();
                foreach (string[] x in subdomains)
                {
                    subdomainQueue.Enqueue(x);
                }
            }
            
        }
        private async Task ThumbNailCDXGrabber(string URL, int index)
        {
            while (subdomainQueue.Count != 0)
            {
                int numlines = 0;
                HttpResponseMessage response;
                if (!subdomainQueue.TryDequeue(out string[]? result))
                {
                    break;
                }
                try
                {
                    if (result[0] == "imgyt")
                    {
                        response = await client.GetAsync($"http://web.archive.org/cdx/search/cdx?url=https://img.youtube.com/vi{result[1]}/{URL}*&filter=!statuscode:404&filter=!mimetype:warc/revisit&fl=timestamp,original&pageSize=10&page=0");
                    }
                    else
                    {
                        response = await client.GetAsync($"http://web.archive.org/cdx/search/cdx?url=https://{result[0]}.ytimg.com/vi{result[1]}/{URL}*&filter=!statuscode:404&filter=!mimetype:warc/revisit&fl=timestamp,original&pageSize=10&page=0");
                    }
                }
                catch
                {
                    subdomainQueue.Enqueue(result);
                    await Task.Delay(1000 * 30);
                    continue;
                }
                if (response.IsSuccessStatusCode)
                {
                    string respText = await response.Content.ReadAsStringAsync();
                    if (respText.Length != 0)
                    {
                        foreach (string line in respText.Split("\n"))
                        {
                            if (line.Length != 0)
                            {
                                URLList.Enqueue(line.Trim());
                                if (myfile != null)
                                {
                                    myfile.Write(Encoding.UTF8.GetBytes(line.Trim() + "\n"));
                                }
                                numlines++;
                            }
                        }
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control is Label label)
                            {
                                if (label.Text == $"{result[0]} {result[1]}: Loading")
                                {
                                    label.Text = $"{result[0]} {result[1]}: {numlines} Thumbnail(s) Found";
                                }
                                if (label.Text == $"{result[0]}: Loading" && result[1] == "")
                                {
                                    label.Text = $"{result[0]}: {numlines} Thumbnail(s) Found";
                                }
                                if (label.Text == $"img.youtube: Loading" && result[1] == "")
                                {
                                    label.Text = $"img.youtube: {numlines} Thumbnail(s) Found";
                                }
                                if (label.Text == $"img.youtube {result[1]}: Loading")
                                {
                                    label.Text = $"img.youtube {result[1]}: {numlines} Thumbnail(s) Found";
                                }
                            }
                        }
                    });
                }
                else
                {
                    subdomainQueue.Enqueue(result);
                    await Task.Delay(1000 * 30);
                    continue;
                }
                await Task.Delay(2000);
            }
        }
        private async Task ThumbNailGrabber(int index)
        {
            while (URLList.Count != 0)
            {
                if (!URLList.TryDequeue(out string? result))
                {
                    break;
                }
                string timestamp = result.Split(" ")[0].Trim();
                string URL = result.Split(" ")[1].Trim();
                string name = URL.Split("/")[URL.Split("/").Length-1];
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync($"http://web.archive.org/web/{timestamp}if_/{URL}");
                }
                catch
                {
                    URLList.Enqueue(result);
                    await Task.Delay(1000 * 30);
                    continue;
                }
                if (response.IsSuccessStatusCode)
                {
                    if (URL.Contains("?sqp=") || URL.Contains("_webp"))
                    {
                        try
                        {
                            var stream = await response.Content.ReadAsStreamAsync();
                            SixLabors.ImageSharp.Image MyImage = await SixLabors.ImageSharp.Image.LoadAsync(stream);
                            MyImage.Mutate(x => x.Resize(320, (int)(320 * ((double)x.GetCurrentSize().Height / x.GetCurrentSize().Width))));
                            var ms = new MemoryStream();
                            await MyImage.SaveAsBmpAsync(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            Bitmap bitmap = new Bitmap(ms);
                            bitmaps.Add(bitmap,name);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            System.Drawing.Image MyImage = System.Drawing.Image.FromStream(response.Content.ReadAsStream());
                            Bitmap resizedImage = new Bitmap(MyImage, 320, (int)(320 * ((double)MyImage.Height / MyImage.Width)));
                            bitmaps.Add(resizedImage,name);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    URLList.Enqueue(result);
                    await Task.Delay(1000 * 30);
                    continue;
                }
                await Task.Delay(1000);
            }
        }
        private void ThumbnailGrabberLeave(object sender, EventArgs e)
        {
            if (myfile != null)
            {
                myfile.Close();
            }
            URLList.Clear();
        }
    }
}
