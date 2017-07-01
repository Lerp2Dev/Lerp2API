using Lerp2API_Docs.Properties;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Lerp2API_Docs
{
    public partial class Form1 : Form
    {
        public static string AppData
        {
            get
            {
                return Environment.GetEnvironmentVariable("AppData");
            }
        }

        public static string folder = Path.Combine(AppData, "Lerp2Dev", "API", "Docs");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            CenterPanel();
            if (!Directory.Exists(folder))
            {
                ThreadStart starter = ExtractDocs;
                starter += WhenReady;
                Thread th = new Thread(starter);
                th.Start();
            }
            else
                WhenReady();
        }

        private void WhenReady()
        {
            panel1.SafeUpdate("Visible", false); // panel1.Visible = false;
            GoHome();
        }

        private void GoHome()
        {
            webBrowser1.SafeUpdate("Url", new Uri(Path.Combine(folder, "html", "index.html")));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            label1.Text = webBrowser1.DocumentTitle;
        }

        private void ExtractDocs()
        {
            Directory.CreateDirectory(folder);
            using (var archive = ZipArchive.Open(Resources.ResourceManager.GetStream("DocsZIP")))
            {
                IEnumerable<ZipArchiveEntry> entries = archive.Entries.Where(entry => !entry.IsDirectory);
                int i = 0, v = entries.Count();
                progressBar1.SafeUpdate("Maximum", v);
                foreach (ZipArchiveEntry entry in entries)
                {
                    entry.WriteToDirectory(folder, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    ++i;
                    progressBar1.SafeUpdate("Value", i);
                    label2.SafeUpdate("Text", string.Format("{0} of {1}", i, v));
                }
            }
        }

        private void CenterPanel()
        {
            panel1.Location = new Point(
                                        this.ClientSize.Width / 2 - panel1.Size.Width / 2,
                                        this.ClientSize.Height / 2 - panel1.Size.Height / 2);
            panel1.Anchor = AnchorStyles.None;
        }
    }

    public static class ControlExtensions
    {
        public static void SafeUpdate<T>(this Control con, string prop, T value)
        {
            if (con.InvokeRequired)
                con.BeginInvoke(new Action<Control, string, T>(SafeUpdate), new object[] { con, prop, value });
            else
                con.SetPropertyValue(prop, value);
        }
    }

    public static class PropertyExtension
    {
        public static void SetPropertyValue(this object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        }
    }
}