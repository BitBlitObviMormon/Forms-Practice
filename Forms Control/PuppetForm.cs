using Nonconventional_Forms;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Forms_Control
{
    public partial class PuppetForm : Form
    {
        /* All of the points for easy reference */
        private int w            { get { return Size.Width;            } }
        private int h            { get { return Size.Height;           } }
        private int midw         { get { return w / 2;                 } }
        private int midh         { get { return h / 2;                 } }
        private int x            { get { return w    + Location.X;     } }
        private int y            { get { return h    + Location.Y;     } }
        private int midx         { get { return midw + Location.X;     } }
        private int midy         { get { return midh + Location.Y;     } }
        public Point top         { get { return new Point(midx, Location.Y);        } set { value.Offset(new Point(-midw, 0));     Location = value; } }
        public Point bottom      { get { return new Point(midx, y);                 } set { value.Offset(new Point(-midw, -h));    Location = value; } }
        public Point left        { get { return new Point(Location.Y, midy);        } set { value.Offset(new Point(0, -midh));     Location = value; } }
        public Point right       { get { return new Point(x, midy);                 } set { value.Offset(new Point(-w, -midh));    Location = value; } }
        public Point center      { get { return new Point(midx, midy);              } set { value.Offset(new Point(-midw, -midh)); Location = value; } }
        public Point topLeft     { get { return new Point(Location.Y, Location.Y);  } set {                                        Location = value; } }
        public Point topRight    { get { return new Point(x, Location.Y);           } set { value.Offset(new Point(-w, 0));        Location = value; } }
        public Point bottomLeft  { get { return new Point(Location.X, y);           } set { value.Offset(new Point(0, -h));        Location = value; } }
        public Point bottomRight { get { return new Point(x, y);                    } set { value.Offset(new Point(-w, -h));       Location = value; } }

        /* How close the target will move towards a point before stopping */
        /* Regardless of what this is set to, it will always move to exactly the point given */
        const float CloseDistance = 1.0f;
        
        /* Initializes the window */
        public PuppetForm()
        {
            InitializeComponent();
        }

        /* Paints the window and changes its region */
        private void PuppetForm_Paint(object sender, PaintEventArgs e)
        {
            // Draw the target image onto the form
            RegionFactory factory = new RegionFactory();
            Bitmap image = new Bitmap("..\\..\\target.png");
            Size = image.Size;
            factory.add(image, Color.FromArgb(255, 0, 0, 0));
            Region = factory.region;
            e.Graphics.DrawImage(image, 0, 0);
        }

        /* Closes the window when it's double clicked */
        private void PuppetForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        /* Translates the form over to the specified coordinates. (Meant to be called on a different thread) */
        public void MoveTo(float x, float y, float dr = 5.0f, string side = "center", bool printOutput = true)
        {
            // Set up the position
            PointF pos = new PointF();
            switch (side.ToLower())
            {
                case "top":
                    pos = top;
                    break;
                case "bottom":
                    pos = bottom;
                    break;
                case "left":
                    pos = left;
                    break;
                case "right":
                    pos = right;
                    break;
                case "center":
                    pos = center;
                    break;
                case "topleft":
                    pos = topLeft;
                    break;
                case "topright":
                    pos = topRight;
                    break;
                case "bottomleft":
                    pos = bottomLeft;
                    break;
                case "bottomright":
                    pos = bottomRight;
                    break;
                default:
                    if (printOutput)
                    {
                        Console.WriteLine("Invalid side: " + side + ".");
                        Console.WriteLine("Options are top, bottom, left, right, center, topLeft, topRight, bottomLeft, bottomRight.");
                    }
                    return;
            }

            float d = dr * 1.41421356237F + CloseDistance;
            while (((pos.X - x) * (pos.X - x) + (pos.Y - y) * (pos.Y - y)) > d * d)
            {
                Thread.Sleep(10);

                // Move the window towards the point
                double direction = Math.Atan2(y - pos.Y, x - pos.X);
                pos.Y += (float)(dr * Math.Sin(direction));
                pos.X += (float)(dr * Math.Cos(direction));

                // Update the window's position
                Invoke(new Action(delegate()
                {
                    switch(side.ToLower())
                    {
                        case "top":
                            top = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "bottom":
                            bottom = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "left":
                            left = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "right":
                            right = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "center":
                            center = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "topleft":
                            topLeft = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "topright":
                            topRight = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "bottomleft":
                            bottomLeft = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                        case "bottomright":
                            bottomRight = Point.Round(new PointF(pos.X, pos.Y));
                            break;
                    }
                }));
            }

            // Update the window to the exact position (now that it's close)
            Invoke(new Action(delegate()
            {
                switch(side.ToLower())
                {
                    case "top":
                        top = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + top.ToString());
                        break;
                    case "bottom":
                        bottom = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + bottom.ToString());
                        break;
                    case "left":
                        left = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + left.ToString());
                        break;
                    case "right":
                        right = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + right.ToString());
                        break;
                    case "center":
                        center = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + center.ToString());
                        break;
                    case "topleft":
                        topLeft = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + topLeft.ToString());
                        break;
                    case "topright":
                        topRight = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + topRight.ToString());
                        break;
                    case "bottomleft":
                        bottomLeft = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + bottomLeft.ToString());
                        break;
                    case "bottomright":
                        bottomRight = Point.Round(new PointF(x, y));
                        if (printOutput)
							Console.WriteLine("Moved to " + bottomRight.ToString());
                        break;
                }
            }));
        }
    }
}
