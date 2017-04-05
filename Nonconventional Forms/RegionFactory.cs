using System.Drawing;

namespace Nonconventional_Forms
{
    /***********************************************
     * REGION FACTORY                              *
     * Used to construct regions from other shapes *
     ***********************************************/
    public class RegionFactory
    {
        /* Stores the region to output */
        public Region region;

        /***************
         * Constructor *
         ***************/
        public RegionFactory() { clear(); }

        /****************************************
         * Constructs the factory with a region *
         ****************************************/
        public RegionFactory(Region region) { this.region = region; }

        /**********************************
         * Adds a rectangle to the region *
         **********************************/
        public static RegionFactory operator +(RegionFactory factory, Rectangle rectangle) { factory.add(rectangle); return factory; }

        /*******************************************************
         * Adds an image to the region using the alpha channel *
         *******************************************************/
        public static RegionFactory operator +(RegionFactory factory, Bitmap image) { factory.add(image); return factory; }

        /**********************************
         * Adds a rectangle to the region *
         **********************************/
        public void add(Rectangle rectangle) { region.Union(rectangle); }

        /*********************
         * Clears the region *
         *********************/
        public void clear() { region = new Region(new Rectangle(0, 0, 0, 0)); }

        /********************************************************
         * Adds the image to the region using the alpha channel *
         ********************************************************/
        public void add(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            int lastx = 0;
            bool draw = false;

            // For every pixel on the image
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // If we're adding pixels to the region and the read pixel has an alpha of zero then stop adding pixels to the region
                    int alpha = image.GetPixel(x, y).A;
                    if (draw && alpha == 0)
                    {
                        region.Union(new Rectangle(lastx, y, x - lastx, 1));
                        lastx = x;
                        draw = false;
                    }
                    // If we're not adding pixels to the region and found a pixel that has a non-zero alpha, then start adding pixels to the region.
                    else if (!draw && alpha != 0)
                    {
                        lastx = x;
                        draw = true;
                    }
                }

                // If we're adding pixels to the region, do so.
                if (draw)
                {
                    add(new Rectangle(lastx, y, w - lastx, 1));
                    draw = false;
                }

                lastx = 0; // Reset the last set pixel to 0 whenever we shift down a row
            }
        }

        /***************************************************************
         * Adds the image to the region using the specified color mask *
         ***************************************************************/
        public void add(Bitmap image, Color mask)
        {
            int w = image.Width;
            int h = image.Height;
            int lastx = 0;
            bool draw = false;

            // For every pixel on the image
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // If we're adding pixels to the region and the read pixel matches the mask then stop adding pixels to the region
                    Color color = image.GetPixel(x, y);
                    if (draw && color == mask)
                    {
                        region.Union(new Rectangle(lastx, y, x - lastx, 1));
                        lastx = x;
                        draw = false;
                    }
                    // If we're not adding pixels to the region and found a pixel that doesn't match the mask, then start adding pixels to the region.
                    else if (!draw && color != mask)
                    {
                        lastx = x;
                        draw = true;
                    }
                }

                // If we're adding pixels to the region, do so.
                if (draw)
                {
                    add(new Rectangle(lastx, y, w - lastx, 1));
                    draw = false;
                }

                lastx = 0; // Reset the last set pixel to 0 whenever we shift down a row
            }
        }
    }
}
