using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace Asterisk_Statusik
{
    public partial class Form1 : MetroForm
    {
        Socket gniazdo = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        protected override void WndProc(ref Message message)
        {
            if (message.Msg == 0x0112 && (message.WParam.ToInt32() & 0xfff0) == 0xF010)
            {
                return;
            }

            base.WndProc(ref message);
        }
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        public Form1()
        {
            InitializeComponent();
           Home.TabPages.Remove(metroTabPage2); //logi skasowac by dzialaly
            if (Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Asterisk") == null)
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Asterisk");
                key.SetValue("IP", "127.0.0.1");
                key.SetValue("Port", "5038");
                key.SetValue("Login", "login");
                key.SetValue("Haslo", EncodeTo64("haslo"));
                key.SetValue("nazwa1", "0");
                key.SetValue("nazwa2", "0");
                key.SetValue("nazwa3", "0");
                key.SetValue("nazwa4", "0");
                key.SetValue("nazwa5", "0");
                key.SetValue("numer1", "0");
                key.SetValue("numer2", "0");
                key.SetValue("numer3", "0");
                key.SetValue("numer4", "0");
                key.SetValue("numer5", "0");
                key.SetValue("kontekst", "0");
                key.Close();
            }

            haslotext.PasswordChar = '*';
            Microsoft.Win32.RegistryKey keys;
            keys = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Asterisk");
            string nazwa1 = Convert.ToString(keys.GetValue("nazwa1", ""));
            string nazwa2 = Convert.ToString(keys.GetValue("nazwa2", ""));
            string nazwa3 = Convert.ToString(keys.GetValue("nazwa3", ""));
            string nazwa4 = Convert.ToString(keys.GetValue("nazwa4", ""));
            string nazwa5 = Convert.ToString(keys.GetValue("nazwa5", ""));
            string numer1 = Convert.ToString(keys.GetValue("numer1", ""));
            string numer2 = Convert.ToString(keys.GetValue("numer2", ""));
            string numer3 = Convert.ToString(keys.GetValue("numer3", ""));
            string numer4 = Convert.ToString(keys.GetValue("numer4", ""));
            string numer5 = Convert.ToString(keys.GetValue("numer5", ""));
            nz1.Text = nazwa1;
            nz2.Text = nazwa2;
            nz3.Text = nazwa3;
            nz4.Text = nazwa4;
            nz5.Text = nazwa5;
            n1.Text = numer1;
            n2.Text = numer2;
            n3.Text = numer3;
            n4.Text = numer4;
            n5.Text = numer5;
            numert1.Text = numer1;
            numert2.Text = numer2;
            numert3.Text = numer3;
            numert4.Text = numer4;
            numert5.Text = numer5;

            nazwat1.Text = nazwa1;
            nazwat2.Text = nazwa2;
            nazwat3.Text = nazwa3;
            nazwat4.Text = nazwa4;
            nazwat5.Text = nazwa5;
            keys.Close();
            try
            {
                backgroundWorker1.RunWorkerAsync();

            }
            catch
            {
                MessageBox.Show("Nie dziala proces background");
            }

            //this.ShowInTaskbar = true;
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                {
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Bottom - this.Height - 40);
                    return;
                }
            }

            numert1.Text = numer1;


        }
        public static string wyszukaj(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End);
            }
            else
            {
                return "";
            }
        }
        public static string odbierz(Socket sk)
        {
            string tk = string.Empty;
            byte[] odp = new byte[1024];
            int i = sk.Receive(odp);
            if (i > 0)
            {

                tk = Encoding.UTF8.GetString(odp, 0, i);
                return tk;
            }
            else {
                return "0";
            }
        }
        public static void wyslij(string t, Socket sk)
        {

            byte[] odp = Encoding.UTF8.GetBytes(t);
            sk.Send(odp);
        }
        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Form1_Move(object sender, EventArgs e)
        {
            bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
            if (this.WindowState == FormWindowState.Minimized && cursorNotInBar)
            {
                this.Hide();

                notifyIcon1.ShowBalloonTip(1000, "Status Asterisk", "Kliknij 2 razy aby uruchomić", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;

            //this.ShowInTaskbar = true;
        }



        private void metroButton1_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Asterisk");
            key.SetValue("IP", iptext.Text);
            key.SetValue("Port", porttext.Text);
            key.SetValue("Login", logintext.Text);
            key.SetValue("Haslo", EncodeTo64(haslotext.Text));
            key.SetValue("kontekst", konteksttext.Text);
            key.Close();
            MessageBox.Show("Zapisano");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Microsoft.Win32.RegistryKey keyo;
            keyo = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Asterisk");
            string ip = Convert.ToString(keyo.GetValue("IP", ""));
            string port = Convert.ToString(keyo.GetValue("Port", ""));
            string login = Convert.ToString(keyo.GetValue("Login", "Nie podano"));
            string kontekst = Convert.ToString(keyo.GetValue("kontekst", "Nie podano"));
            string haslo = DecodeFrom64(Convert.ToString(keyo.GetValue("Haslo", "")));
            keyo.Close();

            BeginInvoke((MethodInvoker)delegate
            {
                iptext.Text = ip;
                porttext.Text = port;
                logintext.Text = login;
                haslotext.Text = haslo;
                konteksttext.Text = kontekst;
            });

            try
            {
                gniazdo.Connect(ip, int.Parse(port));
                backgroundWorker3.RunWorkerAsync();
                wyslij("Action: Login\r\nUsername: " + login + "\r\nSecret: " + haslo + "\r\n\r\n", gniazdo);




            }
            catch (Exception ex)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    infoserwer.AppendText(Convert.ToString(ex));
                });
            }

        }





        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            string ciag = "";
            string statusodt;
            int x = 0;
            Microsoft.Win32.RegistryKey keyo;
            keyo = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Asterisk");
            string kontekst = Convert.ToString(keyo.GetValue("kontekst", "Nie podano"));
            ciag = odbierz(gniazdo);
            while (gniazdo.Connected)
            {
                string[] numer = new string[5] { n1.Text, n2.Text, n3.Text, n4.Text, n5.Text };


                for (x = 0; x < 5; x++)
                {


                    wyslij("Action: ExtensionState\r\nContext: "+ kontekst +"\r\nExten: " + numer[x] + "\r\nActionID: 1\r\n\r\n", gniazdo);

                    ciag = odbierz(gniazdo);

                    statusodt = ciag;

                  /*  BeginInvoke((MethodInvoker)delegate
                    {                                                   //logi!
                        infoserwer.AppendText(statusodt);
                    });
                */
                //MessageBox.Show(Convert.ToString(statusodt.IndexOf("Exten: " + numer[x])));




                if (statusodt.IndexOf("Exten: " + numer[0]) >= 0)
                    {
                        x = 0;
                    }
                    else if (statusodt.IndexOf("Exten: " + numer[1]) >= 0)
                    {
                        x = 1;
                    }
                    else if (statusodt.IndexOf("Exten: " + numer[2]) >= 0)
                    {
                        x = 2;
                    }
                    else if (statusodt.IndexOf("Exten: " + numer[3]) >= 0)
                    {
                        x = 3;
                    }
                    else if (statusodt.IndexOf("Exten: " + numer[4]) >= 0)
                    {
                        x = 4;
                    }
                    else
                    {
                        x = x;
                    }




                    if (numer[0] == numer[x])
                    {
                        if (statusodt.IndexOf("Status: 0") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust1.UseCustomForeColor = true;
                                statust1.ForeColor = Color.Green;

                                statust1.Text = "Wolny";

                            });
                        }
                        else if (statusodt.IndexOf("Status: 1") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust1.UseCustomForeColor = true;
                                statust1.ForeColor = Color.Orange;
                                statust1.Text = "W użyciu";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 2") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust1.UseCustomForeColor = true;
                                statust1.ForeColor = Color.Red;
                                statust1.Text = "Zajęty";
                            });

                        }
                        else if (statusodt.IndexOf("Status: 4") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust1.UseCustomForeColor = true;
                                statust1.ForeColor = Color.Red;
                                statust1.Text = "Niedostępny";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 8") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust1.UseCustomForeColor = true;
                                statust1.ForeColor = Color.Orange;
                                statust1.Text = "Dzwoni";
                            });
                        }
                    }
                    if (numer[1] == numer[x])
                    {
                        if (statusodt.IndexOf("Status: 0") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust2.UseCustomForeColor = true;
                                statust2.ForeColor = Color.Green;

                                statust2.Text = "Wolny";

                            });
                        }
                        else if (statusodt.IndexOf("Status: 1") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust2.UseCustomForeColor = true;
                                statust2.ForeColor = Color.Orange;
                                statust2.Text = "W użyciu";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 2") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust2.UseCustomForeColor = true;
                                statust2.ForeColor = Color.Red;
                                statust2.Text = "Zajęty";
                            });

                        }
                        else if (statusodt.IndexOf("Status: 4") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust2.UseCustomForeColor = true;
                                statust2.ForeColor = Color.Red;
                                statust2.Text = "Niedostępny";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 8") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust2.UseCustomForeColor = true;
                                statust2.ForeColor = Color.Orange;
                                statust2.Text = "Dzwoni";
                            });
                        }

                    }
                    if (numer[2] == numer[x])
                    {
                        if (statusodt.IndexOf("Status: 0") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust3.UseCustomForeColor = true;
                                statust3.ForeColor = Color.Green;

                                statust3.Text = "Wolny";

                            });
                        }
                        else if (statusodt.IndexOf("Status: 1") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust3.UseCustomForeColor = true;
                                statust3.ForeColor = Color.Orange;
                                statust3.Text = "W użyciu";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 2") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust3.UseCustomForeColor = true;
                                statust3.ForeColor = Color.Red;
                                statust3.Text = "Zajęty";
                            });

                        }
                        else if (statusodt.IndexOf("Status: 4") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust3.UseCustomForeColor = true;
                                statust3.ForeColor = Color.Red;
                                statust3.Text = "Niedostępny";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 8") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust3.UseCustomForeColor = true;
                                statust3.ForeColor = Color.Orange;
                                statust3.Text = "Dzwoni";
                            });
                        }
                    }
                   if (numer[3] == numer[x])
                    {
                        if (statusodt.IndexOf("Status: 0") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust4.UseCustomForeColor = true;
                                statust4.ForeColor = Color.Green;

                                statust4.Text = "Wolny";

                            });
                        }
                        else if (statusodt.IndexOf("Status: 1") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust4.UseCustomForeColor = true;
                                statust4.ForeColor = Color.Orange;
                                statust4.Text = "W użyciu";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 2") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust4.UseCustomForeColor = true;
                                statust4.ForeColor = Color.Red;
                                statust4.Text = "Zajęty";
                            });

                        }
                        else if (statusodt.IndexOf("Status: 4") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust4.UseCustomForeColor = true;
                                statust4.ForeColor = Color.Red;
                                statust4.Text = "Niedostępny";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 8") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust4.UseCustomForeColor = true;
                                statust4.ForeColor = Color.Orange;
                                statust4.Text = "Dzwoni";
                            });
                        }
                    }

                    if (numer[4] == numer[x])
                    {
                        if (statusodt.IndexOf("Status: 0") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust5.UseCustomForeColor = true;
                                statust5.ForeColor = Color.Green;

                                statust5.Text = "Wolny";

                            });
                        }
                        else if (statusodt.IndexOf("Status: 1") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust5.UseCustomForeColor = true;
                                statust5.ForeColor = Color.Orange;
                                statust5.Text = "W użyciu";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 2") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust5.UseCustomForeColor = true;
                                statust5.ForeColor = Color.Red;
                                statust5.Text = "Zajęty";
                            });

                        }
                        else if (statusodt.IndexOf("Status: 4") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust5.UseCustomForeColor = true;
                                statust5.ForeColor = Color.Red;
                                statust5.Text = "Niedostępny";
                            });
                        }
                        else if (statusodt.IndexOf("Status: 8") >= 0)
                        {
                            BeginInvoke((MethodInvoker)delegate
                            {
                                statust5.UseCustomForeColor = true;
                                statust5.ForeColor = Color.Orange;
                                statust5.Text = "Dzwoni";
                            });
                        }
                    }




                    Thread.Sleep(100);

                }
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Asterisk");
            key.SetValue("nazwa1", nz1.Text);
            key.SetValue("nazwa2", nz2.Text);
            key.SetValue("nazwa3", nz3.Text);
            key.SetValue("nazwa4", nz4.Text);
            key.SetValue("nazwa5", nz5.Text);
            key.SetValue("numer1", n1.Text);
            key.SetValue("numer2", n2.Text);
            key.SetValue("numer3", n3.Text);
            key.SetValue("numer4", n4.Text);
            key.SetValue("numer5", n5.Text);
            key.Close();
            numert1.Text = n1.Text;
            numert2.Text = n2.Text;
            numert3.Text = n3.Text;
            numert4.Text = n4.Text;
            numert5.Text = n5.Text;

            nazwat1.Text = nz1.Text;
            nazwat2.Text = nz2.Text;
            nazwat3.Text = nz3.Text;
            nazwat4.Text = nz4.Text;
            nazwat5.Text = nz5.Text;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gniazdo.Connected)
            {
                gniazdo.Disconnect(true);
            }
        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Asterisk");
            key.SetValue("IP", iptext.Text);
            key.SetValue("Port", porttext.Text);
            key.SetValue("Login", logintext.Text);
            key.SetValue("kontekst", konteksttext.Text);
            key.SetValue("Haslo", EncodeTo64(haslotext.Text));
            key.Close();
            MessageBox.Show("Zapisano! Aby połączyć się z nowym serwerem zrestartuj aplikację.");




    }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://GitISGit");
        }
    }
}
