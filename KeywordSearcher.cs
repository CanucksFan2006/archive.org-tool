using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.LinkLabel;
//Add when item is selected from listbox5
namespace Archive.org_Tools
{
    public partial class KeywordSearcher : Form
    {
        ConcurrentQueue<string> URLList = new ConcurrentQueue<string>();
        ConcurrentQueue<string> CDXList = new ConcurrentQueue<string>();
        ConcurrentQueue<string> ChangesList = new ConcurrentQueue<string>();
        ConcurrentQueue<string> PagesToWrite = new ConcurrentQueue<string>();
        FileStream? file = null;
        FileStream? pageFile = null;
        FileStream? resumeFile = null;
        FileStream? archiveResumeFile = null;
        List<string> remainingPages = new List<string>();
        List<string> keywords = new List<string>();
        private HttpClient client;
        private HttpClient pagesClient;
        private CookieContainer cookieContainer;
        private CookieContainer cookieContainer2;
        string? pages;
        string? URL;
        int inUse = 0;
        int exitFlag = 0;
        int maxSizeFlag = 0;
        int totalPages = 0;
        int finished = 0;
        int pnum = 1;
        int lineCount = 0;
        int lineCountInitial = 0;
        string mtype = "";
        string from = "";
        string to = "";
        int totalMatches = 0;
        List<Task> tasks = new List<Task>();
        Stopwatch stopwatch = new Stopwatch();
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(10,10);
        int pagesFinished = 0;
        string location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper";
        int itemNumber = -1;
        char[] invalidCharacters = Path.GetInvalidFileNameChars();
        public KeywordSearcher()
        {
            InitializeComponent();
            if (!System.IO.Directory.Exists(location))
            {
                System.IO.Directory.CreateDirectory(location);
            }
            if (!System.IO.File.Exists(location + "\\DefaultKeywords.txt"))
            {
                System.IO.File.Create(location + "\\DefaultKeywords.txt").Close();
            }
            cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = cookieContainer;
            client = new HttpClient(handler);
            cookieContainer2 = new CookieContainer();
            var handler2 = new HttpClientHandler();
            handler2.CookieContainer = cookieContainer2;
            pagesClient = new HttpClient(handler2);
            pagesClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:144.0) Gecko/20100101 Firefox/144.0");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:144.0) Gecko/20100101 Firefox/144.0");
            label3.MaximumSize = new Size(textBox2.Width, 0);
            string[] directories = System.IO.Directory.GetDirectories(location, "*", SearchOption.AllDirectories);
            foreach (string dir in directories)
            {
                if (System.IO.File.Exists(dir + "\\CDXLinksResume.txt"))
                {
                    listBox5.Items.Add(dir.Replace(location+"\\","").Replace("\\","/"));
                }
            }
            foreach (string dir in directories)
            {
                if (System.IO.Directory.GetFiles(dir,"CDXLinks*.txt").Length > 0 && !System.IO.File.Exists(dir + "\\CDXLinksResume.txt"))
                {
                    listBox6.Items.Add(dir.Replace(location + "\\", "").Replace("\\", "/"));
                }
            }
        }
        private void insertCDX(object sender, EventArgs e)
        {
            listBox5.Visible = false;
            listBox6.Visible = false;
            string item;
            if (listBox5.SelectedItem != null)
            {
                location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper";
                item = listBox5.SelectedItem.ToString();
                itemNumber = listBox5.SelectedIndex;
                CDXList.Clear();
                remainingPages.Clear();
            }
            else
            {
                listBox5.Visible = true;
                listBox6.Visible = true;
                return;
            }
            resumeFile = new FileStream((location + $"\\{item}\\CDXLinksResume.txt"), FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(resumeFile))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    remainingPages.Add(line.Trim());
                    try
                    {
                        int.Parse(line.Trim());
                        CDXList.Enqueue(line.Trim());
                    }
                    catch
                    {
                        if (line.Trim() == "m")
                        {
                            mtype = "&matchType=domain";
                        }
                        else if ((line[0] == 't' || line[0] == 'f') && long.TryParse(line.Replace("\n","").Substring(1), out long tempResult))
                        {
                            try
                            {
                                int.Parse(line.Substring(1));
                                if (line[0] == 't')
                                {
                                    to = line.Substring(1);
                                }
                                else
                                {
                                    from = line.Substring(1);
                                }
                            }
                            catch
                            {
                                URL = line.Trim();
                            }
                        }
                        else if (line.Trim() != "m")
                        {
                            URL = line.Trim();
                        }
                    }
                    pages = line.Trim();
                }
                label18.Text = $"Ready to resume {URL}";
                location += $"\\{item.Replace("/", "\\")}";
                listBox5.Visible = true;
                listBox6.Visible = true;
            }
            resumeFile.Close();
        }
        private void prepareKeywordScrape(object sender, EventArgs e)
        {
            string item;
            listBox5.Visible = false;
            listBox6.Visible = false;
            if (listBox6.SelectedItem != null)
            {
                URLList.Clear();
                ChangesList.Clear();
                lineCount = 0;
                pnum = 1;
                location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper";
                item = listBox6.SelectedItem.ToString();
                itemNumber = listBox6.SelectedIndex;
            }
            else
            {
                listBox5.Visible = true;
                listBox6.Visible = true;
                return;
            }
            while (!System.IO.File.Exists(location + $"\\{item}\\CDXLinks{pnum}.txt"))
            {
                pnum++;
            }
            pageFile = new FileStream((location + $"\\{item}\\CDXLinks{pnum}.txt"), FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(pageFile))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        long.Parse(line.Substring(0,14));
                        lineCount++;
                    }
                    catch
                    {
                        //
                    }
                }
                button6.Visible = true;
                label7.Visible = true;
                label7.Text = $"Pages Remaining: {lineCount}";
                lineCountInitial = lineCount;
                label19.Text = $"Ready to resume {item}";
                location += $"\\{item.Replace("/", "\\")}";
                listBox5.Visible = true;
                listBox6.Visible = true;
            }
            pageFile.Close();
            pageFile = null;
        }
        private void check()
        {
            this.Invoke((MethodInvoker)delegate
            {
                label9.Visible = true;
                label10.Visible = true;
            });
            while (true)
            {
                if (lineCount == 0)
                {
                    break;
                }
                if (exitFlag == 1)
                {
                    break;
                }
                if (stopwatch.ElapsedMilliseconds >= 300000.0)
                {
                    stopwatch.Restart();
                    lineCountInitial = lineCount;
                }
                try
                {
                    var val = ((lineCountInitial - lineCount) / (stopwatch.ElapsedMilliseconds / 1000.0)).ToString();
                    var abc = val.IndexOf(".");
                    var abcd = val.IndexOf(",");
                    if (abc == -1 && abcd == -1)
                    {
                        val += ".000";
                    }
                    else
                    {
                        string[] t;
                        if (abc != -1)
                        {
                            t = val.Split(".");
                            while (t[1].Length < 3)
                            {
                                t[1] += "0";
                            }
                            val = t[0] + "." + t[1].Substring(0, 3);
                        }
                        else
                        {
                            t = val.Split(",");
                            while (t[1].Length < 3)
                            {
                                t[1] += "0";
                            }
                            val = t[0] + "," + t[1].Substring(0, 3);
                        }
                    }
                    double.TryParse(val, out var val2);
                    var timeRemaining = (lineCount / val2);
                    var hoursRemaining = timeRemaining / 3600;
                    var minutesRemaining = (hoursRemaining % 1) * 60;
                    var secondsRemaining = (minutesRemaining % 1) * 60;
                    var hours = hoursRemaining.ToString();
                    var minutes = minutesRemaining.ToString();
                    var seconds = secondsRemaining.ToString();
                    abc = hours.IndexOf(".");
                    if (abc != -1)
                    {
                        hours = hours.Split(".")[0];
                    }
                    abc = minutes.IndexOf(".");
                    if (abc != -1)
                    {
                        minutes = minutes.Split(".")[0];
                    }
                    abc = seconds.IndexOf(".");
                    if (abc != -1)
                    {
                        seconds = seconds.Split(".")[0];
                    }
                    abc = hours.IndexOf(",");
                    if (abc != -1)
                    {
                        hours = hours.Split(",")[0];
                    }
                    abc = minutes.IndexOf(",");
                    if (abc != -1)
                    {
                        minutes = minutes.Split(",")[0];
                    }
                    abc = seconds.IndexOf(",");
                    if (abc != -1)
                    {
                        seconds = seconds.Split(",")[0];
                    }
                    if (hours.Length == 1)
                    {
                        hours = "0" + hours;
                    }
                    if (minutes.Length == 1)
                    {
                        minutes = "0" + minutes;
                    }
                    if (seconds.Length == 1)
                    {
                        seconds = "0" + seconds;
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        label9.Text = $"Pages Per Second: " +
                        $"{val}";
                        label10.Text = $"Time Remaining: {hours}:{minutes}:{seconds}";
                    });
                }
                catch
                {
                    break;
                }
            }
        }
        private async Task sizeCheck()
        {
            while (true)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    label14.Text = $"Items In Queue: {PagesToWrite.Count.ToString()}";
                });
                if (PagesToWrite.Count >= 2000000)
                {
                    maxSizeFlag = 1;
                    break;
                }
                if (exitFlag == 1)
                {
                    break;
                }
                if (CDXList.Count == 0)
                {
                    break;
                }
                await Task.Delay(100);
            }
        }
        private async Task insertPages()
        {
            using (FileStream myfile = new FileStream((location + $"\\CDXLinks{pnum}.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(myfile))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync();
                        await Task.Delay(100);
                        if (exitFlag == 1)
                        {
                            int.Parse(line.Substring(0, 1));
                            URLList.Enqueue(line.Trim());
                            break;
                        }
                        int.Parse(line.Substring(0, 1));
                        URLList.Enqueue(line.Trim());
                    }
                    catch
                    {
                        continue;
                    }
                }
                if (exitFlag == 1)
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        try
                        {
                            int.Parse(line.Substring(0, 1));
                            URLList.Enqueue((line.Trim()));
                        }
                        catch
                        {
                            continue;
                        }
                        
                    }
                    return;
                }
            }
            pagesFinished = 1;
        }
        private void KeywordShown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        private void AddKeyword(object sender, EventArgs e)
        {
            if (!listBox1.Items.Contains(textBox1.Text.Trim()))
            {
                listBox1.Items.Add(textBox1.Text.Trim());
            }
        }
        private void RemoveKeyword(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }
        private void AddDefaults(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper" + $"\\DefaultKeywords.txt"))
            {
                using (FileStream myfile = new FileStream((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper" + "\\DefaultKeywords.txt"), FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(myfile))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        listBox1.Items.Add(line.Trim());
                    }
                    if (line == null && listBox1.Items.Count == 0)
                    {
                        MessageBox.Show("DefaultKeywords.txt Is Empty. Add Your Own Keywords to this txt File (1 per Line)");
                    }
                }
            }
            else
            {
                MessageBox.Show("DefaultKeywords.txt Does Not Exist");
            }
        }
        private void ButtonTextEdit(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text != null)
                {
                    int.Parse(textBox3.Text);
                    button4.Text = $"Get Page {textBox3.Text}";
                    button4.Visible = true;
                }
                else
                {
                    button4.Visible = false;
                }
            }
            catch
            {
                button4.Visible = false;
            }
        }
        private async void GetPages(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    return;
                }
                location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper";
                CDXList.Clear();
                from = "";
                to = "";
                mtype = "";
                if (CDXList.Count == 0)
                {
                    URL = textBox2.Text.Trim();
                    if (URL.Contains("https://"))
                    {
                        URL = URL.Replace("https://", "");
                    }
                    if (URL.Contains("http://"))
                    {
                        URL = URL.Replace("http://", "");
                    }
                    if (URL.IndexOf("/") == -1)
                    {
                        URL += "/";
                    }
                    List<string> items = URL.Split("/").Where(s => s != "").ToList();
                    List<string> filteredItems = [];
                    foreach (string item in items)
                    {
                        string y = new string(item.Where(x => !invalidCharacters.Contains(x)).ToArray());
                        filteredItems.Add(y);
                    }
                    int folderMatchCount = 0;
                    foreach (string item in filteredItems)
                    {
                        if (!System.IO.Directory.Exists(location + $"\\{item}"))
                        {
                            System.IO.Directory.CreateDirectory(location + $"\\{item}");
                        }
                        else
                        {
                            folderMatchCount++;
                        }
                        if (System.IO.Directory.Exists(location + $"\\{item}") && items[items.Count - 1] == item && folderMatchCount == items.Count && System.IO.File.Exists(location + $"\\{item}\\CDXLinksResume.txt"))
                        {
                            MessageBox.Show("You already have a crawl for this location waiting to be resumed");
                            return;
                        }
                        location += $"\\{item}";
                    }
                    location += "\\";
                    listBox2.Items.Clear();
                    HttpResponseMessage result;
                    try
                    {
                        if (from == "")
                        {
                            from = textBox4.Text.Trim();
                        }
                        if (to == "")
                        {
                            to = textBox5.Text.Trim();
                        }
                        if (checkBox1.Checked == true)
                        {
                            mtype = "&matchType=domain";
                        }
                        result = await pagesClient.GetAsync($"https://web.archive.org/cdx/search/cdx?url={URL}*&filter=statuscode:200&filter=!mimetype:warc/revisit&from={from}&to={to}{mtype}&pageSize=10&page=0&showNumPages=true");
                    }
                    catch
                    {
                        MessageBox.Show("Error Fetching Total Pages");
                        return;
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        CDXList.Clear();
                        string resultText = await result.Content.ReadAsStringAsync();
                        pages = resultText.Trim();
                        label3.Text = $"{textBox2.Text} has {pages} page(s)";
                        label3.Visible = true;
                        resumeFile = System.IO.File.Open(location + "\\CDXLinksResume.txt", FileMode.Append, FileAccess.Write);
                        if (inUse == 0 && resumeFile != null)
                        {
                            resumeFile.Write(Encoding.UTF8.GetBytes(URL + "\n"));
                            remainingPages.Add(URL);
                            if (mtype != "")
                            {
                                resumeFile.Write(Encoding.UTF8.GetBytes("m" + "\n"));
                                remainingPages.Add("m");
                            }
                            if (from != "")
                            {
                                resumeFile.Write(Encoding.UTF8.GetBytes("f" + from + "\n"));
                                remainingPages.Add("f" + from);
                            }
                            if (to != "")
                            {
                                resumeFile.Write(Encoding.UTF8.GetBytes("t" + to + "\n"));
                                remainingPages.Add("t" + to);
                            }
                            for (int i = 0; i < int.Parse(pages); i++)
                            {
                                CDXList.Enqueue(i.ToString());
                                remainingPages.Add(i.ToString());
                                resumeFile.Write(Encoding.UTF8.GetBytes(i.ToString() + "\n"));
                            }
                            resumeFile.Flush();
                            resumeFile.Close();
                            resumeFile = null;
                            listBox5.Items.Add(URL);
                        }
                        else
                        {
                            MessageBox.Show("Error writing to CDXLinksResume.txt");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Error {(int)result.StatusCode}");
                    }
                }
                else
                {
                    MessageBox.Show($"There is already a crawl waiting to be resumed. If you want to crawl a new domain move all of the CDXLinks files and the CDXLinksResume file out of {location}");
                }
            }
        }
        private async void GetPagesButton(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                return;
            }
            location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper";
            CDXList.Clear();
            from = "";
            to = "";
            mtype = "";
            if (CDXList.Count == 0)
            {
                URL = textBox2.Text.Trim();
                if (URL.Contains("https://"))
                {
                    URL = URL.Replace("https://", "");
                }
                if (URL.Contains("http://"))
                {
                    URL = URL.Replace("http://", "");
                }
                if (URL.IndexOf("/") == -1)
                {
                    URL += "/";
                }
                List<string> items = URL.Split("/").Where(s => s != "").ToList();
                List<string> filteredItems = [];
                foreach (string item in items)
                {
                    string y = new string(item.Where(x => !invalidCharacters.Contains(x)).ToArray());
                    filteredItems.Add(y);
                }
                int folderMatchCount = 0;
                foreach (string item in filteredItems)
                {
                    if (!System.IO.Directory.Exists(location + $"\\{item}"))
                    {
                        System.IO.Directory.CreateDirectory(location + $"\\{item}");
                    }
                    else
                    {
                        folderMatchCount++;
                    }
                    if (System.IO.Directory.Exists(location + $"\\{item}") && items[items.Count - 1] == item && folderMatchCount == items.Count && System.IO.File.Exists(location + $"\\{item}\\CDXLinksResume.txt"))
                    {
                        MessageBox.Show("You already have a crawl for this location waiting to be resumed");
                        return;
                    }
                    location += $"\\{item}";
                }
                location += "\\";
                listBox2.Items.Clear();
                HttpResponseMessage result;
                try
                {
                    if (from == "")
                    {
                        from = textBox4.Text.Trim();
                    }
                    if (to == "")
                    {
                        to = textBox5.Text.Trim();
                    }
                    if (checkBox1.Checked == true)
                    {
                        mtype = "&matchType=domain";
                    }
                    result = await pagesClient.GetAsync($"https://web.archive.org/cdx/search/cdx?url={URL}*&filter=statuscode:200&filter=!mimetype:warc/revisit&from={from}&to={to}{mtype}&pageSize=10&page=0&showNumPages=true");
                }
                catch
                {
                    MessageBox.Show("Error Fetching Total Pages");
                    return;
                }
                if (result.IsSuccessStatusCode)
                {
                    CDXList.Clear();
                    string resultText = await result.Content.ReadAsStringAsync();
                    pages = resultText.Trim();
                    label3.Text = $"{textBox2.Text} has {pages} page(s)";
                    label3.Visible = true;
                    resumeFile = System.IO.File.Open(location + "\\CDXLinksResume.txt", FileMode.Append, FileAccess.Write);
                    if (inUse == 0 && resumeFile != null)
                    {
                        resumeFile.Write(Encoding.UTF8.GetBytes(URL + "\n"));
                        remainingPages.Add(URL);
                        if (mtype != "")
                        {
                            resumeFile.Write(Encoding.UTF8.GetBytes("m" + "\n"));
                            remainingPages.Add("m");
                        }
                        if (from != "")
                        {
                            resumeFile.Write(Encoding.UTF8.GetBytes("f" + from + "\n"));
                            remainingPages.Add("f" + from);
                        }
                        if (to != "")
                        {
                            resumeFile.Write(Encoding.UTF8.GetBytes("t" + to + "\n"));
                            remainingPages.Add("t" + to);
                        }
                        for (int i = 0; i < int.Parse(pages); i++)
                        {
                            CDXList.Enqueue(i.ToString());
                            remainingPages.Add(i.ToString());
                            resumeFile.Write(Encoding.UTF8.GetBytes(i.ToString() + "\n"));
                        }
                        resumeFile.Flush();
                        resumeFile.Close();
                        resumeFile = null;
                        listBox5.Items.Add(URL);
                    }
                    else
                    {
                        MessageBox.Show("Error writing to CDXLinksResume.txt");
                    }
                }
                else
                {
                    MessageBox.Show($"Error {(int)result.StatusCode}");
                }
            }
            else
            {
                MessageBox.Show($"There is already a crawl waiting to be resumed. If you want to crawl a new domain move all of the CDXLinks files and the CDXLinksResume file out of {location}");
            }
        }
        private async void GetPageContent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (URL != null)
                {
                    if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                    {
                        while (System.IO.File.Exists(location + $"\\CDXLinks{pnum + 1}.txt"))
                        {
                            pnum++;
                        }
                        lineCount = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
                        if (lineCount == 100000)
                        {
                            pnum++;
                        }
                    }
                    else
                    {
                        lineCount = 0;
                    }
                    int addedPages = 0;
                    string num = (int.Parse(textBox3.Text) - 1).ToString();
                    button3.Enabled = false;
                    HttpResponseMessage result;
                    if (!remainingPages.Contains(num.Trim()))
                    {
                        MessageBox.Show("That page was already captured");
                        button3.Enabled = true;
                        return;
                    }
                    MessageBox.Show("Getting Page");
                    if (from == "")
                    {
                        from = textBox4.Text.Trim();
                    }
                    if (to == "")
                    {
                        to = textBox5.Text.Trim();
                    }
                    if (checkBox1.Checked == true)
                    {
                        mtype = "&matchType=domain";
                    }
                    try
                    {
                        result = await client.GetAsync($"https://web.archive.org/cdx/search/cdx?url={URL}*&fl=timestamp,original&filter=statuscode:200&filter=!mimetype:warc/revisit&from={from}&to={to}{mtype}&pageSize=10&page={int.Parse(textBox3.Text) - 1}");
                    }
                    catch
                    {
                        MessageBox.Show("You Got Rate Limited");
                        button3.Enabled = true;
                        return;
                    }
                    if (result.IsSuccessStatusCode)
                    {
                        string resultText = await result.Content.ReadAsStringAsync();
                        foreach (string line in resultText.Split("\n"))
                        {
                            if (line.Length > 0)
                            {
                                PagesToWrite.Enqueue(line.Trim());
                                lineCount++;
                                addedPages++;
                                if (lineCount == 100000)
                                {
                                    while (PagesToWrite.TryDequeue(out string? output))
                                    {
                                        if (output != null)
                                        {
                                            using (FileStream myfile = System.IO.File.Open(location + $"\\CDXLinks{pnum}.txt", FileMode.Append, FileAccess.Write))
                                            {
                                                myfile.Write(Encoding.UTF8.GetBytes(output.Trim()+"\n"));
                                            }
                                        }
                                    }
                                    pnum++;
                                    lineCount = 0;
                                }
                            }
                        }
                        remainingPages.Remove(num);
                        MessageBox.Show($"{addedPages.ToString()} URLs Added to Queue");
                        if (PagesToWrite.Count == 0)
                        {
                            PagesToWrite.Enqueue("Nothing");
                        }
                        if (PagesToWrite.Count != 0)
                        {
                            if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                            {
                                while (System.IO.File.Exists(location + $"\\CDXLinks{pnum + 1}.txt"))
                                {
                                    pnum++;
                                }
                                lineCount = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
                                if (lineCount == 100000)
                                {
                                    pnum++;
                                }
                            }
                            else
                            {
                                lineCount = 0;
                            }
                            if (resumeFile != null)
                            {
                                resumeFile.Close();
                            }
                            var resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                            List<string> filteredMain;
                            List<string> filtered;
                            try
                            {
                                filtered = PagesToWrite.Where(s => s != null).ToList();
                                if (filtered.Count > 0)
                                {
                                    resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                    FileStream myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                                    foreach (string line in filtered)
                                    {
                                        if (long.TryParse(line.Substring(0, 14), out long number))
                                        {
                                            myfile.Write(Encoding.UTF8.GetBytes(line.Trim() + "\n"));
                                            lineCount++;
                                            if (lineCount % 100000 == 0)
                                            {
                                                lineCount = 0;
                                                myfile.Close();
                                                pnum++;
                                                resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                                myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                                            }
                                        }
                                    }
                                    myfile.Close();
                                    PagesToWrite.Clear();
                                    lineCount = 0;
                                }
                            }
                            catch
                            {
                                //
                            }
                            resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                            filteredMain = remainingPages.Where(s => s != null).ToList();
                            filtered = remainingPages.Where(s => s != null && int.TryParse(s, out int result)).ToList();
                            if (filtered.Count == 0)
                            {
                                System.IO.File.Delete(resumeFilePath);
                                string[] list = location.Split(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper");
                                string locationToCheck = location + "\\";
                                string[] list1 = list[1].Split("\\");
                                Array.Reverse(list1);
                                foreach (string loc in list1)
                                {
                                    if (loc == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper" || loc == "")
                                    {
                                        continue;
                                    }
                                    if (System.IO.Directory.GetFiles(locationToCheck, "CDXLinks*.txt").Length == 0 && System.IO.Directory.GetDirectories(locationToCheck).Length == 0)
                                    {
                                        System.IO.Directory.Delete(locationToCheck);
                                    }
                                    else
                                    {
                                        if (locationToCheck == location + "\\" && !System.IO.File.Exists(location + "\\CDXLinksResume.txt"))
                                        {
                                            listBox6.Items.Add(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                                        }
                                        break;
                                    }
                                    locationToCheck = locationToCheck.Substring(0, locationToCheck.Length - loc.Length - 1);
                                }
                                listBox5.Items.Remove(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                            }
                            else
                            {
                                System.IO.File.WriteAllLines(resumeFilePath, filteredMain);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Error {(int)result.StatusCode}");
                    }
                }
                else
                {
                    MessageBox.Show("You must put a URL in the 2nd text box and get its total pages first");
                }
                button3.Enabled = true;
            }
        }
        private async void GetPageContentButton(object sender, EventArgs e)
        {
            if (URL != null)
            {
                if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                {
                    while (System.IO.File.Exists(location + $"\\CDXLinks{pnum + 1}.txt"))
                    {
                        pnum++;
                    }
                    lineCount = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
                    if (lineCount == 100000)
                    {
                        pnum++;
                    }
                }
                else
                {
                    lineCount = 0;
                }
                int addedPages = 0;
                string num = (int.Parse(textBox3.Text) - 1).ToString();
                button3.Enabled = false;
                HttpResponseMessage result;
                if (!remainingPages.Contains(num.Trim()))
                {
                    MessageBox.Show("That page was already captured");
                    button3.Enabled = true;
                    return;
                }
                MessageBox.Show("Getting Page");
                if (from == "")
                {
                    from = textBox4.Text.Trim();
                }
                if (to == "")
                {
                    to = textBox5.Text.Trim();
                }
                if (checkBox1.Checked == true)
                {
                    mtype = "&matchType=domain";
                }
                try
                {
                    result = await client.GetAsync($"https://web.archive.org/cdx/search/cdx?url={URL}*&fl=timestamp,original&filter=statuscode:200&filter=!mimetype:warc/revisit&from={from}&to={to}{mtype}&pageSize=10&page={int.Parse(textBox3.Text) - 1}");
                }
                catch
                {
                    MessageBox.Show("You Got Rate Limited");
                    button3.Enabled = true;
                    return;
                }
                if (result.IsSuccessStatusCode)
                {
                    string resultText = await result.Content.ReadAsStringAsync();
                    foreach (string line in resultText.Split("\n"))
                    {
                        if (line.Length > 0)
                        {
                            PagesToWrite.Enqueue(line.Trim());
                            lineCount++;
                            addedPages++;
                            if (lineCount == 100000)
                            {
                                while (PagesToWrite.TryDequeue(out string? output))
                                {
                                    if (output != null && output != "")
                                    {
                                        using (FileStream myfile = System.IO.File.Open(location + $"\\CDXLinks{pnum}.txt", FileMode.Append, FileAccess.Write))
                                        {
                                            myfile.Write(Encoding.UTF8.GetBytes(output.Trim()+"\n"));
                                        }
                                    }
                                }
                                pnum++;
                                lineCount = 0;
                            }    
                        }
                    }
                    remainingPages.Remove(num);
                    MessageBox.Show($"{addedPages.ToString()} URLs Added to Queue");
                    if (PagesToWrite.Count == 0)
                    {
                        PagesToWrite.Enqueue("Nothing");
                    }
                    if (PagesToWrite.Count != 0)
                    {
                        if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                        {
                            while (System.IO.File.Exists(location + $"\\CDXLinks{pnum + 1}.txt"))
                            {
                                pnum++;
                            }
                            lineCount = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
                            if (lineCount == 100000)
                            {
                                pnum++;
                            }
                        }
                        else
                        {
                            lineCount = 0;
                        }
                        if (resumeFile != null)
                        {
                            resumeFile.Close();
                        }
                        var resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                        List<string> filteredMain;
                        List<string> filtered;
                        try
                        {
                            filtered = PagesToWrite.Where(s => s != null).ToList();
                            if (filtered.Count > 0)
                            {
                                resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                FileStream myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                                foreach (string line in filtered)
                                { 
                                    if (long.TryParse(line.Substring(0, 14), out long number))
                                    {
                                        myfile.Write(Encoding.UTF8.GetBytes(line.Trim() + "\n"));
                                        lineCount++;
                                        if (lineCount % 100000 == 0)
                                        {
                                            lineCount = 0;
                                            myfile.Close();
                                            pnum++;
                                            resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                            myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                                        }
                                    }
                                }
                                myfile.Close();
                                PagesToWrite.Clear();
                                lineCount = 0;
                            }
                        }
                        catch
                        {
                            //
                        }
                        resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                        filteredMain = remainingPages.Where(s => s != null).ToList();
                        filtered = remainingPages.Where(s => s != null && int.TryParse(s, out int result)).ToList();
                        if (filtered.Count == 0)
                        {
                            System.IO.File.Delete(resumeFilePath);
                            string[] list = location.Split(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper");
                            string locationToCheck = location + "\\";
                            string[] list1 = list[1].Split("\\");
                            Array.Reverse(list1);
                            foreach (string loc in list1)
                            {
                                if (loc == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper" || loc == "")
                                {
                                    continue;
                                }
                                if (System.IO.Directory.GetFiles(locationToCheck, "CDXLinks*.txt").Length == 0 && System.IO.Directory.GetDirectories(locationToCheck).Length == 0)
                                {
                                    System.IO.Directory.Delete(locationToCheck);
                                }
                                else
                                {
                                    if (locationToCheck == location + "\\" && !System.IO.File.Exists(location + "\\CDXLinksResume.txt"))
                                    {
                                        listBox6.Items.Add(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                                    }
                                    break;
                                }
                                locationToCheck = locationToCheck.Substring(0, locationToCheck.Length - loc.Length - 1);
                            }
                            listBox5.Items.Remove(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                        }
                        else
                        {
                            System.IO.File.WriteAllLines(resumeFilePath, filteredMain);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Error {(int)result.StatusCode}");
                }
            }
            else
            {
                MessageBox.Show("You must put a URL in the 2nd text box and get its total pages first");
            }
            button3.Enabled = true;
        }
        private async void GetAllPagesThreadMaker(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button6.Enabled = false;
            button4.Enabled = false;
            listBox5.Visible = false;
            listBox6.Visible = false;
            label3.Visible = false;
            if (file == null && !System.IO.File.Exists(location + "\\CDXLinksResume.txt"))
            {
                MessageBox.Show("All of the CDX Links are finished");
                button3.Enabled = true;
                button6.Enabled = true;
                button4.Enabled = true;
                listBox5.Visible = true;
                listBox6.Visible = true;
                label3.Visible = true;
                return;
            }
            if (from == "")
            {
                from = textBox4.Text;
            }
            if (to == "")
            {
                to = textBox5.Text;
            }
            if (checkBox1.Checked == true)
            {
                mtype = "&matchType=domain";
            }
            if (pages != null && URL != null)
            {
                if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                {
                    while (System.IO.File.Exists(location + $"\\CDXLinks{pnum + 1}.txt"))
                    {
                        pnum++;
                    }
                    lineCount = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
                    if (lineCount == 100000)
                    {
                        pnum++;
                    }

                }
                else
                {
                    lineCount = 0;
                }
                MessageBox.Show("Creating Threads");
                label14.Visible = true;
                while (CDXList.Count != 0 && exitFlag != 1)
                {
                    label14.Text = $"Items In Queue: {PagesToWrite.Count.ToString()}";
                    tasks.Add(Task.Run(() => sizeCheck()));
                    resumeFile = System.IO.File.Open(location + "\\CDXLinksResume.txt", FileMode.Append, FileAccess.Write);
                    for (int x = 0; x < 10; x++)
                    {
                        int tIndex = x;
                        tasks.Add(Task.Run(async () => await GetAllPages(URL, tIndex, mtype, from, to)));
                        await Task.Delay(100);
                    }
                    await Task.WhenAll(tasks);
                    pagesFinished = 0;
                    label14.Text = $"Items In Queue: {PagesToWrite.Count.ToString()}";
                    if (exitFlag != 1)
                    {
                        if (resumeFile != null)
                        {
                            resumeFile.Close();
                            resumeFile = null;
                        }
                        if (maxSizeFlag != 1)
                        {
                            MessageBox.Show("Done");
                        }
                        var resumeFilePath = "";
                        List<string> filtered;
                        if (CDXList.Count != 0)
                        {
                            resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                            filtered = remainingPages.Where(s => s != null).ToList();
                            System.IO.File.WriteAllLines(resumeFilePath, filtered);
                        }
                        else
                        {
                            resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                            System.IO.File.Delete(resumeFilePath);
                        }
                        if (PagesToWrite.Count > 0)
                        {
                            filtered = PagesToWrite.Where(s => s != null).ToList();
                            if (filtered.Count > 0)
                            {
                                resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                file = new FileStream((location + $"\\CDXLinks{pnum}.txt"), FileMode.Append, FileAccess.Write);
                                foreach (string line in filtered)
                                {
                                    file.Write(Encoding.UTF8.GetBytes(line.Trim() + "\n"));
                                    lineCount++;
                                    if (lineCount % 100000 == 0)
                                    {
                                        lineCount = 0;
                                        file.Close();
                                        pnum++;
                                        resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                        file = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                                    }
                                }
                            }
                        }
                        else
                        {
                            filtered = [];
                        }
                        PagesToWrite.Clear();
                        if (filtered.Count > 0)
                        {
                            filtered.Clear();
                        }
                        maxSizeFlag = 0;
                        if (file != null)
                        {
                            file.Close();
                            file = null;
                        }
                    }
                }
                if (CDXList.Count == 0)
                {
                    if (file != null)
                    {
                        file.Close();
                        file = null;
                    }
                    if (resumeFile != null)
                    {
                        resumeFile.Close();
                        resumeFile = null;
                    }
                }
                string[] list = location.Split(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper");
                string locationToCheck = location + "\\";
                string[] list1 = list[1].Split("\\");
                Array.Reverse(list1);
                foreach (string loc in list1)
                {
                    if (loc == Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\KeywordScraper" || loc == "")
                    {
                        continue;
                    }
                    if (System.IO.Directory.GetFiles(locationToCheck, "CDXLinks*.txt").Length == 0 && System.IO.Directory.GetDirectories(locationToCheck).Length == 0)
                    {
                        System.IO.Directory.Delete(locationToCheck);
                    }
                    else
                    {
                        if (locationToCheck == location + "\\" && !System.IO.File.Exists(location + "\\CDXLinksResume.txt"))
                        {
                            listBox6.Items.Add(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                        }
                        break;
                    }
                    locationToCheck = locationToCheck.Substring(0, locationToCheck.Length - loc.Length - 1);
                }
                listBox5.Items.Remove(listBox5.Items[listBox5.Items.IndexOf(list[1].Substring(1).Replace("\\", "/"))]);
                try
                {
                    var lines = System.IO.File.ReadLines(location + $"\\CDXLinks1.txt").Where(line => int.TryParse(line.ToString().Substring(0, 1), out int result)).ToList();
                    button6.Visible = true;
                    label7.Text = $"Pages Remaining: {lines.Count.ToString()}";
                    label7.Visible = true;
                    button6.Enabled = true;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
                catch
                {
                    button6.Visible = true;
                    label7.Text = $"Pages Remaining: 0";
                    label7.Visible = true;
                    button6.Enabled = true;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("You must put a URL in the 2nd text box and get its total pages first");
            }
            if (file != null && exitFlag != 1)
            {
                file.Close();
                file = null;
            }
            button6.Enabled = true;
            button4.Enabled = true;
            button3.Enabled = true;
            listBox5.Visible = true;
            listBox6.Visible = true;
        }
        private async Task GetAllPages(string URL, int id, string mtype, string from, string to)
        {
            while (CDXList.Count != 0)
            {
                HttpResponseMessage response;
                bool restart = false;
                if (exitFlag == 1)
                {
                    break;
                }
                if (maxSizeFlag == 1)
                {
                    break;
                }
                if (!CDXList.TryDequeue(out string? pageNum))
                {
                    break;
                }
                if (!remainingPages.Contains(pageNum.Trim()))
                {
                    break;
                }
                try
                {
                    response = await client.GetAsync($"https://web.archive.org/cdx/search/cdx?url={URL}*&fl=timestamp,original&filter=statuscode:200&filter=!mimetype:warc/revisit&from={from}&to={to}{mtype}&pageSize=10&page={pageNum}");
                }
                catch
                {
                    CDXList.Enqueue(pageNum);
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox2.Items.Add($"Page {pageNum}: Error");
                    });
                    await Task.Delay(1000 * 45);
                    continue;
                }
                if (response.IsSuccessStatusCode)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox2.Items.Add($"Page {pageNum}: Success");
                    });
                    string respText = await response.Content.ReadAsStringAsync();
                    if (respText.Length != 0)
                    {
                        string[] listOfLines = respText.Split("\n");
                        totalPages = 0;
                        foreach (string line in listOfLines)
                        {
                            if (line.Length != 0)
                            {
                                totalPages++;
                                PagesToWrite.Enqueue(line.Trim());
                                URLList.Enqueue(line.Trim());
                            }
                        }
                    }
                    remainingPages.Remove(pageNum);
                }
                else
                {
                    if ((int)response.StatusCode != 400)
                    {
                        CDXList.Enqueue(pageNum);
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox2.Items.Add($"Page {pageNum}: Error {(int)response.StatusCode}");
                    });
                    await Task.Delay(1000 * 45);
                    continue;
                }
                await Task.Delay(1000 * 4);
            }
        }
        private async void GetAllArchivedPagesThreadMaker(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button6.Enabled = false;
            button4.Enabled = false;
            listBox5.Visible = false;
            listBox6.Visible = false;
            label3.Visible = false;
            label17.Visible = true;
            keywords.Clear();
            int start = 0;
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("You Must Have at Least One Keyword That You're Searching For");
                button3.Enabled = true;
                button6.Enabled = true;
                button4.Enabled = true;
                listBox5.Visible = true;
                listBox6.Visible = true;
                label3.Visible = true;
                label17.Visible = false;
                return;
            }
            else
            {
                foreach (string keyword in listBox1.Items)
                {
                    keywords.Add(keyword.Trim());
                }
            }
            if (pageFile == null)
            {
                pageFile = new FileStream((location + "\\ArchiveMatches.txt"), FileMode.Append, FileAccess.Write);
                start = 1;
            }
            if (start == 1)
            {
                pnum = 1;
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                string[] s = System.IO.File.ReadAllLines(location + $"\\CDXLinks{pnum}.txt");
                lineCount = s.Where(c => c != null && c != "").Count();
                lineCountInitial = lineCount;
                URLList.Clear();
                MessageBox.Show("Creating threads");
                for (int x = 0; x < 10; x++)
                {
                    int tIndex = x;
                    tasks.Add(Task.Run(async () => await GetAllURLs(tIndex)));
                    await Task.Delay(100);
                }
                tasks.Add(Task.Run(async () => await insertPages()));
                stopwatch.Restart();
                tasks.Add(Task.Run(() => check()));
                await Task.WhenAll(tasks);
                if (exitFlag != 1)
                {
                    MessageBox.Show("Finished");
                    if (archiveResumeFile != null)
                    {
                        archiveResumeFile.Close();
                        archiveResumeFile = null;
                    }
                    if (pageFile != null)
                    {
                        pageFile.Close();
                        pageFile = null;
                    }
                    List<string> filtered = URLList.Where(s => s != null).ToList();
                    if (filtered.Count == 0)
                    {
                        System.IO.File.Delete(location + $"\\CDXLinks{pnum}.txt");
                    }
                    else
                    {
                        System.IO.File.WriteAllLines(Path.Combine(location, $"CDXLinks{pnum}.txt"), filtered);
                    }
                    System.IO.File.AppendAllLines(Path.Combine(location, "ArchiveMatches.txt"), ChangesList.ToList());
                    ChangesList.Clear();
                    pnum++;
                    if (System.IO.File.Exists(location + $"\\CDXLinks{pnum}.txt"))
                    {
                        using (FileStream myfile = new FileStream((location + $"\\CDXLinks{pnum}.txt"), FileMode.Open, FileAccess.Read))
                        using (StreamReader reader = new StreamReader(myfile))
                        {
                            string? line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                try
                                {
                                    int.Parse(line.Substring(0, 1));
                                    URLList.Enqueue(line.Trim());
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                        lineCount = URLList.Count;
                        lineCountInitial = URLList.Count;
                        URLList.Clear();
                        label7.Text = $"Pages Remaining: {lineCount.ToString()}";
                    }
                    else
                    {
                        lineCount = 0;
                        lineCountInitial = 0;
                        URLList.Clear();
                        label7.Text = $"Pages Remaining: {lineCount.ToString()}";
                    }
                }
                else
                {
                    try
                    {
                        listBox6.Items.Remove(listBox6.Items[itemNumber]);
                    }
                    catch
                    {
                        //
                    }
                    button6.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    listBox5.Visible = true;
                    listBox6.Visible = true;
                    label17.Visible = false;
                    return;
                }
            }
            else
            {
                MessageBox.Show("There are No URLs to Scrape");
            }
            if (pageFile != null && exitFlag != 1)
            {
                pageFile.Close();
                pageFile = null;
            }
            try
            {
                listBox6.Items.Remove(listBox6.Items[itemNumber]);
            }
            catch
            {
                //
            }
            button6.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            listBox5.Visible = true;
            listBox6.Visible = true;
            label17.Visible = false;
        }
        private async Task GetAllURLs(int id)
        {
            while (true)
            {
                HttpResponseMessage response;
                if (exitFlag == 1)
                {
                    try
                    {
                        semaphoreSlim.Release();
                    }
                    catch
                    {
                        //
                    }
                    break;
                }
                if (pagesFinished == 1 && lineCount == 0)
                {
                    try
                    {
                        semaphoreSlim.Release();
                    }
                    catch
                    {
                        //
                    }
                    break;
                }
                URLList.TryDequeue(out string? result);
                if (result == null)
                {
                    continue;
                }
                lineCount--;
                this.Invoke((MethodInvoker)delegate
                {
                    label7.Text = $"Pages Remaining: {lineCount}";
                });
                if (result.Substring(result.Length - 11, 11) == "/robots.txt")
                {
                    semaphoreSlim.Release();
                    continue;
                }
                string timestamp = result.Substring(0, 14);
                string URL = result.Substring(15).Trim();
                try
                {
                    response = await client.GetAsync($"https://web.archive.org/web/{timestamp}id_/{URL}");
                }
                catch (HttpRequestException ex) when (ex.InnerException is IOException)
                {
                    try
                    {
                        semaphoreSlim.Release();
                    }
                    catch
                    {
                        //
                    }
                    URLList.Enqueue(result);
                    lineCount++;
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox3.Items.Add($"Error");
                        label7.Text = $"Pages Remaining: {lineCount}";
                    });
                    await Task.Delay(5000);
                    continue;
                }
                catch
                {
                    try
                    {
                        semaphoreSlim.Release();
                    }
                    catch
                    {
                        //
                    }
                    URLList.Enqueue(result);
                    lineCount++;
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox3.Items.Add($"Rate Limit");
                        label7.Text = $"Pages Remaining: {lineCount}";
                    });
                    if (exitFlag != 1)
                    {
                        await Task.Delay(1000 * 120);
                    }
                    continue;
                }
                if (response.IsSuccessStatusCode)
                {
                    string respText;
                    if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                    {   
                        using (var gzip = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                        {
                            using (var reader = new StreamReader(gzip, Encoding.UTF8))
                            {
                                try
                                {
                                    respText = await reader.ReadToEndAsync();
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            respText = await response.Content.ReadAsStringAsync();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            continue;
                        }
                    }
                    foreach (string k in keywords)
                    {
                        if (respText.ToLower().Contains(k.ToLower()))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                listBox4.Items.Add($"Match Found: {k}");
                                totalMatches++;
                                label17.Text = $"Total Matches {totalMatches}";

                            });
                            ChangesList.Enqueue($"{result} {k}");
                        }
                    }
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox3.Items.Add($"Error {(int)response.StatusCode}");
                    });
                    if ((int)response.StatusCode != 400 && (int)response.StatusCode != 404 && (int)response.StatusCode != 403)
                    {
                        try
                        {
                            semaphoreSlim.Release();
                        }
                        catch
                        {
                            //
                        }
                        await Task.Delay(1000 * 30);
                        URLList.Enqueue(result);
                        lineCount++;
                    }
                    else
                    {
                        try
                        {
                            await Task.Delay(1000);
                        }
                        catch
                        {
                            //
                        }
                    }
                    continue;
                }
                try
                {
                    semaphoreSlim.Release();
                }
                catch
                {
                    //
                }
            }
        }
        private async void ClosingForm(object sender, FormClosingEventArgs e)
        {
            if (exitFlag == 2)
            {
                return;
            }
            if (exitFlag != 1)
            {
                exitFlag = 1;
                e.Cancel = true;
                MessageBox.Show("Wait For All Threads To Finish (This May Take a Couple Minutes)");
                await Task.WhenAll(tasks);
                MessageBox.Show("All Threads Are Finished. Once the CDXLinksResume.txt File has Updated, the CDX Scraper Will Close Automatically");
            }
            if (file != null)
            {
                file.Close();
            }
            if (archiveResumeFile != null)
            {
                archiveResumeFile.Close();
            }
            if (resumeFile != null)
            {
                resumeFile.Close();
                var resumeFilePath = Path.Combine(location, "CDXLinksResume.txt");
                List<string> filtered = remainingPages.Where(s => s != null).ToList();
                if (filtered.Count == 0)
                {
                    System.IO.File.Delete(resumeFilePath);
                }
                else
                {
                    System.IO.File.WriteAllLines(resumeFilePath, filtered);
                }
                filtered = PagesToWrite.Where(s => s != null).ToList();
                if (filtered.Count > 0)
                {
                    resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                    FileStream myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                    foreach (string line in filtered)
                    {
                        if (long.TryParse(line.Substring(0,14), out long result))
                        {
                            myfile.Write(Encoding.UTF8.GetBytes(line.Trim() + "\n"));
                            lineCount++;
                            if (lineCount % 100000 == 0)
                            {
                                lineCount = 0;
                                myfile.Close();
                                pnum++;
                                resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                                myfile = new FileStream(resumeFilePath, FileMode.Append, FileAccess.Write);
                            }
                        }
                    }
                    myfile.Close();
                }
            }
            if (pageFile != null)
            {
                pageFile.Close();
                var n = URLList.Count.ToString();
                this.BeginInvoke((MethodInvoker)delegate
                {
                    label7.Text = $"Pages Remaining: {n}";
                });
                var resumeFilePath = Path.Combine(location, $"CDXLinks{pnum}.txt");
                List<string> filtered = URLList.Where(s => s != null).ToList();
                System.IO.File.WriteAllLines(resumeFilePath, filtered);
                MessageBox.Show($"There are {filtered.Count.ToString()} URLs left to check");
                System.IO.File.AppendAllLines(Path.Combine(location, "ArchiveMatches.txt"), ChangesList.ToList());
            }
            exitFlag = 2;
            this.Close();
        }
    }
}
