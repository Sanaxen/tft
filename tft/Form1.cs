using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text;
using System.Runtime.InteropServices;

namespace tft
{

    public partial class Form1 : Form
    {
        [DllImport("shlwapi.dll",
            CharSet = CharSet.Auto)]
        private static extern IntPtr PathCombine(
            [Out] StringBuilder lpszDest,
            string lpszDir,
            string lpszFile);

        public string exePath = "";
        public string RlibPath = "";

        public string plot_time_unit = "";
        public Form1()
        {
            InitializeComponent();
            exePath = AppDomain.CurrentDomain.BaseDirectory;

            if ( File.Exists(exePath + "R_install_path.txt"))
            {
                using (StreamReader sr = new StreamReader(exePath + "R_install_path.txt"))
                {
                    textBox1.Text = sr.ReadToEnd().Replace("\n","");
                }
            }

            StringBuilder sb = new StringBuilder(2048);
            IntPtr res = PathCombine(sb, exePath, "..\\..\\..\\lib");
            if (res == IntPtr.Zero)
            {
                MessageBox.Show("Failed to obtain absolute path of R library.");
            }
            else
            {
                RlibPath = sb.ToString().Replace("\\", "/");
            }
            InitializeAsync();
        }
        async void InitializeAsync()
        {
            try
            {
                await webView21.EnsureCoreWebView2Async(null);
                await webView22.EnsureCoreWebView2Async(null);
                await webView24.EnsureCoreWebView2Async(null);
                await webView25.EnsureCoreWebView2Async(null);
            }
            catch (Exception)
            {
                MessageBox.Show("The WebView2 runtime may not be installed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }
        public static System.Drawing.Image CreateImage(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(
                filename,
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
            fs.Close();
            return img;
        }
        public string base_dir;
        public string work_dir;
        public string csv_file;
        public string csv_dir;
        public string base_name = "";
        public string link1, link2, link3, link4, link5;
        ListBox colname_list = new ListBox();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        void clearPlot()
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
            try
            {
                //webView21.Source = new Uri("about:blank");
                //webView22.Source = new Uri("about:blank");
                //webView24.Source = new Uri("about:blank");
                //webView25.Source = new Uri("about:blank");
                //if (webView21.CoreWebView2 != null)
                //{
                //    webView21.CoreWebView2.Navigate("about:blank");
                //}
                //if (webView22.CoreWebView2 != null)
                //{
                //    webView22.CoreWebView2.Navigate("about:blank");
                //}
                //if (webView24.CoreWebView2 != null)
                //{
                //    webView24.CoreWebView2.Navigate("about:blank");
                //}
                //if (webView25.CoreWebView2 != null)
                //{
                //    webView25.CoreWebView2.Navigate("about:blank");
                //}
            }
            catch { }
        }

        void Plot()
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            if (System.IO.File.Exists(base_name + "_p_input_plot.png"))
            {
                pictureBox1.Image = interactivePlot.CreateImage(base_name + "_p_input_plot.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Refresh();
            }
            if (System.IO.File.Exists(base_name + "_p_learn_rate_plot.png"))
            {
                pictureBox2.Image = interactivePlot.CreateImage(base_name + "_p_learn_rate_plot.png");
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.Refresh();
            }
            if (System.IO.File.Exists(base_name + "_fitted_plot.png"))
            {
                pictureBox3.Image = interactivePlot.CreateImage(base_name + "_fitted_plot.png");
                pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox3.Refresh();
            }
            if (System.IO.File.Exists(base_name + "_predict.png"))
            {

                pictureBox4.Image = interactivePlot.CreateImage(base_name + "_predict.png");
                pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox4.Refresh();
            }
            if (System.IO.File.Exists(base_name + "_predict_real.png"))
            {
                pictureBox5.Image = interactivePlot.CreateImage(base_name + "_predict_real.png");
                pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox5.Refresh();
            }
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                try
                {
                    pictureBox6.Image = CreateImage("tft_predict_measure_" + base_name + ".png");
                }
                catch { }
            }
            try
            {
                //webView21.CoreWebView2.Navigate("about:blank");
                //webView22.CoreWebView2.Navigate("about:blank");
                //webView24.CoreWebView2.Navigate("about:blank");
                //webView25.CoreWebView2.Navigate("about:blank");

            }
            catch { }
            if ( true )
            {
                string webpath = work_dir + "/tft_" + base_name + "_p_input_plot.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_input_plot.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView21.Source = new Uri(webpath);
                        if (webView21.CoreWebView2 != null)
                        {
                            //webView21.CoreWebView2.Navigate(webpath);
                        }
                        //webView21.Reload();
                        webView21.Update();
                        webView21.Refresh();
                    }
                    catch { }
                }
                webpath = work_dir + "/tft_" + base_name + "_p_learn_rate_plot.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView22.Source = new Uri(webpath);
                        if (webView22.CoreWebView2 != null)
                        {
                            //webView22.CoreWebView2.Navigate(webpath);
                        }
                        //webView22.Reload();
                        webView22.Update();
                        webView22.Refresh();
                    }
                    catch { }
                }
                webpath = work_dir + "/tft_" + base_name + "_plt0.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView24.Source = new Uri(webpath);
                        if (webView24.CoreWebView2 != null)
                        {
                            //webView24.CoreWebView2.Navigate(webpath);
                        }
                        //webView24.Reload();
                        webView24.Update();
                        webView24.Refresh();
                    }
                    catch { }
                }
                webpath = work_dir + "/tft_" + base_name + "_p.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView25.Source = new Uri(webpath);
                        if (webView25.CoreWebView2 != null)
                        {
                            //webView25.CoreWebView2.Navigate(webpath);
                        }
                        //webView25.Reload();
                        webView25.Update();
                        webView25.Refresh();
                    }
                    catch { }
                }
            }
        }

        private void UpdateInvokeRequire()
        {
            TimeSpan ts = stopwatch.Elapsed;
            label15.Text = $"{ts.Hours}H {ts.Minutes}M {ts.Seconds}S {ts.Milliseconds}ms";
            label15.Refresh();
            if (File.Exists(base_name + "_predict_measure.txt"))
            {
                using (StreamReader sr = new StreamReader(base_name + "_predict_measure.txt"))
                {
                    textBox2.Text = sr.ReadToEnd();
                }
            }

            pictureBox6.Image = null;
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                try
                {
                    pictureBox6.Image = CreateImage("tft_predict_measure_" + base_name + ".png");
                    // pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch { }
            }
            Plot();

            button3.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = false;
            button9.Enabled = false;
            if (checkBox11.Checked) button8.Enabled = true;

            if (File.Exists("tft_train_errorLog_" + base_name + ".txt"))
            {
                using (StreamReader sr = new StreamReader("tft_train_errorLog_" + base_name + ".txt"))
                {
                    label17.Text = sr.ReadToEnd();
                }
                toolTip1.SetToolTip(label7, label7.Text);
            }
            if (File.Exists("tft_predict_errorLog_" + base_name + ".txt"))
            {
                using (StreamReader sr = new StreamReader("tft_predict_errorLog_" + base_name + ".txt"))
                {
                    label17.Text = sr.ReadToEnd();
                }
                toolTip1.SetToolTip(label7, label7.Text);
            }
        }

        void Proc_Exited(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(50);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            if (InvokeRequired)
            {
                Invoke(new Action(this.UpdateInvokeRequire));
                //Invoke(new Action(() => { label1.Text = "Stop!"; }));
            }
            else
            {
                label15.Text = $"{ts.Hours}H {ts.Minutes}M {ts.Seconds}S {ts.Milliseconds}ms";
                label15.Refresh();

                if (File.Exists(base_name + "_predict_measure.txt"))
                {
                    using (StreamReader sr = new StreamReader(base_name + "_predict_measure.txt"))
                    {
                        textBox2.Text = sr.ReadToEnd();
                    }
                }

                pictureBox6.Image = null;
                if (File.Exists("tft_predict_measure_" + base_name + ".png"))
                {
                    try
                    {
                        pictureBox6.Image = CreateImage("tft_predict_measure_" + base_name + ".png");
                        // pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch { }
                }
                Plot();

                button3.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                button8.Enabled = false;
                button9.Enabled = false;
                if (checkBox11.Checked) button8.Enabled = true;

                if (File.Exists("tft_train_errorLog_" + base_name + ".txt"))
                {
                    using (StreamReader sr = new StreamReader("tft_train_errorLog_" + base_name + ".txt"))
                    {
                        label17.Text = sr.ReadToEnd();
                    }
                    toolTip1.SetToolTip(label7, label7.Text);
                }
                if (File.Exists("tft_predict_errorLog_" + base_name + ".txt"))
                {
                    using (StreamReader sr = new StreamReader("tft_predict_errorLog_" + base_name + ".txt"))
                    {
                        label17.Text = sr.ReadToEnd();
                    }
                    toolTip1.SetToolTip(label7, label7.Text);
                }
            }
        }

        public void execute(string script_file, bool wait = true)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo();
            //pInfo.FileName = textBox1.Text + "\\R.exe";
            //pInfo.Arguments = "CMD BATCH  --vanilla " + script_file;
            
            //pInfo.FileName = textBox1.Text + "\\Rscript.exe";
            //pInfo.Arguments = "" + script_file;
            
            pInfo.FileName = textBox1.Text + "\\x64\\Rscript.exe";
            pInfo.Arguments = "" + script_file;

            //Process p = Process.Start(pInfo);
            Process p = new Process();
            p.StartInfo = pInfo;

            if (wait)
            {
                p.Start();
                p.WaitForExit();
            }else
            {
                stopwatch.Start();
                p.Exited += new EventHandler(Proc_Exited);
                p.EnableRaisingEvents = true;
                p.Start();
            }
        }

        public ListBox GetNames()
        {
            if (File.Exists("names.txt"))
            {
                File.Delete("names.txt");
            }

            string cmd = "";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "df <- read.csv(\"" + base_name  + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "x_<-ncol(df)\r\n";
            cmd += "print(x_)\r\n";
            cmd += "for ( i in 1:x_) print(names(df)[i])\r\n";
            string file = "tmp_get_namse_"+base_name +".R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write("sink(file = \"names.txt\")\r\n");
                    sw.Write(cmd);
                    sw.Write("sink()\r\n");
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in "+file , "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return null;
            }

            execute(file);
            ListBox list = new ListBox();

            if (File.Exists("names.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("names.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var nums = line.Split(' ');
                        int num = int.Parse(nums[1]);

                        for (int i = 0; i < num; i++)
                        {
                            line = sr.ReadLine();
                            var names = line.Substring(4);

                            names = names.Replace("\n", "");
                            names = names.Replace("\r", "");
                            names = names.Replace("\"", "");
                            if (names.IndexOf(" ") >= 0)
                            {
                                names = "'" + names + "'";
                            }
                            list.Items.Add(names);
                        }
                        if (list.Items.Count != num)
                        {
                            MessageBox.Show("Does the column name contain \", \" or \"spaces\"?\n" +
                                "ou may not be getting the column names correctly.", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    }
                    sr.Close();
                }
                catch { }
            }
            return list;
        }
        public ListBox GetKeys()
        {
            if (File.Exists("keys.txt"))
            {
                File.Delete("keys.txt");
            }
            if (comboBox5.Text == "")
            {
                MessageBox.Show("select key!");
                return null;
            }
            string cmd = ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "x_<-unique(df$'"+comboBox5.Text + "')\r\n";
            cmd += "length(x_)\r\n";
            cmd += "for ( i in 1:length(x_)) print(x_[i])\r\n";
            string file = "tmp_get_keys_" + base_name + ".R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write("sink(file = \"keys.txt\")\r\n");
                    sw.Write(cmd);
                    sw.Write("sink()\r\n");
                    sw.Write("save.image(\"tft_" + base_name + ".RData\")\r\n");
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in "+ file , "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return null;
            }

            execute(file);
            ListBox list = new ListBox();

            if (File.Exists("keys.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("keys.txt", Encoding.GetEncoding("SHIFT_JIS"));
                    while (sr.EndOfStream == false)
                    {
                        string line = sr.ReadLine();
                        var nums = line.Split(' ');
                        int num = int.Parse(nums[1]);

                        for (int i = 0; i < num; i++)
                        {
                            line = sr.ReadLine();
                            var names = line.Substring(4);

                            names = names.Replace("\n", "");
                            names = names.Replace("\r", "");
                            names = names.Replace("\"", "");
                            if (names.IndexOf(" ") >= 0)
                            {
                                names = "'" + names + "'";
                            }
                            list.Items.Add(names);
                        }
                        if (list.Items.Count != num)
                        {
                            MessageBox.Show("Does the column name contain \", \" or \"spaces\"?\n" +
                                "ou may not be getting the column names correctly.", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    }
                    sr.Close();
                }
                catch { }
            }
            return list;
        }

        public string tft_header_ru()
        {
            string cmd = "";
            cmd += ".libPaths(c('" + RlibPath + "',.libPaths()))\r\n";
            cmd += "dir='" + work_dir.Replace("\\", "\\\\") + "'\r\n";
            cmd += "setwd(dir)\r\n";
            cmd += "library(torch)\r\n";
            cmd += "library(ggplot2)\r\n";
            cmd += "suppressPackageStartupMessages(library(tidymodels))\r\n";
            cmd += "library(plotly)\r\n";
            cmd += "library(htmlwidgets)\r\n";
            cmd += "library(scales)\r\n";
            cmd += "library(dplyr)\r\n";
            cmd += "library(tidyverse)\r\n";
            cmd += "library(recipes)\r\n";
            cmd += "library(tft)\r\n";
            cmd += "set.seed(1)\r\n";
            cmd += "torch::torch_manual_seed(1)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "source('../../script/tft_util.r')\r\n";

            return cmd;
        }
        string tft_init()
        {
            string cmd = "";

            cmd = tft_header_ru();


            cmd += "n_epochs=" + numericUpDown1.Value.ToString() + "\r\n";
            cmd += "n_batch_size=" + numericUpDown2.Value.ToString() + "\r\n";
            cmd += "n_num_workers=" + numericUpDown3.Value.ToString() + "\r\n";
            cmd += "pred_len=" + numericUpDown4.Value.ToString() + "\r\n";
            cmd += "lookback=" + numericUpDown5.Value.ToString() + "\r\n";

            cmd += "future_test_len = " + numericUpDown6.Value.ToString() + "\r\n";

            cmd += "time_colname='" + comboBox3.Text + "'\r\n";
            cmd += "target_colname='" + comboBox4.Text + "'\r\n";
            cmd += "key='" + comboBox5.Text + "'\r\n";

            cmd += "unit ='" + comboBox1.Text + "'\r\n";

            if (checkBox1.Checked)
            {
                cmd += "validation = TRUE\r\n";
            }
            else
            {
                cmd += "validation = FALSE\r\n";
            }
            cmd += "learn_rate = 1e-3\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            if (checkBox2.Checked) cmd += "use_date_year = TRUE\r\n";
            else cmd += "use_date_year = FALSE\r\n";
            if (checkBox3.Checked) cmd += "use_date_month = TRUE\r\n";
            else cmd += "use_date_month = FALSE\r\n";
            if (checkBox4.Checked) cmd += "use_date_week = TRUE\r\n";
            else cmd += "use_date_week = FALSE\r\n";
            if (checkBox5.Checked) cmd += "use_date_day = TRUE\r\n";
            else cmd += "use_date_day = FALSE\r\n";
            if (checkBox6.Checked) cmd += "use_date_wday = TRUE\r\n";
            else cmd += "use_date_wday = FALSE\r\n";
            if (checkBox7.Checked) cmd += "use_date_yday = TRUE\r\n";
            else cmd += "use_date_yday = FALSE\r\n";
            if (checkBox8.Checked) cmd += "use_date_hour = TRUE\r\n";
            else cmd += "use_date_hour = FALSE\r\n";
            if (checkBox9.Checked) cmd += "use_date_am = TRUE\r\n";
            else cmd += "use_date_am = FALSE\r\n";
            if (checkBox10.Checked) cmd += "use_date_pm = TRUE\r\n";
            else cmd += "use_date_pm = FALSE\r\n";
            if (checkBox16.Checked) cmd += "use_date_quarter = TRUE\r\n";
            else cmd += "use_date_quarter = FALSE\r\n";

            if (checkBox12.Checked) cmd += "use_date_sincosY = TRUE\r\n";
            else cmd += "use_date_sincosY = FALSE\r\n";
            if (checkBox13.Checked) cmd += "use_date_sincosM = TRUE\r\n";
            else cmd += "use_date_sincosM = FALSE\r\n";
            if (checkBox15.Checked) cmd += "use_date_sincosW = TRUE\r\n";
            else cmd += "use_date_sincosW = FALSE\r\n";
            if (checkBox14.Checked) cmd += "use_date_sincosD = TRUE\r\n";
            else cmd += "use_date_sincosD = FALSE\r\n";

            if (checkBox18.Checked) cmd += "use_date_lag = TRUE\r\n";
            else cmd += "use_date_lag = FALSE\r\n";
            if (checkBox19.Checked) cmd += "use_date_mean = TRUE\r\n";
            else cmd += "use_date_mean = FALSE\r\n";
            if (checkBox20.Checked) cmd += "use_date_sd = TRUE\r\n";
            else cmd += "use_date_sd = FALSE\r\n";
            if (checkBox21.Checked) cmd += "use_date_quantile = TRUE\r\n";
            else cmd += "use_date_quantile = FALSE\r\n";

            if (checkBox23.Checked) cmd += "use_date_min = TRUE\r\n";
            else cmd += "use_date_min = FALSE\r\n";

            if (checkBox24.Checked) cmd += "use_date_max = TRUE\r\n";
            else cmd += "use_date_max = FALSE\r\n";

            cmd += "window_size=" + numericUpDown7.Value.ToString()+"\r\n";

            if (checkBox22.Checked) cmd += "use_target_diff = TRUE\r\n";
            else cmd += "use_target_diff = FALSE\r\n";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "df <- read.csv(\"" + base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";

            if ( comboBox8.Text != "")
            {
                cmd += "df <- df %>% filter("+ comboBox5.Text+" == '" + comboBox8.Text + "')\r\n";
            }
            cmd += "train <- NULL\r\n";
            cmd += "valid <- NULL\r\n";
            cmd += "test <- NULL\r\n";

            if (listBox4.SelectedIndices.Count > 0)
            {
                cmd += "df2 <- df %>% \r\n";
                cmd += "    select(\r\n";
                cmd += "        "+listBox4.Items[listBox4.SelectedIndices[0]].ToString() + "=" +
                       listBox4.Items[listBox4.SelectedIndices[0]].ToString();
                for (int i = 1; i < listBox4.SelectedIndices.Count; i++)
                {
                    cmd += ",\r\n";
                    cmd += "        " + listBox4.Items[listBox4.SelectedIndices[i]].ToString() + "=" +
                           listBox4.Items[listBox4.SelectedIndices[i]].ToString();
                }
                cmd += "    )\r\n";
            }else
            {
                cmd += "df2 <- df\r\n";
            }

            cmd += "na_pos <- is.na(df[,target_colname])\r\n";
            cmd += "input_df <- tft_colname_conv(df2, time_colname=time_colname, target_colname=target_colname, key=key)\r\n";
            cmd += "str(input_df)\r\n";
            cmd += "\r\n";
            for ( int i = 0; i < listBox3.SelectedIndices.Count; i++)
            {
                cmd += "input_df$" + listBox3.Items[listBox3.SelectedIndices[i]].ToString() + "<- as.factor(" + "input_df$" + listBox3.Items[listBox3.SelectedIndices[i]].ToString() + ")\r\n";
            }
            cmd += "n = length(unique(input_df$key))\r\n";
            cmd += "nr = nrow(df)-(pred_len+future_test_len)*n\r\n";
            cmd += "na_pos[1:(nr-1)] <- FALSE\r\n";

            cmd += "if ( use_target_diff ){\r\n";
            cmd += "    input_df$target[na_pos==T] <- NA\r\n";
            cmd += "    input_df <-input_df %>%  group_by(key) %>%  mutate(target_diff = target - lag(target, n=1))\r\n";
            cmd += "    input_df <- mutate_at(input_df, c('target_diff'), ~replace(., is.na(.), 0))\r\n";
            cmd += "    target_colname='target_diff'\r\n";
            cmd += "    input_df$target_org <-  input_df$target\r\n";
            cmd += "    input_df$target <-input_df$target_diff\r\n";
            cmd += "}\r\n";
            cmd += "str(input_df)\r\n";
            cmd += "print(table(is.na(input_df)))\r\n";

            cmd += "input_plot <- tft_plot_input(input_df, unit='" + comboBox2.Text + "')\r\n";

            if (comboBox7.Text != "")
            {
                cmd += "input_df <- tft_data_compact(input_df, step_unit='" + comboBox7.Text + "')\r\n";
                cmd += "input_plot <- tft_plot_input(input_df, unit='" + comboBox2.Text + "')\r\n";
                comboBox1.Text = comboBox7.Text;
                cmd += "unit ='" + comboBox1.Text + "'\r\n";
            }
            cmd += "\r\n";
            cmd += "ggsave(file = \"" + base_name + "_p_input_plot.png\", plot = input_plot, dpi = 100, width = 6.4, height = 4.8)\r\n";
            cmd += "\r\n";
            cmd += "p_input_plot <- ggplotly(input_plot)\r\n";
            cmd += "print(p_input_plot)\r\n";
            cmd += "htmlwidgets::saveWidget(as_widget(p_input_plot), \"tft_" + base_name + "_p_input_plot.html\", selfcontained = F)\r\n";

            cmd += "data_tbl <- tft_data_split(input_df, unit=unit, lookback, pred_len, future_test_len, validation)\r\n";
            cmd += "source('tmp_tft_data_split.r')\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "write.csv(data_tbl,'" + base_name + "_Reshaped.csv', row.names = FALSE)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "rec <- tft_make_recipe(train, validation=validation)\r\n";
            cmd += "tft_make_spec(train, rec, covariate_known=c(\r\n";

            if (listBox1.SelectedItems.Count >= 1)
            {
                cmd += "'" + listBox1.SelectedItems[0].ToString() + "'";
                for (int i = 1; i < listBox1.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    cmd += "'" + listBox1.SelectedItems[i].ToString() + "'";
                }
            }
            if (listBox2.SelectedItems.Count >= 1)
            {
                cmd += "),\r\n";
                cmd += "covariate_static=c(\r\n";
                cmd += "'" + listBox2.SelectedItems[0].ToString() + "'";
                for (int i = 1; i < listBox2.SelectedItems.Count; i++)
                {
                    cmd += ", ";
                    cmd += "'" + listBox2.SelectedItems[i].ToString() + "'";
                }
                cmd += ")\r\n";
            }else
            {
                cmd += ")\r\n";
            }
            //
            cmd += ")\r\n";
            cmd += "source('tmp_tft_make_spec.r')\r\n";
            cmd += "print(spec)\r\n";
            cmd += "save.image(\"tft_" + base_name + ".RData\")\r\n";
            cmd += "\r\n";

            return cmd;
        }

        void save()
        {
            string file = "tft_setting_"+base_name+".txt";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(listBox1.Items.Count.ToString() + "\n");
                    for ( int i = 0; i < listBox1.Items.Count; i++)
                    {
                        sw.Write(listBox1.Items[i].ToString() + "\n");
                    }

                    sw.Write(listBox1.SelectedItems.Count.ToString() + "\n");
                    if (listBox1.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox1.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox2.SelectedItems.Count.ToString() + "\n");
                    if (listBox2.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox2.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox2.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox3.SelectedItems.Count.ToString() + "\n");
                    if (listBox3.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox3.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox3.SelectedIndices[i].ToString() + "\n");
                        }
                    }
                    sw.Write(listBox4.SelectedItems.Count.ToString() + "\n");
                    if (listBox4.SelectedItems.Count >= 1)
                    {
                        for (int i = 0; i < listBox4.SelectedItems.Count; i++)
                        {
                            sw.Write(listBox4.SelectedIndices[i].ToString() + "\n");
                        }
                    }

                    sw.Write("r_path," + textBox1.Text + "\n");
                    sw.Write("n_epochs," + numericUpDown1.Value.ToString() + "\n");
                    sw.Write("n_batch_size," + numericUpDown2.Value.ToString() + "\n");
                    sw.Write("n_num_workers," + numericUpDown3.Value.ToString() + "\n");
                    sw.Write("pred_len," + numericUpDown4.Value.ToString() + "\n");
                    sw.Write("lookback," + numericUpDown5.Value.ToString() + "\n");
                    sw.Write("future_test_len," + numericUpDown6.Value.ToString() + "\n");

                    sw.Write("time_colname," + comboBox3.Text + "\n");
                    sw.Write("target_colname," + comboBox4.Text + "\n");
                    sw.Write("target_key," + comboBox8.Text + "\n");
                    sw.Write("key," + comboBox5.Text + "\n");
                    sw.Write("plot unit," + comboBox2.Text + "\n");
                    sw.Write("unit," + comboBox1.Text + "\n");
                    sw.Write("time compression," + comboBox7.Text + "\n");

                    sw.Write("learn_rate,");
                    if (checkBox11.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("validation,");
                    if (checkBox1.Checked) sw.Write("true\n");
                    else sw.Write("false\n");

                    sw.Write("use_date_year,");
                    if (checkBox2.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_month,");
                    if (checkBox3.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_week,");
                    if (checkBox4.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_day,");
                    if (checkBox5.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_wday,");
                    if (checkBox6.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_yday,");
                    if (checkBox7.Checked) sw.Write("true\n");
                    else sw.Write("0\n");
                    sw.Write("use_date_hour,");
                    if (checkBox8.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_am,");
                    if (checkBox9.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_pm,");
                    if (checkBox10.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_quarter,");
                    if (checkBox16.Checked) sw.Write("true\n");
                    else sw.Write("false\n");

                    sw.Write("use_date_sincos_Y,");
                    if (checkBox12.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_sincos_M,");
                    if (checkBox13.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_sincos_W,");
                    if (checkBox15.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_sincos_D,");
                    if (checkBox14.Checked) sw.Write("true\n");
                    else sw.Write("false\n");

                    sw.Write("accelerator,");
                    if (checkBox17.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("GPU id," + comboBox6.Text + "\n");

                    sw.Write("use_date_lag,");
                    if (checkBox18.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_mean,");
                    if (checkBox19.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_sd,");
                    if (checkBox20.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_quantile,");
                    if (checkBox21.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_target_diff,");
                    if (checkBox22.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_min,");
                    if (checkBox23.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("use_date_max,");
                    if (checkBox24.Checked) sw.Write("true\n");
                    else sw.Write("false\n");
                    sw.Write("Importance,");
                    if (checkBox25.Checked) sw.Write("true\n");
                    else sw.Write("false\n");

                    sw.Write("window_size," + numericUpDown7.Value.ToString() + "\n");

                    sw.Write("link1,");
                    sw.Write(link1 + "\n");
                    sw.Write("link2,");
                    sw.Write(link2 + "\n");
                    sw.Write("link3,");
                    sw.Write(link3 + "\n");
                    sw.Write("link4,");
                    sw.Write(link4 + "\n");
                    sw.Write("link5,");
                    sw.Write(link5 + "\n");

                    sw.Write("predict_measure,");
                    sw.Write(textBox2.Text + "\r\n");
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in tft_setting.txt", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
            try
            {
                File.Copy("tft_setting_" + base_name + ".txt", "pre_setting.txt", true);
            }
            catch { }
        }
        void load(string setting_file)
        {
            string file = "tft_setting_" + base_name + ".txt";
            if (setting_file == "")
            {
                if (base_name == "")
                {
                    MessageBox.Show("input csv file !");
                    return;
                }
                if (!File.Exists(file)) save();

                if (!File.Exists(file))
                {
                    MessageBox.Show("file not found[" + "tft_setting_" + base_name + ".txt]");
                }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
            }
            else
            {
                file = setting_file;
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(file, Encoding.GetEncoding("SHIFT_JIS"));
            if (sr != null)
            {
                while (sr.EndOfStream == false)
                {
                    string s = sr.ReadLine();
                    int n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        if (setting_file == "")
                        {
                            listBox1.Items.Add(s.Replace("\n", ""));
                            listBox2.Items.Add(s.Replace("\n", ""));
                            listBox3.Items.Add(s.Replace("\n", ""));
                            listBox4.Items.Add(s.Replace("\n", ""));
                        }
                    }
                    if (setting_file == "")
                    {
                        for (int i = 0; i < n; i++)
                        {
                            listBox1.SetSelected(i, false);
                            listBox2.SetSelected(i, false);
                            listBox3.SetSelected(i, false);
                            listBox4.SetSelected(i, false);
                        }
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox1.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox2.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox3.SetSelected(k, true);
                    }
                    s = sr.ReadLine();
                    n = int.Parse(s.Replace("\n", ""));
                    for (int i = 0; i < n; i++)
                    {
                        s = sr.ReadLine();
                        int k = int.Parse(s.Replace("\n", ""));
                        listBox4.SetSelected(k, true);
                    }
                    while (sr.EndOfStream == false)
                    {
                        s = sr.ReadLine();
                        var ss = s.Split(',');

                        if (ss[0].IndexOf("r_path") >= 0)
                        {
                            textBox1.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("link1") >= 0)
                        {
                            link1 = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("link2") >= 0)
                        {
                            link2 = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("link3") >= 0)
                        {
                            link3 = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("link4") >= 0)
                        {
                            link4 = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("link5") >= 0)
                        {
                            link5 = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("n_epochs") >= 0)
                        {
                            numericUpDown1.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        if (ss[0].IndexOf("n_batch_size") >= 0)
                        {
                            numericUpDown2.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        if (ss[0].IndexOf("n_num_workers") >= 0)
                        {
                            numericUpDown3.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        if (ss[0].IndexOf("pred_len") >= 0)
                        {
                            numericUpDown4.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        if (ss[0].IndexOf("lookback") >= 0)
                        {
                            numericUpDown5.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        if (ss[0].IndexOf("future_test_len") >= 0)
                        {
                            numericUpDown6.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                        //
                        if (ss[0].IndexOf("time_colname") >= 0)
                        {
                            comboBox3.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("target_key") >= 0)
                        {
                            comboBox8.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("target_colname") >= 0)
                        {
                            comboBox4.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("key") >= 0)
                        {
                            comboBox5.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("plot unit") >= 0)
                        {
                            comboBox2.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("unit") >= 0)
                        {
                            comboBox1.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("time compressio") >= 0)
                        {
                            comboBox7.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("learn_rate") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox11.Checked = true;
                            }
                            else
                            {
                                checkBox11.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("validation") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox1.Checked = true;
                            }
                            else
                            {
                                checkBox1.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_year") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox2.Checked = true;
                            }
                            else
                            {
                                checkBox2.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_month") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox3.Checked = true;
                            }
                            else
                            {
                                checkBox3.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_week") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox4.Checked = true;
                            }
                            else
                            {
                                checkBox4.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_day") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox5.Checked = true;
                            }
                            else
                            {
                                checkBox5.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_wday") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox6.Checked = true;
                            }
                            else
                            {
                                checkBox6.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_yday") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox7.Checked = true;
                            }
                            else
                            {
                                checkBox7.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_hour") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox8.Checked = true;
                            }
                            else
                            {
                                checkBox8.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_am") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox9.Checked = true;
                            }
                            else
                            {
                                checkBox9.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_pm") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox10.Checked = true;
                            }
                            else
                            {
                                checkBox10.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_quarter") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox16.Checked = true;
                            }
                            else
                            {
                                checkBox16.Checked = false;
                            }
                            continue;
                        }   

                        if (ss[0].IndexOf("predict_measure") >= 0)
                        {
                            textBox2.Text = ss[1]+ sr.ReadToEnd();
                            continue;
                        }

                        if (ss[0].IndexOf("use_date_sincos_Y") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox12.Checked = true;
                            }
                            else
                            {
                                checkBox12.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_sincos_M") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox13.Checked = true;
                            }
                            else
                            {
                                checkBox13.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_sincos_W") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox15.Checked = true;
                            }
                            else
                            {
                                checkBox15.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_sincos_D") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox14.Checked = true;
                            }
                            else
                            {
                                checkBox14.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("accelerator") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox17.Checked = true;
                            }
                            else
                            {
                                checkBox17.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("GPU id") >= 0)
                        {
                            comboBox6.Text = ss[1].Replace("\r\n", "");
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_lag") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox18.Checked = true;
                            }
                            else
                            {
                                checkBox18.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_mean") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox19.Checked = true;
                            }
                            else
                            {
                                checkBox19.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_sd") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox20.Checked = true;
                            }
                            else
                            {
                                checkBox20.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_quantile") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox21.Checked = true;
                            }
                            else
                            {
                                checkBox21.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_target_diff") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox22.Checked = true;
                            }
                            else
                            {
                                checkBox22.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_min") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox23.Checked = true;
                            }
                            else
                            {
                                checkBox23.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("use_date_max") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox24.Checked = true;
                            }
                            else
                            {
                                checkBox24.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("Importance") >= 0)
                        {
                            if (ss[1].Replace("\r\n", "") == "true")
                            {
                                checkBox25.Checked = true;
                            }
                            else
                            {
                                checkBox25.Checked = false;
                            }
                            continue;
                        }
                        if (ss[0].IndexOf("window_size") >= 0)
                        {
                            numericUpDown7.Value = int.Parse(ss[1].Replace("\r\n", ""));
                            continue;
                        }
                    }
                }
            }
            if (sr != null) sr.Close();
            Plot();
        }

        string tft_train()
        {
            string cmd = "";

            cmd += "tft_accelerator = luz::accelerator(\r\n";
            cmd += "    device_placement = TRUE,\r\n";
            if (checkBox17.Checked)
            {
                cmd += "    cpu = FALSE,\r\n";
                if (comboBox6.Text == "default")
                {
                    cmd += "    cuda_index = torch::cuda_current_device()\r\n";
                }
                else
                {
                    cmd += "    cuda_index = " + comboBox6.Text + "\r\n";
                }
            }
            else
            {
                cmd += "    cpu = TRUE,\r\n";
                cmd += "    cuda_index = 0\r\n";
            }

            cmd += ")\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
 
            if (checkBox11.Checked)
            {
                cmd += "learn_rate = lookup_lr(spec, train, accelerator=tft_accelerator,steps = 20, return_plot=T)\r\n";
                cmd += "print(learn_rate[[1]])\r\n";
                cmd += "learn_rate_plot <- (learn_rate[[2]])\r\n";
                cmd += "save.image(\"tft_" + base_name + ".RData\")\r\n";
	            cmd += "\r\n";
	            cmd += "ggsave(file = \"" + base_name + "_p_learn_rate_plot.png\", plot = learn_rate_plot, dpi = 100, width = 6.4, height = 4.8)\r\n";
	            cmd += "\r\n";
	            cmd += "p_learn_rate_plot <- ggplotly(learn_rate_plot)\r\n";
	            cmd += "print(p_learn_rate_plot)\r\n";
	            cmd += "htmlwidgets::saveWidget(as_widget(p_learn_rate_plot), \"tft_" + base_name + "_p_learn_rate_plot.html\", selfcontained = F)\r\n";
            }
            cmd += "\r\n";
            cmd += "\r\n";

            cmd += "fitted <- tft_train(spec, accelerator=tft_accelerator, validation=validation, base_name ='" + base_name+"')\r\n";
            cmd += "plot(fitted)\r\n";

            cmd += "png(\""+ base_name + "_fitted_plot.png\", width = 640, height = 400)\r\n"; ;
            cmd += "plot(fitted)\r\n";
            cmd += "dev.off()\r\n";
            cmd += "save.image(\"tft_" + base_name + ".RData\")\r\n";
            cmd += "save.image(\"tft_" + base_name + "_fitted.RData\")\r\n";

            cmd += "# cpp_torch_tensor_dtype(self$ptr) error: external pointer is not valid\r\n";
            cmd += "#saveRDS(fitted, file=\"" + base_name + "_fitted.rd\")\r\n";
            cmd += "luz::luz_save(fitted, \"luz_fitted_"+base_name+".luz\")\r\n";
            cmd += "\r\n";
            cmd += "\r\n";

            return cmd;
        }

        string tft_test()
        {
            string cmd = "";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "# cpp_torch_tensor_dtype(self$ptr) error: external pointer is not valid\r\n";
            cmd += "fitted <-luz::luz_load(\"luz_fitted_" + base_name + ".luz\")\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "# cpp_Tensor_storage(self$ptr) error: external pointer is not valid\r\n";
            cmd += "#load(\"tft_" + base_name + ".RData\")\r\n";
            cmd += "#fitted<- readRDS(\"" + base_name + "_fitted.rd\")\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "pred <- NULL\r\n";
            cmd += "pred <- tft_predict(fitted, test, validation=validation, base_name ='"+base_name +"')\r\n";
            cmd += "plt0 <- tft_predict_plot(pred, timestep='" + comboBox2.Text + "')\r\n";
            cmd += "plt0_plotly <- ggplotly(plt0)\r\n";
            cmd += "print(plt0_plotly)\r\n";
            cmd += "htmlwidgets::saveWidget(as_widget(plt0_plotly), \"tft_" + base_name + "_plt0.html\", selfcontained = F)\r\n";
            cmd += "\r\n";
            cmd += "plt3 <- tft_predict_plot(pred, timestep='" + comboBox2.Text + "', use_real_data=T)\r\n";
            cmd += "#plt1 <- tft_predict_plot_ymd(pred, cutoff_ymd='2019-01-01')\r\n";
            cmd += "tft_pred_save(pred, filename=\"" + base_name + "_pred_save.csv\")\r\n";
            cmd += "plt2 <- tft_predict_check(pred, timestep='" + comboBox2.Text + "')\r\n";
            cmd += "p <- gridExtra::grid.arrange(plt3, plt2, ncol = 1)\r\n";
            cmd += "\r\n";
            cmd += "ggsave(file = \"" + base_name + "_predict.png\", plot = plt0, dpi = 100, width = 6.4, height = 4.8)\r\n";
            cmd += "\r\n";
            cmd += "p_plotly <- subplot(plt3, plt2, nrows = 2)\r\n";
            cmd += "print(p_plotly)\r\n";
            cmd += "htmlwidgets::saveWidget(as_widget(p_plotly), \"tft_" + base_name + "_p.html\", selfcontained = F)\r\n";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "ggsave(file = \"" + base_name + "_predict_real.png\", plot = p, dpi = 100, width = 6.4, height = 4.8)\r\n";
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "#saveRDS(fitted, file=\"" + base_name + "_fitted.rd\")\r\n";
            cmd += "save.image(\"tft_" + base_name + ".RData\")\r\n";
            cmd += "load(\"tft_" + base_name + ".RData\")\r\n";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "sink(file = \"" + base_name + "_predict_measure.txt\")\r\n";
            cmd += "tft_predict_measure(pred)\r\n";
            cmd += "sink()\r\n";

            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "library(gtable)\r\n";
            cmd += "library(gridExtra)\r\n";
            cmd += "library(grid)\r\n";
            cmd += "library(ggplot2)\r\n";
            cmd += "meas <- tft_predict_measure(pred)\r\n";
            cmd += "g_ <- gridExtra::tableGrob(meas[[2]])\r\n";
            cmd += "ggsave(file = \"tft_predict_measure_"+base_name +".png\", plot = g_,dpi=100, width= 1.5*6.4,height=0.09*4.8"+ "*length(unique(input_df$key)), limitsize = FALSE)\r\n";


            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "fi <- permutationFeatureImportance(fitted, test, validation=F, base_name ='" + base_name + "')\r\n";
            cmd += "fi_plot <- gridExtra::grid.arrange(fi[[2]], fi[[3]], nrows = 2)\r\n";
            cmd += "ggsave(file = \"tft_fi_plot_" + base_name + ".png\", plot = fi_plot,dpi=100, width= 1.5*6.4,height=0.09*4.8" + "*fi[[1]], limitsize = FALSE)\r\n";
            cmd += "fi_plot1 <- ggplotly(fi[[2]])\r\n";
            cmd += "fi_plot2 <- ggplotly(fi[[3]])\r\n";
            cmd += "fi_plotly <- subplot(fi_plot1, fi_plot2, nrows = 2)\r\n";
            cmd += "print(fi_plotly)\r\n";
            cmd += "htmlwidgets::saveWidget(as_widget(fi_plotly), \"tft_" + base_name + "_fi.html\", selfcontained = F)\r\n";
            return cmd;
        }

        public void listBox_remake()
        {
            colname_list = GetNames();

            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox8.Items.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            for (int i = 0; i < colname_list.Items.Count; i++)
            {
                listBox1.Items.Add(colname_list.Items[i]);
                listBox2.Items.Add(colname_list.Items[i]);
                listBox3.Items.Add(colname_list.Items[i]);
                listBox4.Items.Add(colname_list.Items[i]);
                comboBox3.Items.Add(colname_list.Items[i]);
                comboBox4.Items.Add(colname_list.Items[i]);
                comboBox5.Items.Add(colname_list.Items[i]);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            linkLabel1.LinkVisited = false;
            link1 = "";
            linkLabel2.LinkVisited = false;
            link2 = "";
            linkLabel3.LinkVisited = false;
            link3 = "";
            linkLabel4.LinkVisited = false;
            link4 = "";
            linkLabel5.LinkVisited = false;
            link5 = "";

            //button3.Enabled = false;
            //button6.Enabled = false;
            //button7.Enabled = false;
            //button8.Enabled = false;
            //button9.Enabled = false;

            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox8.Text = "";

            label17.Text = "";
            toolTip1.SetToolTip(label7, "");
            if ( openFileDialog1.ShowDialog() != DialogResult.OK )
            {
                return;
            }
            System.IO.Directory.SetCurrentDirectory(exePath+ "\\..\\..\\..\\");
            Directory.CreateDirectory("work");
            System.IO.Directory.SetCurrentDirectory(exePath + "\\..\\..\\..\\");

            base_dir = System.IO.Directory.GetCurrentDirectory();
            work_dir = base_dir + "\\work";
            System.IO.Directory.SetCurrentDirectory(work_dir);


            csv_file = openFileDialog1.FileName;
            csv_dir = Path.GetDirectoryName(csv_file);
            base_name = Path.GetFileNameWithoutExtension(csv_file);
            
            Directory.CreateDirectory(base_name);
            work_dir = base_dir + "\\work\\"+ base_name;
            System.IO.Directory.SetCurrentDirectory(work_dir);


            File.Copy(csv_file, base_name + ".csv", true);

            listBox_remake();

            try
            {
                //load("");
            }
            catch { }

            string file = exePath + "R_install_path.txt";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(textBox1.Text+"\n");
                }
            }
            catch
            {
                if (MessageBox.Show("R_install_path", "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }
        }

        void predict_measure()
        {
            string cmd = "";

            cmd += "load(\"tft_" + base_name + ".RData\")\r\n";
            cmd += "tft_predict_measure(pred)\r\n";

            string file = "tft_" + base_name + "_predict_measure.R";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write("options(width=1000)\r\n");
                    sw.Write("sink(file = \"tft_predict_measure.txt\")\r\n");
                    sw.Write(cmd);
                    sw.Write("sink()\r\n");
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in "+file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return ;
            }

            execute(file);
        }

        void clear()
        {
            if (File.Exists("tft_train_errorLog_" + base_name + ".txt"))
            {
                File.Delete("tft_train_errorLog_" + base_name + ".txt");
            }
            if (File.Exists("tft_predict_errorLog_" + base_name + ".txt"))
            {
                File.Delete("tft_predict_errorLog_" + base_name + ".txt");
            }
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                File.Delete("tft_predict_measure_" + base_name + ".png");
            }
            if (File.Exists(base_name + "_predict_measure.txt"))
            {
                File.Delete(base_name + "_predict_measure.txt");
            }
            if (File.Exists("tft_" + base_name + ".RData"))
            {
                File.Delete("tft_" + base_name + ".RData");
            }
            if (File.Exists(base_name + "_fitted.rd"))
            {
                File.Delete(base_name + "_fitted.rd");
            }
            if (File.Exists(base_name + "_predict.png"))
            {
                File.Delete(base_name + "_predict.png");
            }
            if (File.Exists(base_name + "_predict_real.png"))
            {
                File.Delete(base_name + "_predict_real.png");
            }
            if (File.Exists(base_name + "_p_learn_rate_plot.png"))
            {
                File.Delete(base_name + "_p_learn_rate_plot.png");
            }
            if (File.Exists(base_name + "_p_input_plot.png"))
            {
                File.Delete(base_name + "_p_input_plot.png");
            }
            if (File.Exists(base_name + "_fitted_plot.png"))
            {
                File.Delete(base_name + "_fitted_plot.png");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_plt0.html");
            }
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {
                File.Delete(work_dir + "\\tft_" + base_name + "_p.html");
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            if ( comboBox3.Text == "")
            {
                MessageBox.Show("time_colname");
                return;
            }
            if (comboBox3.Text == "")
            {
                MessageBox.Show("target_colname");
                return;
            }

            if (checkBox11.Checked) button8.Enabled = true;
            button7.Enabled = true;

            clear();

            plot_time_unit = comboBox2.Text;
            label17.Text = "";
            toolTip1.SetToolTip(label7, "");

            string cmd = tft_init();
            cmd += tft_train();
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += tft_test();

            save();

            string file = "tft_"+base_name +".R";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in "+file , "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            execute(file, false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            load("");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (plot_time_unit != comboBox2.Text || !System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
            {

                string cmd = "";

                cmd += tft_header_ru();
                cmd += "library(plotly)\r\n";
                cmd += "library(htmlwidgets)\r\n";
                cmd += "load(\"tft_" + base_name + ".RData\")\r\n";
                cmd += "plt0 <- tft_predict_plot(pred, timestep='" + comboBox2.Text + "')\r\n";
                cmd += "\r\n";
                cmd += "ggsave(file = \"" + base_name + "_predict.png\", plot = plt0, dpi = 100, width = 6.4, height = 4.8)\r\n";
                cmd += "plt0_plotly <- ggplotly(plt0)\r\n";
                cmd += "print(plt0_plotly)\r\n";
                cmd += "htmlwidgets::saveWidget(as_widget(plt0_plotly), \"tft_" + base_name + "_plt0.html\", selfcontained = F)\r\n";

                string file = "tft_" + base_name + "_plt0_html.R";

                try
                {
                    using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                    {
                        sw.Write("options(width=1000)\r\n");
                        sw.Write(cmd);
                    }
                }
                catch
                {
                    if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                }

                execute(file);
                System.Threading.Thread.Sleep(50);
            }

            string webpath = work_dir + "/tft_" + base_name + "_plt0.html";
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
            {
                webpath = webpath.Replace("\\", "/").Replace("//", "/");

                link1 = webpath;
                linkLabel1.Visible = true;
                linkLabel1.LinkVisited = true;
                linkLabel1.Refresh();
            }
            else
            {
                //return;
            }

            interactivePlot plot = new interactivePlot();

            try
            {
                plot.pictureBox1.Image = null;
                pictureBox4.Image = null;
                if (System.IO.File.Exists(base_name+"_predict.png"))
                {

                    plot.pictureBox1.Image = interactivePlot.CreateImage(base_name + "_predict.png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    TopMost = true;
                    TopMost = false;
                    pictureBox4.Image = interactivePlot.CreateImage(base_name + "_predict.png");
                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                    tabControl1.SelectedIndex = 3;
                }
            }
            catch { }

            try
            {
                plot.webView21.Source = new Uri(webpath);
                if (plot.webView21.CoreWebView2 != null)
                {
                    //plot.webView21.CoreWebView2.Navigate(webpath);
                }

                plot.webView21.Refresh();
                webView24.Source = new Uri(webpath);
                webView24.Refresh();
            }
            catch { }

            plot.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (link1 == "") return;
            try
            {
                System.Diagnostics.Process.Start(link1, null);
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (plot_time_unit != comboBox2.Text || !System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {

                string cmd = "";

                cmd += tft_header_ru();
                cmd += "library(plotly)\r\n";
                cmd += "library(htmlwidgets)\r\n";
                cmd += "load(\"tft_" + base_name + ".RData\")\r\n";
                cmd += "plt3 <- tft_predict_plot(pred, timestep='" + comboBox2.Text + "', use_real_data=T)\r\n";
                cmd += "plt2 <- tft_predict_check(pred, timestep='" + comboBox2.Text + "')\r\n";
                cmd += "p <- gridExtra::grid.arrange(plt3, plt2, ncol = 1)\r\n";
                cmd += "ggsave(file = \"" + base_name + "_predict_real.png\", plot = p, dpi = 100, width = 6.4, height = 4.8)\r\n";
                cmd += "p_plotly <- subplot(plt3, plt2, nrows = 2)\r\n";
                cmd += "print(p_plotly)\r\n";
                cmd += "htmlwidgets::saveWidget(as_widget(p_plotly), \"tft_" + base_name + "_p.html\", selfcontained = F)\r\n";

                string file = "tft_" + base_name + "_p_html.R";

                try
                {
                    using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                    {
                        sw.Write("options(width=1000)\r\n");
                        sw.Write(cmd);
                    }
                }
                catch
                {
                    if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                }

                execute(file);
                System.Threading.Thread.Sleep(50);
            }

            string webpath = work_dir + "/tft_" + base_name + "_p.html";
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
            {
                webpath = webpath.Replace("\\", "/").Replace("//", "/");

                link2 = webpath;
                linkLabel2.Visible = true;
                linkLabel2.LinkVisited = true;
                linkLabel2.Refresh();
            }
            else
            {
                //return;
            }

            interactivePlot plot = new interactivePlot();

            try
            {
                plot.pictureBox1.Image = null;
                pictureBox5.Image = null;
                if (System.IO.File.Exists(base_name + "_predict_real.png"))
                {

                    plot.pictureBox1.Image = interactivePlot.CreateImage(base_name + "_predict_real.png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    TopMost = true;
                    TopMost = false;
                    pictureBox5.Image = interactivePlot.CreateImage(base_name + "_predict_real.png");
                    pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                    tabControl1.SelectedIndex = 4;
                }
            }
            catch { }

            try
            {
                plot.webView21.Source = new Uri(webpath);
                if (plot.webView21.CoreWebView2 != null)
                {
                    //plot.webView21.CoreWebView2.Navigate(webpath);
                }
                plot.webView21.Refresh();
                webView25.Source = new Uri(webpath);
                webView25.Refresh();
            }
            catch { }

            plot.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (link2 == "") return;
            try
            {
                System.Diagnostics.Process.Start(link2, null);
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (plot_time_unit != comboBox2.Text || !System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_input_plot.html"))
            {

                string cmd = "";

                cmd += tft_header_ru();
                cmd += "load(\"tft_" + base_name + ".RData\")\r\n";
                cmd += "input_plot <- tft_plot_input(input_df, unit='" + comboBox2.Text + "')\r\n";
                cmd += "\r\n";
                cmd += "ggsave(file = \"" + base_name + "_p_input_plot.png\", plot = input_plot, dpi = 100, width = 6.4, height = 4.8)\r\n";
                cmd += "p_input_plot <- ggplotly(input_plot)\r\n";
                cmd += "print(p_input_plot)\r\n";
                cmd += "htmlwidgets::saveWidget(as_widget(p_input_plot), \"tft_" + base_name + "_p_input_plot.html\", selfcontained = F)\r\n";

                string file = "tft_" + base_name + "_p_input_plot_html.R";

                try
                {
                    using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                    {
                        sw.Write("options(width=1000)\r\n");
                        sw.Write(cmd);
                    }
                }
                catch
                {
                    if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                }

                execute(file);
                System.Threading.Thread.Sleep(50);
            }
            string webpath = work_dir + "/tft_" + base_name + "_p_input_plot.html";
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_input_plot.html"))
            {
                webpath = webpath.Replace("\\", "/").Replace("//", "/");

                link3 = webpath;
                linkLabel3.Visible = true;
                linkLabel3.LinkVisited = true;
                linkLabel3.Refresh();
            }
            else
            {
                //return;
            }

            interactivePlot plot = new interactivePlot();

            try
            {
                plot.pictureBox1.Image = null;
                pictureBox1.Image = null;
                if (System.IO.File.Exists(base_name + "_p_input_plot.png"))
                {

                    plot.pictureBox1.Image = interactivePlot.CreateImage(base_name + "_p_input_plot.png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    
                    TopMost = true;
                    TopMost = false;

                    pictureBox1.Image = interactivePlot.CreateImage(base_name + "_p_input_plot.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    tabControl1.SelectedIndex = 0;
                }
            }
            catch { }

            try
            {
                plot.webView21.Source = new Uri(webpath);
                if (plot.webView21.CoreWebView2 != null)
                {
                    //plot.webView21.CoreWebView2.Navigate(webpath);
                }
                plot.webView21.Refresh();
                webView21.Source = new Uri(webpath);
                webView21.Refresh();
            }
            catch { }

            plot.Show();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start(link5, null);
            button9_Click(sender, null);
        }

        public void button10_Click(object sender, EventArgs e)
        {
            for ( int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox4.SetSelected(i, true);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                if ( listBox4.GetSelected(i)) listBox1.SetSelected(i, true);
                else listBox1.SetSelected(i, false);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox2.SetSelected(i, false);

                if (listBox4.GetSelected(i) && !listBox1.GetSelected(i))
                {
                    listBox2.SetSelected(i, true);
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox3.SetSelected(i, false);

                if (listBox4.GetSelected(i) && !listBox1.GetSelected(i) & !listBox2.GetSelected(i))
                {
                    listBox3.SetSelected(i, true);
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (File.Exists(base_name + "_predict_measure.txt"))
            {
                File.Delete(base_name + "_predict_measure.txt");
            }
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                File.Delete("tft_predict_measure_" + base_name + ".png");
            }
            plot_time_unit = comboBox2.Text;

            string cmd = tft_header_ru();
            cmd += "\r\n";
            cmd += "\r\n";
            cmd += "load(\"tft_" + base_name + "_fitted.RData\")\r\n";
            cmd += tft_test();

            save();

            string file = "tft_" + base_name + "_predict.R";
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in "+file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            execute(file, false);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox4.SetSelected(i, !listBox4.GetSelected(i));
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string webpath = "";
            if (tabControl1.SelectedIndex == 0)
            {
                pictureBox1.Image = null;
                if (System.IO.File.Exists(base_name + "_p_input_plot.png"))
                {
                    pictureBox1.Image = interactivePlot.CreateImage(base_name + "_p_input_plot.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                pictureBox1.Refresh();
                webpath = work_dir + "/tft_" + base_name + "_p_input_plot.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_input_plot.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView21.Source = new Uri(webpath);
                        webView21.Refresh();
                    }
                    catch { }
                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                pictureBox2.Image = null;
                if (System.IO.File.Exists(base_name + "_p_learn_rate_plot.png"))
                {
                    pictureBox2.Image = interactivePlot.CreateImage(base_name + "_p_learn_rate_plot.png");
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                pictureBox2.Refresh();
                webpath = work_dir + "/tft_" + base_name + "_p_learn_rate_plot.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView22.Source = new Uri(webpath);
                        webView22.Refresh();
                    }
                    catch { }
                }
            }
            if (tabControl1.SelectedIndex == 2)
            {
                pictureBox3.Image = null;
                if (System.IO.File.Exists(base_name + "_fitted_plot.png"))
                {
                    pictureBox3.Image = interactivePlot.CreateImage(base_name + "_fitted_plot.png");
                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                pictureBox3.Refresh();
            }
            if (tabControl1.SelectedIndex == 3)
            {
                pictureBox4.Image = null;
                if (System.IO.File.Exists(base_name + "_predict.png"))
                {

                    pictureBox4.Image = interactivePlot.CreateImage(base_name + "_predict.png");
                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                pictureBox4.Refresh();
                webpath = work_dir + "/tft_" + base_name + "_plt0.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_plt0.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView24.Source = new Uri(webpath);
                        webView24.Refresh();
                    }
                    catch { }
                }
            }
            if (tabControl1.SelectedIndex == 4)
            {
                pictureBox5.Image = null;
                if (System.IO.File.Exists(base_name + "_predict_real.png"))
                {
                    pictureBox5.Image = interactivePlot.CreateImage(base_name + "_predict_real.png");
                    pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                pictureBox5.Refresh();
                webpath = work_dir + "/tft_" + base_name + "_p.html";
                if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p.html"))
                {
                    webpath = webpath.Replace("\\", "/").Replace("//", "/");
                    try
                    {
                        webView25.Source = new Uri(webpath);
                        webView25.Refresh();
                    }
                    catch { }
                }
            }
            if ( tabControl1.SelectedIndex == 5)
            {
                if (File.Exists("tft_predict_measure_" + base_name + ".png"))
                {
                    try
                    {
                        pictureBox6.Image = CreateImage("tft_predict_measure_" + base_name + ".png");
                    }
                    catch { }
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.ShowDialog();
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            comboBox6.Enabled = checkBox17.Checked;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (File.Exists("tft_predict_measure_" + base_name + ".png"))
            {
                interactivePlot plot = new interactivePlot();
                try
                {
                    plot.pictureBox1.Image = null;
                    plot.pictureBox1.Image = interactivePlot.CreateImage("tft_predict_measure_" + base_name + ".png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                    pictureBox6.Image = CreateImage("tft_predict_measure_" + base_name + ".png");
                }
                catch { }
                plot.Show();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            var keys = GetKeys();
            if (keys == null) return;
            for (int i = 0; i < keys.Items.Count; i++)
            {
                comboBox8.Items.Add(keys.Items[i]);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            interactivePlot plot = new interactivePlot();

            try
            {
                plot.pictureBox1.Image = null;
                pictureBox3.Image = null;
                if (System.IO.File.Exists(base_name + "_fitted_plot.png"))
                {

                    plot.pictureBox1.Image = interactivePlot.CreateImage(base_name + "_fitted_plot.png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    TopMost = true;
                    TopMost = false;
                    pictureBox3.Image = interactivePlot.CreateImage(base_name + "_fitted_plot.png");
                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                    tabControl1.SelectedIndex = 2;
                }
            }
            catch { }

            plot.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (link3 == "") return;
            try
            {
                System.Diagnostics.Process.Start(link3, null);
            }
            catch { }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
            {

                string cmd = "";

                cmd = tft_header_ru();
                cmd += "library(plotly)\r\n";
                cmd += "library(htmlwidgets)\r\n";
                cmd += "load(\"tft_" + base_name + ".RData\")\r\n";
                cmd += "p_learn_rate_plot <- ggplotly(learn_rate_plot)\r\n";
                cmd += "print(p_learn_rate_plot)\r\n";
                cmd += "htmlwidgets::saveWidget(as_widget(p_learn_rate_plot), \"tft_" + base_name + "_p_learn_rate_plot.html\", selfcontained = F)\r\n";

                string file = "tft_" + base_name + "_p_learn_rate_plot_html.R";

                try
                {
                    using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                    {
                        sw.Write("options(width=1000)\r\n");
                        sw.Write(cmd);
                    }
                }
                catch
                {
                    if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        return;
                }

                execute(file);
                System.Threading.Thread.Sleep(50);
            }

            string webpath = work_dir + "/tft_" + base_name + "_p_learn_rate_plot.html";
            if (System.IO.File.Exists(work_dir + "\\tft_" + base_name + "_p_learn_rate_plot.html"))
            {
                webpath = webpath.Replace("\\", "/").Replace("//", "/");

                link4 = webpath;
                linkLabel4.Visible = true;
                linkLabel4.LinkVisited = true;
                linkLabel4.Refresh();
            }
            else
            {
                //return;
            }

            interactivePlot plot = new interactivePlot();

            try
            {
                plot.pictureBox1.Image = null;
                pictureBox2.Image = null;
                if (System.IO.File.Exists(base_name + "_p_learn_rate_plot.png"))
                {

                    plot.pictureBox1.Image = interactivePlot.CreateImage(base_name + "_p_learn_rate_plot.png");
                    plot.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                    TopMost = true;
                    TopMost = false;
                    pictureBox2.Image = interactivePlot.CreateImage(base_name + "_p_learn_rate_plot.png");
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    tabControl1.SelectedIndex = 1;
                }
            }
            catch { }

            try
            {
                plot.webView21.Source = new Uri(webpath);
                if (plot.webView21.CoreWebView2 != null)
                {
                    //plot.webView21.CoreWebView2.Navigate(webpath);
                }
                plot.webView21.Refresh();
                webView22.Source = new Uri(webpath);
                webView22.Refresh();
            }
            catch { }

            plot.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (link4 == "") return;
            try
            {
                System.Diagnostics.Process.Start(link4, null);
            }
            catch { }
        }
    }
}
