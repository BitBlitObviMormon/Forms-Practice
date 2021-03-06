﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Forms_Control
{
    /* Used for EnumWindows callbacks */
    public delegate bool CallBackPtr(IntPtr hWnd, IntPtr lParam);

    /******************************************
     * RECT                                   *
     * Used for storing a window's dimensions *
     ******************************************/
    public struct RECT
    {
        public int Left;    // x position of upper-left corner
        public int Top;     // y position of upper-left corner
        public int Right;   // x position of lower-right corner
        public int Bottom;  // y position of lower-right corner
    }

    /**************************************************************
     * WINDOWS CONTROLLER                                         *
     * A static class that wields the power to manipulate windows *
     **************************************************************/
    public static class WinController
    {
        /*********************************
         * Sends a message to the window *
         *********************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, IntPtr Msg, IntPtr wParam, IntPtr lParam);

        /***********************************************
         * Releases Capture of... I need to look it up *
         * TODO: Get info on ReleaseCapture()          *
         ***********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool ReleaseCapture();

        /*******************************************
         * Minimizes the window (Despite its name) *
         *******************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool CloseWindow(IntPtr hWnd);

        /********************************************************
         * Closes the window (Not to confuse with CloseWindow!) *
         ********************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        /**********************************************
         * Brings the window to the top of the screen *
         **********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        /************************************************************************
         * Moves and resizes the window to the given dimensions and coordinates *
         ************************************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool redraw);

        /**********************************
         * Enables or disables the window *
         **********************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        /***********************************************************
         * Returns whether or not the window is minimized (iconic) *
         ***********************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool IsIconic(IntPtr hWnd);

        /*************************************************
         * Returns whether or not the handle is a window *
         *************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool IsWindow(IntPtr hWnd);

        /************************************************
         * Returns whether or not the window is enabled *
         ************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        /**********************************************************************
         * Returns whether or not the given window has WS_VISIBLE set to true *
         **********************************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /************************************************************************
         * Changes the state of the window (minimized, maximized, hidden, etc.) *
         ************************************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        public static        bool HideWindow(IntPtr hWnd) { return ShowWindow(hWnd, 0); }

        /***********************************
         * Get the currently active window *
         ***********************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        /***************************
         * Get the foremost window *
         ***************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /*********************************************
         * Grabs the window's thread and process ids *
         *********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        public static        IntPtr GetWindowThreadProcessId(IntPtr hWnd) { IntPtr nil = IntPtr.Zero; return GetWindowThreadProcessId(hWnd, out nil); }

        /********************************
         * Get the given window's title *
         ********************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /**********************************************
         * Get the length of the given window's title *
         **********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /***********************************************************
         * Retrieve the handle of a window using the window's name *
         ***********************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static        IntPtr FindWindow(string lpWindowName) { return FindWindow("", lpWindowName); }

        /*****************************************
         * Retrieves the desktop's window handle *
         *****************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();

        /*********************************
         * Calls a method on all windows *
         *********************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(CallBackPtr callPtr, IntPtr lParam);

        /***************************************
         * Calls a method on all child windows *
         ***************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, CallBackPtr callPtr, IntPtr lParam);

        /**********************************************
         * Calls a method on all windows on a desktop *
         **********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, CallBackPtr filter, IntPtr lParam);

        /********************************************
         * Gets the window that currently has focus *
         ********************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr GetFocus();

        /*********************************************************************************************
         * Focuses on the given window and returns the window that previously had focus.             *
         * Returns null if the handle is invalid or we're not attached to the window's message queue *
         * https://msdn.microsoft.com/en-us/library/windows/desktop/ms646312(v=vs.85).aspx           *
         *********************************************************************************************/
        [DllImportAttribute("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        /*******************************************************
         * Returns a string containing the window's title text *
         *******************************************************/
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

        /*********************************************************************
         * Returns a list of all of the windows that pass through the filter *
         *********************************************************************/
        public static IEnumerable<IntPtr> FindWindows(CallBackPtr filter) { return FindWindows(IntPtr.Zero, filter); }
        public static IEnumerable<IntPtr> FindWindows(IntPtr parentHWnd, CallBackPtr filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
            {
                // Only add windows that pass through the filter
                if (filter(hWnd, lParam))
                    windows.Add(hWnd);

                return true;
            }, parentHWnd);

            return windows;
        }

        /****************************************
         * Returns a list of all of the windows *
         ****************************************/
        public static IEnumerable<IntPtr> GetWindows(bool excludeEmptyTitles = true)
        {
            return FindWindows(delegate(IntPtr hWnd, IntPtr lParam)
            {
                // If the window's title is empty and we're told to exclude empty titles then don't return it
                if (GetWindowText(hWnd) == "" && excludeEmptyTitles)
                    return false;
                return true;
            });
        }
    }
}
