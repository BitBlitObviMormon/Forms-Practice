using System;
using System.Collections.Generic;

namespace Forms_Control
{
    public class CommandManager
    {
        private Dictionary<string, Object> vars;
        public Object commandReturn { get; private set; }

        /* Constructor */
        public CommandManager() { vars = new Dictionary<string,object>(); }

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
         *****************************************************************************/
        public int runCommand(string command, bool printOutput = true)
        {
            string[] commands = command.ToLower().Replace("=", "").Split(new[]{' '},  StringSplitOptions.RemoveEmptyEntries);

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
                                Console.WriteLine(commandReturn.ToString());
                            return CommandError.InvalidArgument;
                        }
                        
                        if (printOutput)
                            Console.WriteLine(name + " = " + vars[name].ToString());
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
