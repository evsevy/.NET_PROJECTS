using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Faprogram
{
    internal static class Program
    {
        public static Dictionary<string, string> BrowserIds { get; set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BrowserIds = new Dictionary<string, string>();

            string fileName = "FA.json";
            string jsonString = File.ReadAllText(fileName);
            BrowserIds = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
