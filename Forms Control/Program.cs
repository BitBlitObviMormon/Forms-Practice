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
            PuppetForm form = new PuppetForm();
            Thread consoleThread = new Thread(() => CMain(form));
            consoleThread.IsBackground = true;
            consoleThread.Start();

            Thread bonusThread = new Thread(CBackground);
            bonusThread.IsBackground = true;
            bonusThread.Start();

            // Run the application
            Application.Run(form);
        }

        // The console's main loop
        static void CMain(PuppetForm form)
        {
            Console.Title = "Forms Controller Console";
            Program.commandManager = new CommandManager(form);

            while (true)
            {
                Program.commandManager.runCommand(Console.ReadLine());
            }
        }

        static void CBackground()
        {
            while (true)
            {
                Thread.Sleep(1000);
//                Program.commandManager.runCommand("say I like your name!");
            }
        }
    }
}
