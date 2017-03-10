using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gma.System.MouseKeyHook;

namespace Screensaver
{
    public partial class MainForm : Form
    {
        public IKeyboardMouseEvents globalHook;

        /*************************
         * Initialize the window *
         *************************/
        public MainForm()
        {
            //Show the window and start the screen saver
            Start();

            //Initialize the global mouse and keyboard hooks
            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += Close;
            globalHook.MouseDown += Close;
            globalHook.MouseMove += Close;
            globalHook.MouseWheel += Close;
        }

        /**************************
         * Start the screen saver *
         **************************/
        private void Start()
        {
            //Export the screen saver to a file so that it can be run more easily
            string exe = Path.Combine(Directory.GetCurrentDirectory(), "Temp" + Process.GetCurrentProcess().Handle.ToString() + ".exe");
            using (FileStream fout = new FileStream(exe, FileMode.CreateNew, FileAccess.Write))
            {
                byte[] bytes = Properties.Resources.Fireworks;
                fout.Write(bytes, 0, bytes.Length);
            }

            //Startup the screen saver
            InitializeComponent();
            Cursor.Hide();
            Process.Start(exe, "/p " + this.Handle.ToString());

            //On a different thread, check if the screen saver stops running
            Thread thread = new Thread(() => DeleteTempExe(exe));
            thread.Start();
        }

        /**************************************
         * Delete the screen saver executable *
         **************************************/
        private void DeleteTempExe(string filename)
        {
            bool fail = true;
            do
            {
                try
                {
                    File.Delete(filename);
                    fail = false;
                }
                catch (Exception) {}
            } while (fail);
        }

        /*******************************************************
         * Closes the screen saver if any input is detected    *
         * (First event is ignored in order to flush the keys) *
         *******************************************************/
        private void Close(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
