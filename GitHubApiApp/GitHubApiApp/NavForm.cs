using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GitHubApiApp.Views;

namespace GitHubApiApp
{
    public partial class NavForm : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        
        public NavForm()
        {
            InitializeComponent();
            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
            FormBorderStyle = FormBorderStyle.None;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            HomeForm home = new HomeForm();
            home.TopLevel = false;
            panel4.Controls.Add(home);
            home.FormBorderStyle = FormBorderStyle.None;
            home.Dock = DockStyle.Fill;
            home.Show();
        }

        private void btn_Hover(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
        }

        private void nav_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            ReportForm report = new ReportForm
            {
                TopLevel = false,
            };
            panel4.Controls.Add(report);
            report.FormBorderStyle = FormBorderStyle.None;
            report.Dock = DockStyle.Fill;

            switch (btn.Text) 
            {
                case "Репозиторий":
                    panel4.Controls[1].Hide();
                    panel4.Controls[0].Show();
                    break;
                case "Формирование отчета":
                    HomeForm home = panel4.Controls[0] as HomeForm;
                    if (home.GetSelectedFiles())
                    {
                        panel4.Controls[0].Hide();
                        panel4.Controls[1].Show();
                    }
                    break;
            }
        }

        
        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

    

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            btn.ForeColor = Color.FromArgb(51,51,76);
            btn.BackColor = Color.FromArgb(51, 51, 76);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }
    }
}
