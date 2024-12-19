using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kakarot
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private System.Windows.Forms.Timer fadeTimer;
        private float opacityIncrement;
        private bool fadeIn;
        // Variables for dragging the form
        private bool isDragging = false;
        private Point dragStartPoint;
        public void SplashScreen()
        {
            // Set the form properties for the splash screen.
            // this.StartPosition = FormStartPosition.CenterScreen;
            this.Opacity = 0; // Start fully transparent.
                              // Load the splash image from resources.
                              // var resourceManager = new ResourceManager("Form1.Resources", Assembly.GetExecutingAssembly());
            var splashImage = Resources.kakarot21;
            splashImage.MakeTransparent(Color.White);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Initialize the fade-in/fade-out timer.
            fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 100; // Adjust for smoother or faster animation.
            fadeTimer.Tick += FadeTimer_Tick;
            this.MouseDown += SplashScreen_MouseDown;
            this.MouseMove += SplashScreen_MouseMove;
            this.MouseUp += SplashScreen_MouseUp;
            // Start fade-in.
            opacityIncrement = 0.10f; // Adjust for smoother or faster fade.
            fadeIn = true;
            fadeTimer.Start();
        }
        private void SplashScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = new Point(e.X, e.Y);
            }
        }

        private void SplashScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newPosition = this.Location;
                newPosition.X += e.X - dragStartPoint.X;
                newPosition.Y += e.Y - dragStartPoint.Y;
                this.Location = newPosition;
            }
        }

        private void SplashScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadeIn)
            {
                this.Opacity += opacityIncrement;
                if (this.Opacity >= 1)
                {
                    fadeTimer.Stop();
                    // Hold the splash screen for a moment before fading out.
                    System.Windows.Forms.Timer holdTimer = new System.Windows.Forms.Timer();
                    holdTimer.Interval = 2000; // Adjust hold time as needed.
                    holdTimer.Tick += (s, args) =>
                    {
                        holdTimer.Stop();
                        fadeIn = false;
                        fadeTimer.Start();
                    };
                    holdTimer.Start();
                }
            }
            else
            {
                this.Opacity -= opacityIncrement;
                if (this.Opacity <= 0)
                {
                    fadeTimer.Stop();
                    this.Close();
                }
            }
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            SplashScreen();
        }

        private void Splash_Click(object sender, EventArgs e)
        {
            this.Close();
            fadeTimer.Dispose();
        }
    }
}
