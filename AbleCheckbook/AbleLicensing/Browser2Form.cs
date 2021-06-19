using System;
using System.Threading;
using System.Windows.Forms;

namespace AbleLicensing
{
    public partial class Browser2Form : Form
    {

        private string _homeUrl = "";

        private string _searchUrlBase = "";

        /// <summary>
        /// Web browser
        /// </summary>
        /// <param name="title">Title of window</param>
        /// <param name="homeUrl">Initial URL</param>
        /// <param name="searchUrlBase">Search prefix, typically ending in a %20 or q= or +</param>
        /// <param name="bounds">optional bounds for the form</param>
        public Browser2Form(string title, string homeUrl, string searchUrlBase, Form bounds = null)
        {
            _homeUrl = homeUrl;
            _searchUrlBase = searchUrlBase;
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            this.Text = title;
            if(bounds != null)
            {
                this.Bounds = bounds.Bounds;
            }
        }

        public void ReShow(string searchPattern = null)
        {
            webBrowser1.Stop();
            Thread.Sleep(1000);
            webBrowser1.Navigate(searchPattern == null ? _homeUrl : _searchUrlBase + searchPattern);
            textBoxSearchPattern.Text = searchPattern == null ? "" : searchPattern;
            this.Show();
            this.BringToFront();
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {
            buttonBack.Text = "←  Back";
            buttonHome.Text = "⌂  Home";
            buttonSearch.Text = "Ꙭ  Search";
        }

        private void textBoxSearchPattern_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            buttonSearch_Click(null, null);
        }

        private void FitToPanel()
        {
            if(webBrowser1.Document == null || webBrowser1.Document.Window == null)
            {
                return;
            }
            //int pageWidth = webBrowser1.Document.Window.Size.Width;
            //int viewWidth = webBrowser1.ClientSize.Width;
            //double scale = viewWidth / pageWidth;
            // todo
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(_homeUrl);
            textBoxSearchPattern.Text = "";
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
            textBoxSearchPattern.Text = "";
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if(textBoxSearchPattern.Text.Trim().Length < 1 || _searchUrlBase == null || _searchUrlBase.Length < 1)
            {
                return;
            }
            string url = textBoxSearchPattern.Text.Trim();
            if(!url.Contains("www.") && !url.StartsWith("http") || url.Contains(" "))
            {
                url = _searchUrlBase + textBoxSearchPattern.Text.Trim();
            }
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_Resize(object sender, EventArgs e)
        {
            FitToPanel();
        }

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if(e.CurrentProgress >= e.MaximumProgress)
            {
                FitToPanel();
            }
        }
    }

}
