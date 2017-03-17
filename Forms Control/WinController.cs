using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nonconventional_Forms
{
    // Used for EnumWindows callbacks
    public delegate bool CallBackPtr(IntPtr hWnd, IntPtr lParam);

    public static class WinController
    {
        // Sends a message to the window
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);

        // Releases Capture of... I need to look it up
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // Minimizes the window (Despite its name)
        [DllImportAttribute("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        // Closes the window (Not to confuse with CloseWindow!)
        [DllImportAttribute("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        // Brings the window to the top of the screen
        [DllImportAttribute("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        // Moves and resizes the window to the dimensions and coordinates
        [DllImportAttribute("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool redraw);

        // Enables or disables the window
        [DllImportAttribute("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        // Get the currently active window
        [DllImportAttribute("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        // Retrieve the handle of a window using the window's name
        [DllImportAttribute("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static IntPtr FindWindow(string lpWindowName) { return FindWindow("", lpWindowName); }

        // Retrieves the desktop's window handle
        [DllImportAttribute("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        // Calls a method on all windows
        [DllImportAttribute("user32.dll")]
        public static extern bool EnumWindows(CallBackPtr callPtr, IntPtr lParam);

        // Calls a method on all child windows
        [DllImportAttribute("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, CallBackPtr callPtr, IntPtr lParam);

        // Calls a method on all windows on a desktop
        [DllImportAttribute("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, CallBackPtr callPtr, IntPtr lParam);

    }
}
