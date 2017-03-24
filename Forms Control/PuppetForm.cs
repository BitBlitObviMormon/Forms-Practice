using Nonconventional_Forms;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms_Control
{
    public partial class PuppetForm : Form
    {
        /* All of the points for easy reference */
        private int w { get { return Size.Width; } }
        private int h { get { return Size.Height; } }
        private int midw { get { return w / 2; } }
        private int midh { get { return h / 2; } }
        private int x { get { return w + Location.X; } }
        private int y { get { return h + Location.Y; } }
        private int midx { get { return midw + Location.X; } }
        private int midy { get { return midh + Location.Y; } }
        public Point top { get { return new Point(midx, Location.Y); } set { value.Offset(new Point(-midw, 0)); Location = value; } }
        public Point bottom { get { return new Point(midx, y); } set { value.Offset(new Point(-midw, -h)); Location = value; } }
        public Point left { get { return new Point(Location.X, midy); } set { value.Offset(new Point(0, -midh)); Location = value; } }
        public Point right { get { return new Point(x, midy); } set { value.Offset(new Point(-w, -midh)); Location = value; } }
        public Point center { get { return new Point(midx, midy); } set { value.Offset(new Point(-midw, -midh)); Location = value; } }
        public Point topLeft { get { return new Point(Location.X, Location.Y); } set { Location = value; } }
        public Point topRight { get { return new Point(x, Location.Y); } set { value.Offset(new Point(-w, 0)); Location = value; } }
        public Point bottomLeft { get { return new Point(Location.X, y); } set { value.Offset(new Point(0, -h)); Location = value; } }
        public Point bottomRight { get { return new Point(x, y); } set { value.Offset(new Point(-w, -h)); Location = value; } }

        /* The puppet's name */
        public string name;

        /* If this is the form's first draw (for initialization with graphics purposes) */
        private bool firstDraw;

        /* The Puppet Form's image */
        Bitmap image;

        /* How close the target will move towards a point before stopping */
        /* Regardless of what this is set to, it will always move to exactly the point given */
        const float CloseDistance = 1.0f;

        /* Initializes the window */
        public PuppetForm()
        {
            name = "Your totally not shady friend";
            firstDraw = true;

            InitializeComponent();
        }

        /* Paints the window and changes its region */
        private void PuppetForm_Paint(object sender, PaintEventArgs e)
        {
            // Runs when the form is first drawn
            if (firstDraw)
            {
                // Draw the target image onto the form
                firstDraw = false;
                image = new Bitmap("..\\..\\target.png");
                RegionFactory factory = new RegionFactory();
                Size = image.Size;
                factory.add(image, Color.FromArgb(255, 0, 0, 0));
                Region = factory.region;

                // Be creepy and say hi
                Say("Hello and welcome! I am your totally not shady friend, nice to meet you!", 30000, ToolTipIcon.Error);
                Notify("Did I mention that I can also talk to you from the taskbar?", 30000, ToolTipIcon.Error);
            }

            // Draw the form
            e.Graphics.DrawImage(image, 0, 0);
        }

        /* Closes the window when it's double clicked */
        private void PuppetForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        /* Make a speech bubble pop up in the taskbar */
        public void Notify(string title, string text, int time = 30000, ToolTipIcon tipIcon = ToolTipIcon.None, Icon icon = null)
        {
            if (icon == null) icon = SystemIcons.Application;
            notifyBubble.Visible = true;
            notifyBubble.Icon = icon;
            notifyBubble.ShowBalloonTip(time, title, text, tipIcon);
        }
        
        /* Make a speech bubble pop up in the taskbar */
        public void Notify(string text, int time = 30000, ToolTipIcon tipIcon = ToolTipIcon.None, Icon icon = null)
        {
            Notify(name, text, time, tipIcon, icon);
        }

        /* Make the window show something in a speech bubble */
        public void Say(string title, string text, int time = 30000, ToolTipIcon tipIcon = ToolTipIcon.None)
        {
            chatBubble.ToolTipTitle = title;
            chatBubble.ToolTipIcon = tipIcon;

            // TODO: Show popup without stealing focus, perhaps the way is here?
            // http://stackoverflow.com/questions/156046/show-a-form-without-stealing-focus
            Focus(); // Steal focus to show the bubble
            chatBubble.Show(text, this, (w + midw) / 2, -60, time);
        }

        /* Make the window show something in a speech bubble */
        public void Say(string text, int time = 30000, ToolTipIcon tipIcon = ToolTipIcon.None)
        {
            Say(name, text, time, tipIcon);
        }

        /* Returns the square of the distance between two points */
        public static float distanceSquared(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        /* Returns the square of the distance between two points */
        public static float distanceSquared(PointF p1, PointF p2)
        {
            return distanceSquared(p1.X, p1.Y, p2.X, p2.Y);
        }

        /* Returns the square of the distance between two points */
        public static float distanceSquared(float x1, float y1, PointF p2)
        {
            return distanceSquared(x1, y1, p2.X, p2.Y);
        }

        /* Returns the square of the distance between two points */
        public static float distanceSquared(PointF p1, float x2, float y2)
        {
            return distanceSquared(p1.X, p1.Y, x2, y2);
        }

        /* Translates the form over to the specified coordinates. (Meant to be called on a different thread than the gui)    */
        /* Calls the method callFunc when it is not null. callFunc has to execute for LESS THAN 10 milliseconds to not skip. */
        /* If callFunc is executing for longer than 100 milliseconds, it WILL be aborted. Only quick runtimes, please.       */
        public void MoveTo(PointF pos, string side, Func<float, float, bool> callFunc = null, bool printOutput = true)
        {
            MoveTo(pos.X, pos.Y, 5.0f, side, callFunc, printOutput);
        }

        /* Translates the form over to the specified coordinates. (Meant to be called on a different thread than the gui)    */
        /* Calls the method callFunc when it is not null. callFunc has to execute for LESS THAN 10 milliseconds to not skip. */
        /* If callFunc is executing for longer than 100 milliseconds, it WILL be aborted. Only quick runtimes, please.       */
        public void MoveTo(PointF pos, float dr = 5.0f, string side = "closest", Func<float, float, bool> callFunc = null, bool printOutput = true)
        {
            MoveTo(pos.X, pos.Y, dr, side, callFunc, printOutput);
        }

        /* Translates the form over to the specified coordinates. (Meant to be called on a different thread than the gui)    */
        /* Calls the method callFunc when it is not null. callFunc has to execute for LESS THAN 10 milliseconds to not skip. */
        /* If callFunc is executing for longer than 100 milliseconds, it WILL be aborted. Only quick runtimes, please.       */
        public void MoveTo(float x, float y, string size, Func<float, float, bool> callFunc = null, bool printOutput = true)
        {
            MoveTo(x, y, 5.0f, size, callFunc, printOutput);
        }

        /* Translates the form over to the specified coordinates. (Meant to be called on a different thread than the gui)    */
        /* Calls the method callFunc when it is not null. callFunc has to execute for LESS THAN 10 milliseconds to not skip. */
        /* If callFunc is executing for longer than 100 milliseconds, it WILL be aborted. Only quick runtimes, please.       */
        public void MoveTo(float x, float y, float dr = 5.0f, string side = "closest", Func<float, float, bool> callFunc = null, bool printOutput = true)
        {
            // If the user wants us to decide what to use then let's decide for them.
            side = side.ToLower();
            if (side.Contains("closest"))
            {
                side = side.Replace("closest", "");
                if (side == "")
                    side = pickClosest(x, y, "any");
                else
                    side = pickClosest(x, y, side);
            }

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
                        Console.WriteLine("Options are: top, bottom, left, right, center, topLeft, topRight,");
                        Console.WriteLine("   bottomLeft, bottomRight, closest, closestside, closestcorner.");
                    }
                    return;
            }

            // Use this thread to call the callback method later
            int skips = -1;
            Task task = new Task(() => callFunc.Invoke(pos.X, pos.Y));

            // While the target is not yet close enough, move towards it
            float d = dr * 1.41421356237F + CloseDistance;
            while (((pos.X - x) * (pos.X - x) + (pos.Y - y) * (pos.Y - y)) > d * d)
            {
                // Move the window towards the point
                double direction = Math.Atan2(y - pos.Y, x - pos.X);
                pos.Y += (float)(dr * Math.Sin(direction));
                pos.X += (float)(dr * Math.Cos(direction));

                // Update the window's position
                BeginInvoke(new Action(delegate()
                {
                    switch (side.ToLower())
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

                // Invoke the callback method if it exists and isn't currently running or forgotten.
                if (callFunc != null && skips != -2)
                {
                    // If the thread was never started, start it up
                    if (skips == -1)
                    {
                        task.Start();
                        skips = 0;

                        // Check to see if the thread is trustworthy: if it takes too long, forget about it; it'll die eventually.
                        if (!task.Wait(100))
                        {
                            Console.WriteLine("MoveTo: Aborted thread - took too long to respond.");
                            skips = -2; // Thread's now forgotten
                        }
                    }
                    // If the thread is still alive and running then skip a frame
                    else if (task.Status != TaskStatus.RanToCompletion)
                    {
                        // If the thread skipped too many frames, forget about it; the callback method must be untrustworthy; it'll die eventually.
                        skips++;
                        task = task.ContinueWith((Object) => callFunc.Invoke(pos.X, pos.Y));
                        Console.WriteLine("MoveTo: Skipped " + skips.ToString() + (skips == 1 ? " frame." : " frames."));
                        if (skips > 10)
                        {
                            Console.WriteLine("MoveTo: Aborted thread - took too long to respond.");
                            skips = -2; // Thread's now forgotten
                        }
                    }
                    // If the thread finished, start a new one
                    else
                    {
                        task = task.ContinueWith((Object) => callFunc.Invoke(pos.X, pos.Y));
                        skips = 0;
                    }
                }

                // Wait for 10 milliseconds
                Thread.Sleep(10);
            }

            // Update the window to the exact position (now that it's close)
            BeginInvoke(new Action(delegate()
            {
                switch (side.ToLower())
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

            // If the thread is still running, wait for the thread to finish
            if (task != null && skips >= 0)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    task = task.ContinueWith((Object) => callFunc.Invoke(x, y));

                // If the thread still didn't finish, forget about it ever existing, it'll die eventually. :o
                if (!task.Wait(100))
                    Console.WriteLine("MoveTo: Aborted thread - it was taking too long to finish.");
            }
        }

        /* Determine the side, corner, or center that is closest to the given point */
        /* Can take "any", "side", or "corner"                                      */
        public string pickClosest(float x, float y, string type = "any")
        {
            // Variables for remembering which was the closest
            float closestDistance = float.PositiveInfinity;
            float tempDist = 0.0f;
            string decision = type;

            // If we're picking anything, decide if the center is closest
            if (type == "any")
            {
                // Check if the center is closest
                closestDistance = distanceSquared(x, y, center);
                decision = "center";
            }
            // If we're picking sides, decide which side is the closest
            if (type == "decision" || type == "any")
            {
                // Check if the top side is closest
                tempDist = distanceSquared(x, y, top);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "top";
                }
                // Check if the left side is closest
                tempDist = distanceSquared(x, y, left);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "left";
                }
                // Check if the right side is closest
                tempDist = distanceSquared(x, y, right);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "right";
                }
                // Check if the bottom side is closest
                tempDist = distanceSquared(x, y, bottom);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "bottom";
                }
            }
            // If we're picking corners, decide which corner is the closest
            if (type == "corner" || type == "any")
            {
                // Check if the top-left corner is closest
                tempDist = distanceSquared(x, y, topLeft);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "topleft";
                }
                // Check if the top-right corner is closest
                tempDist = distanceSquared(x, y, topRight);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "topright";
                }
                // Check if the bottom-left corner is closest
                tempDist = distanceSquared(x, y, bottomLeft);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "bottomleft";
                }
                // Check if the bottom-right corner is closest
                tempDist = distanceSquared(x, y, bottomRight);
                if (tempDist < closestDistance)
                {
                    closestDistance = tempDist;
                    decision = "bottomright";
                }
            }

            return decision;
        }
    }
}
