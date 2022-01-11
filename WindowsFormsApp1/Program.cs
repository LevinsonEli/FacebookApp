using System;
using System.Windows.Forms;

namespace FacebookApp.UI
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(AppForm.Create());
        }
    }
}
