using Lerp2API_Docs.Properties;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;
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
        { //Mañana le meto la barra de progreso.
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                using (var archive = RarArchive.Open(new MemoryStream(Resources.Lerp2API)))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        entry.WriteToDirectory(folder, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
            GoHome();
        }

        private void GoHome()
        {
            webBrowser1.Url = new Uri(Path.Combine(folder, "html", "index.html"));
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
    }
}