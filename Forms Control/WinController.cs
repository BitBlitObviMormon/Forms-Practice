using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Forms_Control
{
    // Used for EnumWindows callbacks
    public delegate bool CallBackPtr(IntPtr hWnd, IntPtr lParam);

    // Used for getting a window's dimensions
    public struct RECT
    {
        public int Left;    // x position of upper-left corner
        public int Top;     // y position of upper-left corner
        public int Right;   // x position of lower-right corner
        public int Bottom;  // y position of lower-right corner
    }

    public static class WinController
    {
        // Sends a message to the window
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern int SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);

        // Releases Capture of... I need to look it up
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool ReleaseCapture();

        // Minimizes the window (Despite its name)
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool CloseWindow(IntPtr hWnd);

        // Closes the window (Not to confuse with CloseWindow!)
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        // Brings the window to the top of the screen
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        // Moves and resizes the window to the dimensions and coordinates
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool redraw);

        // Enables or disables the window
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        // Get the currently active window
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern IntPtr GetActiveWindow();

        // Get the given window's title
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // Get the length of the given window's title
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        // Retrieve the handle of a window using the window's name
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static        IntPtr FindWindow(string lpWindowName) { return FindWindow("", lpWindowName); }

        // Retrieves the desktop's window handle
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern IntPtr GetDesktopWindow();

        // Calls a method on all windows
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool EnumWindows(CallBackPtr callPtr, IntPtr lParam);

        // Calls a method on all child windows
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, CallBackPtr callPtr, IntPtr lParam);

        // Calls a method on all windows on a desktop
        [DllImportAttribute("user32.dll", SetLastError=true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, CallBackPtr filter, IntPtr lParam);

        /* Returns a string containing the window's title text */
        public static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length > 0)
            {
                StringBuilder builder = new StringBuilder(length + 1);
                WinController.GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return "";
        }

        /* Returns a list of all of the windows that pass through the filter */
        public static IEnumerable<IntPtr> FindWindows(CallBackPtr filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
            {
                // Only add windows that pass through the filter
                if (filter(hWnd, lParam))
                {
                    windows.Add(hWnd);
                }

                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /* Returns a list of all of the windows */
        public static IEnumerable<IntPtr> GetWindows(bool excludeEmptyTitles = true)
        {
            return FindWindows(delegate(IntPtr hWnd, IntPtr lParam)
            {
                if (GetWindowText(hWnd) != "")
                    return true;
                return false;
            });
        }
    }
}
