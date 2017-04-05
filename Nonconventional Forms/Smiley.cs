using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Nonconventional_Forms
{
    public partial class Smiley : Form
    {
        /* Constants or events that are needed for global hooks */
        private IMouseEvents hook;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        /* Constants that are used by the game */
        private const int startFrame = 0;
        private const int frameTickLength = 100;
        private const int gameTickLength = 10;
        private const float closeDistance = 2.0F;
        private const bool bounce = true;

        /* Used for the window movement */
        Point mousePos;
        Point midPos;
        PointF windowPos;
        private float acceleration = 0.01F;
        private float speed = 0.0F;

        /* Used for different ticks */
        private Timer frameTimer = new Timer();
        private Timer tickTimer = new Timer();

        /* Used for the animation */
        private List<Bitmap> images;
        private List<Region> regions;
        private int frame;
        private int direction = 1;

        /************************
         * Initializes the form *
         ************************/
        public Smiley()
        {
            // Load all of the frames for the animation
            loadFrames(120, 120);
            frame = startFrame;

            // Start up the form
            InitializeComponent();
            HandleCreated += Form1_HandleCreated;

            // Set up the hook
            hook = Hook.GlobalEvents();
            hook.MouseMove += hook_MouseMove;

            // Update the region of the form every tick
            frameTimer.Interval = frameTickLength;
            frameTimer.Enabled = true;
            frameTimer.Tick += frameTick;

            // Update the game every tick
            tickTimer.Interval = gameTickLength;
            tickTimer.Enabled = true;
            tickTimer.Tick += gameTick;
        }

        /************************************************************************
         * Gets the position of the window and mouse when the window is created *
         ************************************************************************/
        void Form1_HandleCreated(object sender, EventArgs e)
        {
            windowPos.X = Location.X;
            windowPos.Y = Location.Y;
            mousePos.X = Cursor.Position.X;
            mousePos.Y = Cursor.Position.Y;
        }

        /*********************************************
         * Captures the global position of the mouse *
         *********************************************/
        private void hook_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos.X = e.X;
            mousePos.Y = e.Y;
        }

        /************************
         * Runs an in-game tick *
         ************************/
        private void gameTick(object sender, EventArgs e)
        {
            // Calculate and rename all of the variables to something smaller and easier to read
            float x1 = mousePos.X;
            float y1 = mousePos.Y;
            float x2 = windowPos.X;
            float y2 = windowPos.Y;
            float d = speed * 1.41421356237F + closeDistance;

            // If the distance between the smiley and the cursor is greater than the minimum distance then move closer
            if (((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) > d * d)
            {
                //Accelerate the smiley a bit
                speed += acceleration;

                // Move the window 1 unit towards the mouse using trigonometry
                double direction = Math.Atan2(mousePos.Y - windowPos.Y, mousePos.X - windowPos.X);
                windowPos.Y += (float)(speed * Math.Sin(direction));
                windowPos.X += (float)(speed * Math.Cos(direction));

                // Update the window position
                Location = Point.Round(new PointF(windowPos.X - midPos.X, windowPos.Y - midPos.Y));
            }
            // If the smiley is close enough then stop moving
            else
                speed = 0.0F;
        }

        /*******************************
         * Updates the smiley face gui *
         *******************************/
        private void frameTick(object sender, EventArgs e)
        {
            // Advance the frame by 1
            frame += direction;
            if (frame >= regions.Count)
            {
                // If the animation is supposed to bounce then play it backwards
                if (bounce)
                {
                    direction = -1;
                    frame = regions.Count + direction;
                }
                // If the animation does not bounce, loop back to the beginning
                else
                    frame = 0;
            }
            else if (frame < 0)
            {
                // If the animation is supposed to bounce then play it forwards again
                if (bounce)
                {
                    direction = 1;
                    frame = direction;
                }
                // If the animation does not bounce, loop back to the end
                else
                    frame = regions.Count - 1;
            }

            // Draw the frame
            Region = regions[frame].Clone();
            CreateGraphics().DrawImage(images[frame], 0, 0);
        }

        /*********************************************
         * Loads all of the images for the animation *
         *********************************************/
        private void loadFrames(int frameWidth, int frameHeight)
        {
            images = new List<Bitmap>();
            regions = new List<Region>();
            Bitmap img = new Bitmap("..\\..\\animSmiley.png");

            // Assuming the image strip is horizontal, load the images
            try
            {
                // Load every image in the strip
                for (int x = 0; x < img.Width; x += frameWidth)
                {
                    Rectangle rect = new Rectangle(x, 0, frameWidth, frameHeight);
                    images.Add(img.Clone(rect, PixelFormat.DontCare));
                }
            }
            // Make a popup window for every unexpected exception that shows up
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }

            // Calculate the regions for all of the images now (rather than do it in realtime)
            for (int i = 0; i < images.Count; i++)
            {
                RegionFactory factory = new RegionFactory();
                factory.add(images[i], Color.FromArgb(255, 0, 0, 0));
                regions.Add(factory.region);
            }

            // Calculate the center point of the smiley
            midPos.X = images[0].Width / 2;
            midPos.Y = images[0].Height / 2;
        }

        /**************************************************
         * Closes the form when it is right clicked twice *
         **************************************************/
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e) { Close(); }

        /*********************************************
         * Turns off the timers when the form closes *
         *********************************************/
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Disable the timers
            frameTimer.Enabled = false;
            tickTimer.Enabled = false;
        }

        /***************************************************
         * Changes the shape of the form once it is loaded *
         ***************************************************/
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the smiley
            Region = regions[frame].Clone();
            e.Graphics.DrawImage(images[frame], 0, 0);
        }
    }
}
