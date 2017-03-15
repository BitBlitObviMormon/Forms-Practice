using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Nonconventional_Forms
{
    public partial class Form1 : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private const int startFrame = 0;
        private const int tickLength = 100;
        private const bool bounce = true;
        private int direction;

        private Timer timer;

        private List<Bitmap> images;
        private List<Region> regions;
        private int frame;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /* Initializes the form */
        public Form1()
        {
            // Load all of the frames for the animation
            loadFrames(120, 120);
            frame = startFrame;
            direction = 1;

            // Start up the form
            InitializeComponent();

            // Update the region of the form every tick
            timer = new Timer();
            timer.Interval = tickLength;
            timer.Enabled = true;
            timer.Tick += new EventHandler(
            delegate(object sender, EventArgs e)
            {
                update();
            });
        }

        /* Updates the smiley face gui */
        private void update()
        {
            // Advance the frame by 1
            frame += direction;
            if (frame >= regions.Count)
            {
                if (bounce)
                {
                    direction = -1;
                    frame = regions.Count + direction;
                }
                else
                    frame = 0;
            }
            else if (frame < 0)
            {
                if (bounce)
                {
                    direction = 1;
                    frame = direction;
                }
                else
                    frame = regions.Count - 1;
            }

            // Draw the frame
            Region = regions[frame].Clone();
            CreateGraphics().DrawImage(images[frame], 0, 0);
        }

        /* Loads all of the images for the animation */
        private void loadFrames(int frameWidth, int frameHeight)
        {
            images = new List<Bitmap>();
            regions = new List<Region>();
            Bitmap img = new Bitmap("..\\..\\animSmiley.png");

            // Assuming the image strip is horizontal, load the images
            try
            {
                for (int x = 0; x < img.Width; x += frameWidth)
                {
                    Rectangle rect = new Rectangle(x, 0, frameWidth, frameHeight);
                    images.Add(img.Clone(rect, PixelFormat.DontCare));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }

            for (int i = 0; i < images.Count; i++)
            {
                RegionFactory factory = new RegionFactory();
                factory.add(images[i], Color.FromArgb(255, 0, 0, 0));
                regions.Add(factory.region);
            }
        }

        /* Drags the form when it is clicked */
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /* Closes the form when it is right clicked twice */
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
            Close();
        }

        /* Turns off the timer when the form closes */
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Enabled = false;
        }

        /* Changes the shape of the form once it is loaded */
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Region = regions[frame].Clone();
            e.Graphics.DrawImage(images[frame], 0, 0);
        }
    }
}
