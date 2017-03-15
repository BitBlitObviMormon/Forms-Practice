using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonconventional_Forms
{
    class RegionFactory
    {
        public Region region;
        public RegionFactory() { region = new Region(new Rectangle(0, 0, 0, 0)); }
        public RegionFactory(Region region) { this.region = region; }
        public static RegionFactory operator +(RegionFactory factory, Rectangle rectangle) { factory.add(rectangle); return factory; }
        public static RegionFactory operator +(RegionFactory factory, Bitmap image) { factory.add(image); return factory; }
        public void add(Rectangle rectangle) { region.Union(rectangle); }

        public void add(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            int lastx = 0;
            bool draw = false;
            for (int y = 0; y < h; y++)
            {
                draw = false;
                for (int x = 0; x < w; x++)
                {
                    int alpha = image.GetPixel(x, y).A;
                    if (draw && alpha == 0)
                    {
                        add(new Rectangle(lastx, y, x - lastx, 1));
                        lastx = x;
                        draw = false;
                    }
                    else if (!draw && alpha != 0)
                    {
                        lastx = x;
                        draw = true;
                    }
                }
            }
        }

        public void add(Bitmap image, Color mask)
        {
            int w = image.Width;
            int h = image.Height;
            int lastx = 0;
            bool draw = false;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color color = image.GetPixel(x, y);
                    if (draw && color == mask)
                    {
                        try
                        {
                            region.Union(new Rectangle(lastx, y, x - lastx, 1));
                            lastx = x;
                            draw = false;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "\n" + e.StackTrace);
                        }
                    }
                    else if (!draw && color != mask)
                    {
                        lastx = x;
                        draw = true;
                    }
                }

                if (draw)
                {
                    add(new Rectangle(lastx, y, w - lastx, 1));
                    draw = false;
                }

                lastx = 0;
            }
        }
    }
}
