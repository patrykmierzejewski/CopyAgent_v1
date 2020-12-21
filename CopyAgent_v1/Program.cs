using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyAgent_v1
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (CheckInstanceProgram())
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool CheckInstanceProgram()
        {
            //Check Instance program
            if (InstanceMutex.IsActiveInstance())
            {
                System.Threading.ThreadPool.QueueUserWorkItem(t =>
                {
                    System.Threading.Thread.Sleep(10000);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                });

                MessageBox.Show("Allow Only Instance Program !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            else
                return false;
        }
    }
}
