using System;
using System.Threading;
using System.Windows.Forms;

namespace Forms_Control
{
    internal static class Program
    {
        public static CommandManager commandManager;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Prep the windows forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the console program
            CMain();
        }

        // The console's main loop
        static void CMain(PuppetForm form = null)
        {
            Console.Title = "Forms Controller Console";
            Program.commandManager = new CommandManager(form);
            
            // Tell the user how to make the form show
            Console.WriteLine("Thank you for using the Forms Control program.\nTo begin type show in the console to start the main puppet form.");

            while (true)
            {
                Program.commandManager.runCommand(Console.ReadLine());
            }
        }
    }
}
