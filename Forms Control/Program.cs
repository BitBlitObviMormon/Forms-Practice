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
            // Start the console program
            Thread consoleThread = new Thread(CMain);
            consoleThread.IsBackground = true;
            consoleThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PuppetForm());
        }

        // The console's main loop
        static void CMain()
        {
            Console.Title = "Forms Controller Console";
            Program.commandManager = new CommandManager();

            while (true)
            {
                Program.commandManager.runCommand(Console.ReadLine());
            }
        }
    }
}
