using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonconventional_Forms
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private Bitmap image;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            // Load the image
            image = new Bitmap("..\\..\\smiley.png");
            InitializeComponent(); // Start up the form

            HandleCreated += change; // Change the shape of the form once it is created
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /* Changes the shape of the form every frame */
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Does nothing right now
        }

        /* Changes the shape of the form to the image */
        private void change(object sender, EventArgs e)
        {
            Debug.WriteLine("Wrote stuff");
            RegionFactory factory = new RegionFactory();
            factory.add(image, Color.FromArgb(255, 0, 0, 0));
            Region = factory.region;
            RectangleF rectangle = factory.region.GetBounds(CreateGraphics());
            Width = (int)rectangle.Width;
            Height = (int)rectangle.Height;
        }

        /* Closes the form when it is right clicked twice */
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}
