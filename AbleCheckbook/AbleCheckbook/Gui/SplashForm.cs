using AbleCheckbook.Logic;
using AbleLicensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public partial class SplashForm : Form
    {

        /// <summary>
        /// How long to display the splash screen in milliseconds
        /// </summary>
        private const int ShutDownMillis = 5000;

        /// <summary>
        /// Used to shut down this form after the set number of millis. 
        /// </summary>
        private static System.Timers.Timer _delayTimer = null;

        /// <summary>
        /// Static to track this instance for future shutdown.
        /// </summary>
        private static SplashForm _splashForm = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        public SplashForm()
        {
            InitializeComponent();
            _splashForm = this;
            Point screenCenter = this.PointToClient(new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2));
            Point splashCenter = new Point(this.Width / 2, this.Height / 2);
            this.Location = new Point(screenCenter.X - splashCenter.X, screenCenter.Y - splashCenter.Y);
        }

        /// <summary>
        /// Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplashForm_Load(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            textBoxHeading.Text = Strings.Get("Able Strategies AbleCheckbook - See Terms of License");
            textBox2.Text = Strings.Get("Time-Limited Evaluation Copy");
            if (Configuration.Instance.GetIsLicensedVersion())
            {
                textBox2.Text = Strings.Get("Licensed to: ") + Configuration.Instance.SiteDescription;
            }
            int expDays = Activation.Instance.UpdateSiteSettings();
            if (expDays < 0)
            {
                textBox2.Text = Strings.Get("Expired") + " " + Math.Abs(expDays) + Strings.Get(" days");
            }
            else if (expDays < 30)
            {
                textBox2.ForeColor = Color.Red;
                if (expDays < 4)
                {
                    textBox2.Text = textBox2.Text + " !";
                }
            }
            textBox3.Text = Strings.Get("Level: ") + Configuration.Instance.GetUserLevel().ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            textBox4.Text = Strings.Get("Version: ") + version.ToString();
            SetTimer();
        }

        /// <summary>
        /// Set a timer to shut down this splash screen after so many milliseconds.
        /// </summary>
        private static void SetTimer()
        {
            _delayTimer = new System.Timers.Timer(ShutDownMillis);
            _delayTimer.Elapsed += OnTimedEvent1;
            _delayTimer.AutoReset = true;
            _delayTimer.Enabled = true;
        }

        /// <summary>
        /// Handle timer expiration.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent1(Object source, ElapsedEventArgs e)
        {
            _delayTimer.Enabled = false;
            _splashForm.Invoke(new ShutDownDelegate(_splashForm.ShutDown), 0);
        }

        /// <summary>
        /// Delegate used to call ShutDown() on the main GUI thread
        /// </summary>
        /// <param name="dummy">Unused at this time</param>
        private delegate void ShutDownDelegate(int dummy);

        /// <summary>
        /// Shut down this form
        /// </summary>
        /// <param name="dummy">Unused at this time</param>
        private void ShutDown(int dummy)
        { 
            _splashForm.Hide();
            _splashForm.Close();
        }
    }
}
