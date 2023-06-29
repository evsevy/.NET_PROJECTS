using OtpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Faprogram
{
    
    public partial class Form1 : Form
    {
        private string _id;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string lol = "";
            try
            {
                lol = Clipboard.GetText();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Program.BrowserIds.Add(this.Text, lol);
            ToJson(Program.BrowserIds);
            Clipboard.SetText(GetPin(Program.BrowserIds[_id]));
        }
        private async void ToJson(Dictionary<string, string> br)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string fileName = "FA.json";
            using FileStream createStream = File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, br, options);
            await createStream.DisposeAsync();
        }

        private string GetPin(string st)
        {
            var bytes = Base32Encoding.ToBytes(st);
            var totp = new Totp(bytes);

            return (totp.ComputeTotp());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Second > 30)
            {
                progressBar1.Value = 60 - DateTime.Now.Second;
            }
            else
            {
                progressBar1.Value = 30 - DateTime.Now.Second;
            }

            IntPtr h = GetForegroundWindow();
            int pid = 0;
            GetWindowThreadProcessId(h, ref pid);
            Process p = Process.GetProcessById(pid);
            var st = p.MainWindowTitle;

            Regex regex = new Regex(@"\d\d\d\d - SunBrowser");
            if (regex.IsMatch(st))
            {
                var s = st.Split(' ');
                _id = s[0];
                this.Text = s[0];
            }

            if (Program.BrowserIds.Select(x => x.Key).Contains(_id))
            {
                button1.Visible = false;
                button2.Visible = true;
                textBox1.Text = GetPin(Program.BrowserIds[_id]);
            }
            else
            {
                button1.Visible = true;
                button2.Visible = false;
                textBox1.Text = "";
            } 
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                timer2.Enabled = false;
                var lol = Clipboard.GetText();
                var array = lol.ToArray();
                Thread.Sleep(200);
                for (int i = 0; i < array.Length; i++)
                {
                    SendKeys.Send(array[i].ToString());
                    Thread.Sleep(5);
                }
                timer2.Enabled = true;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(timer2.Enabled == true)
            {
                timer2.Enabled = false;
                button3.BackColor = Color.White;
                button3.Text = "OFF";
            }
            else
            {
                button3.Text = "ON";
                timer2.Enabled = true;
                button3.BackColor = Color.Green;
            }
        }
    }
}
