using System;
using System.Collections.Generic;
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

        /*****************************************************************************
         * Runs the given command, see list of commands below                        *
         * If printOutput is false, then it will not print anything onto the console *
         *****************************************************************************
         * SET %var% = %value%                                                       *
         * Makes the variable %var% equal to %value%, creating %var% if necessary    *
         *****************************************************************************
         * GET %var%                                                                 *
         * PRINT %var%                                                               *
         * Prints the value of %var% or returns an error if it does not exist        *
         *****************************************************************************
         * EXIT                                                                      *
         * Closes the application                                                    *
         *****************************************************************************
         * MOVETO %x% %y%                                                            *
         * Translates the puppet form to the given position                          *
         *****************************************************************************/
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
                        if (commands.Length < 2)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        string name = commands[1];
                        if (printOutput)
                            Console.WriteLine(name + " = " + vars[name].ToString());
                        commandReturn = vars[name];
                        return CommandError.Success;
                    }
                    catch (KeyNotFoundException)
                    {
                        if (printOutput)
                            Console.WriteLine(commands[1] + " does not exist");
                        return CommandError.VarDoesNotExist;
                    }
                // SET %var% = %value%
                case "set":
                    {
                        if (commands.Length < 2)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
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
                // EXIT
                case "exit":
                    Application.Exit();
                    return CommandError.Null;
                // MOVETO %x% %y%
                case "moveto":
                    {
                        commands = command.ToLower().Split(new[]{' ', ',', '(', ')'}, StringSplitOptions.RemoveEmptyEntries);

                        // Check to make sure the number of parameters is 3 or more
                        if (commands.Length < 3)
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Not enough arguments.");
                            return CommandError.NotEnoughArguments;
                        }
                        // Try to parse the 2nd and 3rd arguments as integers, if that fails, complain
                        int x = 0, y = 0;
                        if (!(Int32.TryParse(commands[1], out x) && Int32.TryParse(commands[2], out y)))
                        {
                            if (printOutput)
                                Console.WriteLine("Error: Invalid arguments.");
                            return CommandError.InvalidArgument;
                        }

                        // Move the puppet to the given coordinates
                        if (printOutput)
                            Console.WriteLine("Moving to (" + x.ToString() + ", " + y.ToString() + ")");
                        jobs.add(() => puppet.MoveTo(x, y));

                        jobs.start();

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
