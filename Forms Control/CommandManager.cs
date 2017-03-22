using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Forms_Control
{
    public class CommandManager
    {
        private Dictionary<string, Object> vars;
        private PuppetForm puppet;
        private JobQueue jobs;
        public Object commandReturn { get; private set; }

        /* Constructor */
        public CommandManager(PuppetForm puppet)
        {
            vars = new Dictionary<string, object>();
            jobs = new JobQueue();
            this.puppet = puppet;
            jobs.start();
        }

        /* Adds a variable to the list of variables */
        public void addVar(string name, Object var) { vars.Add(name, var); }

        /* Returns the value of a variable */
        public Object getVar(string name) { return vars[name]; }

        /* Prints the value of a variable */
        public void printVar(string name) { Console.WriteLine(name + " = " + vars[name].ToString()); }

        /* Deletes the variable from the list of variables */
        public void deleteVar(string name) { vars.Remove(name); }

        /********************************************************************************************
         * Runs the given command, see list of commands below                                       *
         * If printOutput is false, then it will not print anything onto the console.               *
         ********************************************************************************************
         * SET %var% = %value%                                                                      *
         * Makes the variable %var% equal to %value%, creating %var% if necessary.                  *
         ********************************************************************************************
         * GET %var%                                                                                *
         * PRINT %var%                                                                              *
         * Prints the value of %var% or returns an error if it does not exist.                      *
         ********************************************************************************************
         * DELETE %var%                                                                             *
         * Deletes the variable %var%.                                                              *
         ********************************************************************************************
         * EXIT                                                                                     *
         * Closes the application.                                                                  *
         ********************************************************************************************
         * CLEAR                                                                                    *
         * Clears the console screen (ignores printOutput)                                          *
         ********************************************************************************************
         * MOVETO %x% %y% [%speed%] [%side%]                                                        *
         * Translates the given side of the puppet form to the given position with the given speed. *
         * %speed% defaults to 5.0, %side% defaults to "center"                                     *
         ********************************************************************************************
         * GETWINDOWS                                                                               *
         * Prints a list of all of the windows                                                      *
         ********************************************************************************************
         * GETWINDOWTITLE %hwnd%                                                                    *
         * Prints the title of the window                                                           *
         ********************************************************************************************
         * ENABLE %hwnd%                                                                            *
         * Enables the window                                                                       *
         ********************************************************************************************
         * DISABLE %hwnd%                                                                           *
         * Disables the window                                                                      *
         ********************************************************************************************/
        public int runCommand(string command, bool printOutput = true)
        {
            string[] commands = command.ToLower().Split(new[]{' ', '='},  StringSplitOptions.RemoveEmptyEntries);

            if (commands.Length < 1)
            {
                if (printOutput)
                    Console.WriteLine("Error: No command given.");
                return CommandError.Null;
            }

            switch (commands[0])
            {
                // PRINT %var% or GET %var%
                case "print":
                case "get":
                    try
                    {
                        // If there aren't any arguments passed, complain
                        if (commands.Length <= 1)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Get the value of the variable
                        string name = commands[1];
                        if (printOutput)
                            Console.WriteLine(name + " = " + vars[name].ToString());
                        commandReturn = vars[name];
                        return CommandError.Success;
                    }
                    // If the variable could not be found, complain
                    catch (KeyNotFoundException)
                    {
                        if (printOutput)
                            Console.WriteLine("Error: " + commands[1] + " does not exist.");
                        return CommandError.VarDoesNotExist;
                    }
                // SET %var% = %value%
                case "set":
                    {
                        // If there aren't any arguments passed, complain
                        if (commands.Length <= 1)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // If there aren't enough arguments passed, complain
                        string name = commands[1];
                        if (command.Length < 5 + name.Length)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        string value = command.Substring(5 + name.Length);
                        if (value.Contains("= "))
                            value = value.Substring(2);
                        // Set the variable to the value, if that fails, complain and explain why
                        if (setVar(name, value) != CommandError.Success)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: " + commandReturn.ToString() + ".");
                            return CommandError.InvalidArgument;
                        }
                        
                        if (printOutput)
                            Console.WriteLine(name + " = " + vars[name].ToString());
                        return CommandError.Success;
                    }
                // DELETE %var%
                case "delete":
                    try
                    {
                        // If there aren't any arguments passed, complain
                        if (commands.Length <= 1)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Delete the variable
                        string name = commands[1];
                        if (printOutput)
                            Console.WriteLine("Deleted " + name + ".");
                        vars.Remove(name);
                        return CommandError.Success;
                    }
                    // If the variable could not be found, complain
                    catch (KeyNotFoundException)
                    {
                        if (printOutput)
                            Console.WriteLine("Error: " + commands[1] + " does not exist.");
                        return CommandError.VarDoesNotExist;
                    }
                // EXIT
                case "exit":
                    Application.Exit(); // Close the application
                    return CommandError.Success;
                // CLEAR
                case "clear":
                    Console.Clear(); // Clear the console screen
                    return CommandError.Success;
                // MOVETO %x% %y%
                case "moveto":
                    {
                        commands = command.ToLower().Split(new[]{' ', ',', '(', ')', '\"'}, StringSplitOptions.RemoveEmptyEntries);

                        // Check to make sure the number of parameters is 2 or more
                        if (commands.Length <= 2)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Try to parse the 1st and 2nd arguments as integers, if that fails, complain
                        int x = 0, y = 0;
                        if (!(Int32.TryParse(commands[1], out x) && Int32.TryParse(commands[2], out y)))
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Invalid arguments.");
                            return CommandError.InvalidArgument;
                        }
                        // Check if there's a third argument and attempt to parse it as a float, if that fails, parse it as a string
                        float speed = 5.0f;
                        string side = "center";
                        if (commands.Length > 3)
                            if (!float.TryParse(commands[3], out speed))
                                side = commands[3];
                        // Check if there's a fourth argument and attempt to parse it as a float, if that fails, parse it as a string
                        if (commands.Length > 4)
                            if (!float.TryParse(commands[4], out speed))
                                side = commands[4];
                        // If the speed is zero then make it 5.0
                        if (speed == 0.0f)
                            speed = 5.0f;
                        // Move the puppet to the given coordinates
                        if (printOutput)
                            Console.WriteLine("Moving to (" + x.ToString() + ", " + y.ToString() + ")");
                        jobs.add(() => puppet.MoveTo(x, y, speed, side));

                        return CommandError.Success;
                    }
                // GETWINDOWS
                // LISTWINDOWS
                case "getwindows":
                case "listwindows":
                    {
                        IEnumerable<IntPtr> windows = WinController.GetWindows();
                        commandReturn = windows;

                        if (printOutput)
                            foreach(IntPtr hWnd in WinController.GetWindows())
                            {
                                // Get the name of each window
                                string text = WinController.GetWindowText(hWnd);
                                Console.WriteLine(text.PadRight(70, '.') + "." + hWnd.ToString().PadLeft(8, '.'));
                            }

                        return CommandError.Success;
                    }
                // GETWINDOWTITLE %HWND%
                case "getwindowtitle":
                    {
                        // Check for the first parameter
                        if (commands.Length <= 1)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not anough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Parse the first argument as a pointer
                        int ptr;
                        if (!Int32.TryParse(commands[1], out ptr))
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Invalid window handle.");
                            return CommandError.InvalidArgument;
                        }
                        // Get the window's text from the pointer and complain if the window does not exist or has no title
                        commandReturn = WinController.GetWindowText((IntPtr)ptr);
                        if ((string)commandReturn == "")
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Invalid window handle.");
                            return CommandError.InvalidHandle;
                        }
                        if (printOutput)
                            Console.WriteLine((string)commandReturn);

                        return CommandError.Success;
                    }
                // DISABLE %HWND%
                // ENABLE %HWND%
                case "disable":
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
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not anough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Parse the first argument as a pointer
                        int ptr;
                        if (!Int32.TryParse(commands[1], out ptr))
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Invalid window handle.");
                            return CommandError.InvalidArgument;
                        }
                        // Enable the given window and complain if it doesn't work
                        if (!WinController.EnableWindow((IntPtr)ptr, enabled))
                        {
                            if (printOutput)
                                Console.WriteLine("Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message + ".");
                            return CommandError.InvalidHandle;
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
            }

            if (printOutput)
                Console.WriteLine("Error: invalid command.");
            return CommandError.InvalidCommand;
        }

        /* Sets the variable %name% to whatever command was given (could be int, string, evaluation, etc.) */
        private CommandError setVar(string name, string command)
        {
            if (vars.ContainsKey(name))
                vars[name] = command;
            else
                vars.Add(name, command);

            commandReturn = vars[name];
            return CommandError.Success;
        }
    }
}
