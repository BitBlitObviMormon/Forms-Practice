﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Forms_Control
{
    /*********************************************************************************************
     * COMMAND MANAGER                                                                           *
     * Processes all console commands, manages variables, and runs a job queue for a puppet form *
     *********************************************************************************************/
    public class CommandManager
    {
        /* A container for all of the variables */
        private Dictionary<string, Object> vars;

        /* This form will be dragged around while manipulating other objects and forms */
        private PuppetForm puppet;

        /* The job queue for the puppet form (You should only drag the puppet to one place at a time) */
        private JobQueue jobs;

        /* Used as the return value whenever a command is processed (Because the command itself returns an error code) */
        public Object commandReturn { get; private set; }

        /***************
         * Constructor *
         ***************/
        public CommandManager(PuppetForm puppet)
        {
            // Set up the Command Manager's vars and job queue
            vars = new Dictionary<string, object>();
            jobs = new JobQueue();
            this.puppet = puppet;
            jobs.start();

            // Hook onto the console to dispose of the PuppetForm properly when it closes
            hookConsole(OnConsoleClosing, true);
        }

        /********************************************
         * Adds a variable to the list of variables *
         ********************************************/
        public void addVar(string name, Object var) { vars.Add(name, var); }

        /***********************************
         * Returns the value of a variable *
         ***********************************/
        public Object getVar(string name) { return vars[name]; }

        /********************************
         * Sets the value of a variable *
         ********************************/
        public void setVar(string name, Object var) { vars[name] = var; }

        /**********************************
         * Prints the value of a variable *
         **********************************/
        public void printVar(string name) { Console.WriteLine(name + " = " + vars[name].ToString()); }

        /***************************************************
         * Deletes the variable from the list of variables *
         ***************************************************/
        public void deleteVar(string name) { vars.Remove(name); }

        /*********************************************************
         * Translates escape characters to their string literals *
         *********************************************************/
        public static string translateEscapeSequences(string text)
        {
            return text.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\a", "\a").Replace("\\b", "\b")
                .Replace("\\f", "\f").Replace("\\r", "\r").Replace("\\v", "\v").Replace("\\\"", "\"");
        }

        /*****************************************************************************************************
         * Runs the given command and stores the return value in commandReturn, see list of commands below   *
         * The command itself returns greater than zero if successful and a negative error code on failure   *
         * If printOutput is false, then it will not print anything onto the console.                        *
         *****************************************************************************************************
         * void SET %var% = %value%                                                                          *
         * Makes the variable %var% equal to %value%, creating %var% if necessary.                           *
         *****************************************************************************************************
         * Object GET %var%                                                                                  *
         * Object PRINT %var%                                                                                *
         * Returns the value of %var% or an error if it does not exist.                                      *
         *****************************************************************************************************
         * void DELETE %var%                                                                                 *
         * Deletes the variable %var%.                                                                       *
         *****************************************************************************************************
         * void CLEAR                                                                                        *
         * Deletes all of the variables                                                                      *
         *****************************************************************************************************
         * void EXIT                                                                                         *
         * Closes the application.                                                                           *
         *****************************************************************************************************
         * void CLEARSCREEN                                                                                  *
         * void CLS                                                                                          *
         * Clears the console screen (ignores printOutput)                                                   *
         *****************************************************************************************************
         * void MOVETO %x% %y% [%speed%] [%side%]                                                            *
         * Translates the given side of the puppet form to the given position with the given speed.          *
         * %speed% defaults to 5.0, %side% defaults to "any"                                                 *
         *****************************************************************************************************
         * HWND GETPUPPET                                                                                    *
         * HWND GETPUPPETFORM                                                                                *
         * Prints the to the Puppet Form                                                                     *
         *****************************************************************************************************
         * HWND GETWINDOW                                                                                    *
         * HWND GETACTIVEWINDOW                                                                              *
         * Gets the handle to the currently active window                                                    *
         *****************************************************************************************************
         * IEnumerable<IntPtr> GETWINDOWS                                                                    *
         * IEnumerable<IntPtr> LISTWINDOWS                                                                   *
         * Prints a list of all of the windows                                                               *
         *****************************************************************************************************
         * string GETWINDOWTITLE %hwnd%                                                                      *
         * Prints the title of the window                                                                    *
         *****************************************************************************************************
         * void ENABLE %hwnd%                                                                                *
         * Enables the window                                                                                *
         *****************************************************************************************************
         * void DISABLE %hwnd%                                                                               *
         * Disables the window                                                                               *
         *****************************************************************************************************
         * void BRINGTOTOP %hwnd%                                                                            *
         * Moves the window to the front of the screen, in front of the other windows                        *
         *****************************************************************************************************
         * bool ISENABLED %hwnd%                                                                             *
         * Returns true if the window is enabled                                                             *
         *****************************************************************************************************
         * bool ISMINIMIZED %hwnd%                                                                           *
         * Returns true if the window is minimized                                                           *
         *****************************************************************************************************
         * bool ISVISIBLE %hwnd%                                                                             *
         * Returns true if the window is not flagged as invisible                                            *
         *****************************************************************************************************
         * bool ISWINDOW %hwnd%                                                                              *
         * Returns whether or not the given handle is a valid window                                         *
         *****************************************************************************************************
         * void SAY [%title%] %text% [%time%]                                                                *
         * void TALK [%title%] %text% [%time%]                                                               *
         * Makes the puppet form display a chat bubble with the given text and title for the given duration  *
         *****************************************************************************************************
         * void NOTIFY [%title%] %text% [%time%]                                                             *
         * Makes the puppet form display a notification with the given text and title for the given duration *
         *****************************************************************************************************
         * void SHOW [%hwnd%]                                                                                *
         * Shows the given window (puppet form is the default)                                               *
         *****************************************************************************************************
         * void HIDE [%hwnd%]                                                                                *
         * Hides the given window (puppet form is the default)                                               *
         *****************************************************************************************************/
        public int runCommand(string command, bool printOutput = true)
        {
            if (command == null)
                return CommandError.Null;

            string[] commands = command.ToLower().Split(new[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            commandReturn = null;

            if (commands.Length < 1)
                return printErr(CommandError.NoCommandGiven, printOutput);

            switch (commands[0])
            {
                // PRINT %var% or GET %var%
                case "print":
                case "get":
                    {
                        try
                        {
                            // If there aren't any arguments passed, complain
                            if (commands.Length <= 1)
                                return printErr(CommandError.NotEnoughArguments, printOutput);

                            // Get the value of the variable
                            string name = commands[1];
                            if (printOutput)
                                printVar(name);
                            commandReturn = vars[name];
                            return CommandError.Success;
                        }
                        // If the variable could not be found, complain
                        catch (KeyNotFoundException)
                        {
                            return printErr(CommandError.VarDoesNotExist, printOutput);
                        }
                    }
                // SET %var% = %value%
                case "set":
                    {
                        // If there aren't any arguments passed, complain
                        if (commands.Length <= 1)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // If there aren't enough arguments passed, complain
                        string name = commands[1];
                        if (command.Length < 5 + name.Length)
                            return printErr(CommandError.NotEnoughArguments, printOutput);
                        
                        string value = command.Substring(5 + name.Length);
                        if (value.Contains("= "))
                            value = value.Substring(2);

                        // Set the variable to the value, if that fails, complain and explain why
                        if (set(name, value) != CommandError.Success)
                            return printErr(CommandError.InvalidArgument, printOutput);

                        if (printOutput)
                            printVar(name);
                        return CommandError.Success;
                    }
                // DELETE %var%
                case "delete":
                    {
                        try
                        {
                            // If there aren't any arguments passed, complain
                            if (commands.Length <= 1)
                                return printErr(CommandError.NotEnoughArguments, printOutput);

                            // Delete the variable
                            string name = commands[1];
                            if (printOutput)
                                Console.WriteLine("Deleted " + name + ".");

                            deleteVar(name);
                            return CommandError.Success;
                        }
                        // If the variable could not be found, complain
                        catch (KeyNotFoundException)
                        {
                            return printErr(CommandError.VarDoesNotExist, printOutput);
                        }
                    }
                // CLEAR
                case "clear":
                    {
                        // Clear all of the variables
                        vars.Clear();

                        // Spit output if allowed
                        if (printOutput)
                            Console.WriteLine("All variables cleared.");

                        return CommandError.Success;
                    }
                // EXIT
                case "exit":
                    {
                        // If the puppet is alive, dispose of it properly
                        if (puppet != null)
                            puppet.Invoke(new Action(puppet.Dispose));

                        Environment.Exit(0); // Close the application

                        return CommandError.Success;
                    }
                // CLEARSCREEN
                // CLS
                case "clearscreen":
                case "cls":
                    {
                        Console.Clear(); // Clear the console screen
                        return CommandError.Success;
                    }
                // MOVETO %x% %y% [%speed%] [%side%]
                case "moveto":
                    {
                        // Make sure the puppet is not null
                        if (puppet == null)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: " + CommandError.PuppetNotCreated.ToString() + ".");
                            return CommandError.PuppetNotCreated;
                        }

                        commands = command.ToLower().Split(new[] { ' ', ',', '(', ')', '\"' }, StringSplitOptions.RemoveEmptyEntries);

                        // Check to make sure the number of parameters is 2 or more
                        if (commands.Length <= 2)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // Try to parse the 1st and 2nd arguments as integers, if that fails, complain
                        int x = 0, y = 0;
                        if (!(Int32.TryParse(commands[1], out x) && Int32.TryParse(commands[2], out y)))
                            return printErr(CommandError.InvalidArgument, printOutput);

                        // Check if there's a third argument and attempt to parse it as a float (speed), if that fails, parse it as a string (side)
                        float speed = 5.0f;
                        string side = "closest";
                        if (commands.Length > 3)
                            if (!float.TryParse(commands[3], out speed))
                                side = commands[3];
                        // Check if there's a fourth argument and attempt to parse it as a float (speed), if that fails, parse it as a string (side)
                        if (commands.Length > 4)
                        {
                            if (speed == 0.0f)
                            {
                                if (!float.TryParse(commands[4], out speed))
                                    side = commands[4];
                            }
                            else
                                side = commands[4];
                        }
                        // If the speed is zero then make it 5.0
                        if (speed == 0.0f)
                            speed = 5.0f;
                        // Move the puppet to the given coordinates
                        if (printOutput)
                            Console.WriteLine("Moving to (" + x.ToString() + ", " + y.ToString() + ")");
//                        int i = 0;
                        jobs.add(() => puppet.MoveTo(x, y, speed, side, new Func<float, float, bool>((float px, float py) =>
                        {
//                            if (i++ == 10)
//                            {
//                                Console.WriteLine(new PointF(px, py).ToString());
//                                i = 0;
//                            }
                            return true;
                        })));

                        return CommandError.Success;
                    }
                // GETPUPPET
                case "getpuppet":
                // GETPUPPETWINDOW
                case "getpuppetform":
                    {
                        // Make sure the puppet is not null
                        if (puppet == null)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: " + CommandError.PuppetNotCreated.ToString() + ".");
                            return CommandError.PuppetNotCreated;
                        }

                        // Get the title of the window
                        puppet.Invoke(new Action(() => {
                            commandReturn = puppet.Handle;
                            if (printOutput)
                                Console.WriteLine(puppet.Text + ": " + puppet.Handle.ToString());
                        }));

                        return CommandError.Success;
                    }
                // GETWINDOW
                case "getwindow":
                // GETACTIVEWINDOW
                case "getactivewindow":
                    {
                        IntPtr hWnd = WinController.GetForegroundWindow();

                        // If thw window didn't return a handle, complain
                        if (hWnd == IntPtr.Zero)
                        {
                            int code = Marshal.GetLastWin32Error();
                            if (code != 0)
                            {
                            if (printOutput)
                                Console.WriteLine("Error: " + new Win32Exception(code).Message + ".");
                            return CommandError.InvalidHandle;
                            }
                            else
                            {
                                if (printOutput)
                                    Console.WriteLine("No window is active at this time.");
                                return CommandError.Null;
                            }
                        }

                        // Get the title of the window
                        if (printOutput)
                            Console.WriteLine(WinController.GetWindowText(hWnd) + ": " + hWnd.ToString());

                        commandReturn = hWnd;

                        return CommandError.Success;
                    }
                // GETWINDOWS or LISTWINDOWS
                case "getwindows":
                case "listwindows":
                    {
                        IEnumerable<IntPtr> windows = WinController.GetWindows();
                        commandReturn = windows;

                        if (printOutput)
                            foreach (IntPtr hWnd in WinController.GetWindows())
                            {
                                // Get the name of each window
                                string text = WinController.GetWindowText(hWnd);
                                Console.WriteLine(text.PadRight(70, '.') + "." + hWnd.ToString().PadLeft(8, '.'));
                            }

                        return CommandError.Success;
                    }
                // GETWINDOWTITLE %hwnd%
                case "getwindowtitle":
                    {
                        // Check for the first parameter
                        if (commands.Length <= 1)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // Get the window's text from the pointer and complain if the window does not exist or has no title
                        commandReturn = WinController.GetWindowText(readPtr(commands, printOutput));
                        if ((string)commandReturn == "")
                            return printErr(CommandError.InvalidHandle, printOutput);

                        if (printOutput)
                            Console.WriteLine((string)commandReturn);

                        return CommandError.Success;
                    }
                // DISABLE %hwnd%
                case "disable":
                // ENABLE %hwnd%
                case "enable":
                    {
                        // Decide if the user wants to enable or disable the window
                        bool enabled;
                        if (commands[0] == "disable")
                            enabled = false;
                        else
                            enabled = true;

                        // Check for the first parameter
                        if (commands.Length <= 1)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // Enable the given window and complain if it doesn't work
                        IntPtr ptr = readPtr(commands, printOutput);
                        if (ptr == IntPtr.Zero) return CommandError.InvalidArgument;
                        if (!WinController.EnableWindow(ptr, enabled))
                        {
                            // Check to make sure it didn't work
                            int lasterror = Marshal.GetLastWin32Error();
                            if (WinController.IsWindowEnabled(ptr) != enabled || !WinController.IsWindow(ptr))
                            {
                                if (printOutput)
                                    Console.WriteLine("Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message + ".");
                                return CommandError.InvalidHandle;
                            }
                        }
                        // Print the output
                        if (printOutput)
                        {
                            if (enabled)
                                Console.WriteLine("Enabled " + ptr.ToString() + ".");
                            else
                                Console.WriteLine("Disabled " + ptr.ToString() + ".");
                        }

                        return CommandError.Success;
                    }
                // BRINGTOTOP %hwnd%
                case "bringtotop":
                    {
                        // Check for the first parameter
                        if (commands.Length <= 1)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // Bring the window to the top and complain if it doesn't work
                        IntPtr ptr = readPtr(commands, printOutput);
                        if (ptr == IntPtr.Zero) return CommandError.InvalidHandle;
                        if (!WinController.BringWindowToTop(ptr))
                        {
                            // Get the resulting error
                            int lasterror = Marshal.GetLastWin32Error();
                            if (printOutput)
                                Console.WriteLine("Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message + ".");
                            return CommandError.InvalidHandle;
                        }
                        // Print the output
                        if (printOutput)
                            Console.WriteLine("Brought " + ptr.ToString() + " to top.");

                        return CommandError.Success;
                    }
                // SETFOCUS %hwnd%
                case "setfocus":
                    {
                        // Check for the first parameter
                        if (commands.Length <= 1)
                            return printErr(CommandError.NotEnoughArguments, printOutput);

                        // Give the window focus and complain if it doesn't work
                        IntPtr ptr = readPtr(commands, printOutput);
                        if (ptr == IntPtr.Zero) return CommandError.InvalidHandle;
                        IntPtr lastFocus = WinController.SetFocus(ptr);
                        if (lastFocus == IntPtr.Zero)
                        {
                            // Get the resulting error
                            int lasterror = Marshal.GetLastWin32Error();
                            if (printOutput)
                                Console.WriteLine("Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message + ".");
                            return CommandError.InvalidHandle;
                        }
                        // Print the output
                        if (printOutput)
                            Console.WriteLine("Gave " + ptr.ToString() + " focus. Last window that had focus was " + lastFocus.ToString() + ".");

                        return CommandError.Success;
                    }
                // GETFOCUS
                case "getfocus":
                    {
                        // Get the window that has focus and complain if it doesn't work
                        IntPtr focus = WinController.GetFocus();
                        if (focus == IntPtr.Zero)
                        {
                            // Get the resulting error
                            int lasterror = Marshal.GetLastWin32Error();
                            if (printOutput)
                                Console.WriteLine("Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message + ".");
                            return CommandError.InvalidHandle;
                        }
                        // Print the output
                        if (printOutput)
                            Console.WriteLine(focus.ToString() + " currently has focus.");

                        return CommandError.Success;
                    }
                // ISENABLED %hwnd%
                case "isenabled":
                    {
                        // Interpret the first argument as a pointer and complain if it can't
                        commandReturn = readPtr(commands, printOutput);
                        if ((IntPtr)commandReturn == IntPtr.Zero) return CommandError.InvalidHandle;

                        // Return whether the window is enabled or not
                        commandReturn = WinController.IsWindowEnabled((IntPtr)commandReturn);
                        return printReturnValue(printOutput);
                    }
                // ISMINIMIZED %hwnd%
                case "isminimized":
                    {
                        // Interpret the first argument as a pointer and complain if it can't
                        commandReturn = readPtr(commands, printOutput);
                        if ((IntPtr)commandReturn == IntPtr.Zero) return CommandError.InvalidHandle;

                        // Return whether the window is minimized or not
                        commandReturn = WinController.IsIconic((IntPtr)commandReturn);
                        return printReturnValue(printOutput);
                    }
                // ISVISIBLE %hwnd%
                case "isvisible":
                    {
                        // Interpret the first argument as a pointer and complain if it can't
                        commandReturn = readPtr(commands, printOutput);
                        if ((IntPtr)commandReturn == IntPtr.Zero) return CommandError.InvalidHandle;

                        // Return whether the window is visible or not
                        commandReturn = WinController.IsWindowVisible((IntPtr)commandReturn);
                        return printReturnValue(printOutput);
                    }
                // ISWINDOW %hwnd%
                case "iswindow":
                    {
                        // Interpret the first argument as a pointer and complain if it can't
                        commandReturn = readPtr(commands, printOutput);
                        if ((IntPtr)commandReturn == IntPtr.Zero) return CommandError.InvalidHandle;

                        // Return whether the handle is a window or not
                        commandReturn = WinController.IsWindow((IntPtr)commandReturn);
                        return printReturnValue(printOutput);
                    }
                // SAY [%title%] %text% [%time%] or TALK [%title%] %text% [%time%]
                // TODO: Implement %title% and %time%
                case "say":
                    {
                        commandReturn = 4;
                        goto case "talk";
                    }
                case "talk":
                    {
                        if (commandReturn == null)
                            commandReturn = 5;

                        if (puppet == null)
                            return printErr(CommandError.PuppetNotCreated, printOutput);

                        string text = translateEscapeSequences(command.Substring((int)commandReturn));
                        puppet.BeginInvoke(new Action(() => { puppet.Say(text); }));

                        return CommandError.Success;
                    }
                // NOTIFY [%title%] %text% [%time%]
                // TODO: Implement %title% and %time%
                case "notify":
                    {
                        if (puppet == null)
                            return printErr(CommandError.PuppetNotCreated, printOutput);

                        string text = translateEscapeSequences(command.Substring(7));
                        puppet.BeginInvoke(new Action(() => { puppet.Notify(text); }));

                        return CommandError.Success;
                    }
                // SHOW [%hwnd%]
                // TODO: Implement hwnd
                case "show":
                    {
                        if (puppet == null)
                            new Thread(() => { Application.Run(puppet = new PuppetForm()); }).Start();
                        else
                            puppet.BeginInvoke(new Action(() => { puppet.Visible = true; }));
                        return CommandError.Success;
                    }
                // HIDE [%hwnd%]
                // TODO: Implement hwnd
                case "hide":
                    {
                        if (puppet == null)
                            return printErr(CommandError.PuppetNotCreated, printOutput);

                        puppet.BeginInvoke(new Action(() => { puppet.Visible = false; }));
                        return CommandError.Success;
                    }
            }

            return printErr(CommandError.InvalidCommand, printOutput);
        }

        /****************************************************
         * Prints the command if printOutput is set to true *
         ****************************************************/
        private CommandError printErr(CommandError err, bool printOutput)
        {
            if (printOutput)
                Console.WriteLine("Error: " + err.ToString() + ".");
            return err;
        }

        /***************************************************************************************************
         * Sets the variable %name% to whatever command was given (could be int, string, evaluation, etc.) *
         ***************************************************************************************************/
        private CommandError set(string name, string command)
        {
            if (vars.ContainsKey(name))
                setVar(name, command);
            else
                addVar(name, command);

            commandReturn = vars[name];
            return CommandError.Success;
        }

        /*************************************************************************
         * Reads the given pointer and outputs error data if printOutput is true *
         *************************************************************************/
        private static IntPtr readPtr(string[] commands, bool printOutput)
        {
            int ptr;
            if (!Int32.TryParse(commands[1], out ptr))
            {
                if (printOutput)
                    Console.WriteLine("Error: Invalid window handle.");
                return IntPtr.Zero;
            }
            return new IntPtr(ptr);
        }

        /*****************************************************************
         * Spits out the command return and returns CommandError.Success *
         *****************************************************************/
        private CommandError printReturnValue(bool printOutput)
        {
            // If we're allowed to print the variable, do so
            if (printOutput)
                Console.WriteLine(commandReturn.ToString());

            return CommandError.Success;
        }

        /**********************************************************************************************
         * Event that's called when the console is closing unexpectedly or by the user clicking the X *
         **********************************************************************************************/
        private void OnConsoleClosing(object sender, CtrlSigEventArgs args)
        {
            switch (args.type)
            {
                // The console caught a CTRL-BREAK or CTRL-C signal
                case CtrlSigType.CTRL_BREAK_EVENT:
                case CtrlSigType.CTRL_C_EVENT:
                    Console.WriteLine("Type exit to quit.");
                    return;
                // The console is closing unexpectedly or by user interaction
                case CtrlSigType.CTRL_CLOSE_EVENT:
                case CtrlSigType.CTRL_LOGOFF_EVENT:
                case CtrlSigType.CTRL_SHUTDOWN_EVENT:
                default:
                    // If the puppet is alive, dispose of it properly
                    if (puppet != null)
                    {
                        if (!puppet.IsDisposed)
                            puppet.Invoke(new Action(puppet.Dispose));
                    }

                    return;
            }
        }

        /*****************************************************
         * Hooks onto the console to capture control signals *
         *****************************************************/
        private void hookConsole(Action<object, CtrlSigEventArgs> func, bool cancelBreaks = false)
        {
            // Disable default CTRL-C and CTRL-BREAK behaviour if we want to cancel them
            if (cancelBreaks)
                Console.CancelKeyPress += new ConsoleCancelEventHandler((object sender, ConsoleCancelEventArgs args) => { args.Cancel = true; });

            // Attach the hook
            new CtrlSigEvent(OnConsoleClosing);
        }
    }
}
