using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProfileSaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            String user = null;
            using (var login = new loginForm())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
                user = login.returnUser();
            }
            Application.Run(new Form1(user));
        }
    }
}
